using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common.CSV;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;
using InventoryItemSource = InventoryDS.Business.Entities.InventoryItemSource;

namespace WaterNut.DataSpace
{
    public class SaveCsvEntryData
    {
        private static readonly SaveCsvEntryData instance;

        static SaveCsvEntryData()
        {
            instance = new SaveCsvEntryData();
        }

        public static SaveCsvEntryData Instance
        {
            get { return instance; }
        }

        public async Task<bool> ExtractEntryData(string fileType, string[] lines, string[] headings, string csvType,
            List<AsycudaDocumentSet> docSet, bool overWriteExisting, int? emailId, int? fileTypeId,
            string droppedFilePath)
        {
            try
            {


                if (docSet == null)
                {
                    throw new ApplicationException("Please select Document Set before proceding!");

                }

                var mapping = new Dictionary<string, int>();
                GetMappings(mapping, headings);

                if (fileType == "Sales" && !mapping.ContainsKey("Tax"))
                    throw new ApplicationException("Sales file dose not contain Tax");


                var eslst = GetCSVDataSummayList(lines, mapping, headings);

                if (eslst == null) return true;


                if (csvType == "QB9")
                {
                    foreach (var item in eslst)
                    {
                        item.ItemNumber = item.ItemNumber.Split(':').Last();
                    }
                }

                var i = Array.IndexOf(headings,"Instructions");
                List<string> instructions = new List<string>();
                if (i > 0)
                {
                    instructions = lines.Select(x => x.Split(',')[i]).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }

                if (instructions.Contains("Append")) overWriteExisting = false;
                if (instructions.Contains("Replace")) overWriteExisting = true;

                return await ProcessCsvSummaryData(fileType, docSet, overWriteExisting, emailId, fileTypeId,
                    droppedFilePath, eslst).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nex = new ApplicationException($"Error Importing File: {droppedFilePath} - {e.Message}", e);
                Console.WriteLine(nex);
                throw nex;
            }
        }

        public async Task<bool> ProcessCsvSummaryData(string fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, int? emailId,
            int? fileTypeId, string droppedFilePath, List<CSVDataSummary> eslst)
        {
            try
            {
                await ImportInventory(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                await ImportSuppliers(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                await ImportEntryDataFile(fileType, eslst.Where(x => !string.IsNullOrEmpty(x.SourceRow)).ToList(),
                    emailId, fileTypeId, droppedFilePath, docSet.First().ApplicationSettingsId).ConfigureAwait(false);
                if (await ImportEntryData(fileType, eslst, docSet, overWriteExisting, emailId, fileTypeId,
                        droppedFilePath)
                    .ConfigureAwait(false)) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private async Task ImportEntryDataFile(string fileType, List<CSVDataSummary> eslst, int? emailId,
            int? fileTypeId, string droppedFilePath, int applicationSettingsId)
        {
            using (var ctx = new EntryDataDSContext())
            {

                ctx.Database.ExecuteSqlCommand($@"delete from EntryDataFiles where SourceFile = '{droppedFilePath}'");
                foreach (var line in eslst)
                {
                    var drow = new EntryDataFiles(true)
                    {
                        TrackingState = TrackingState.Added,
                        SourceFile = droppedFilePath,
                        ApplicationSettingsId = applicationSettingsId,
                        SourceRow = line.SourceRow,
                        FileType = fileType,
                        EmailId = emailId,
                        FileTypeId = fileTypeId,
                        EntryDataId = line.EntryDataId,
                        EntryDataDate = line.EntryDataDate,
                        Cost = line.Cost,
                        InvoiceQty = line.InvoiceQuantity,
                        ItemDescription = line.ItemDescription,
                        ItemNumber = line.ItemNumber,
                        LineNumber = line.LineNumber,
                        Quantity = line.Quantity,
                        ReceivedQty = line.ReceivedQuantity,
                        TaxAmount = line.Tax,
                        TotalCost = line.TotalCost,
                        Units = line.Units
                    };
                    ctx.EntryDataFiles.Add(drow);
                }

                ctx.SaveChanges();
            }
        }

        private async Task<bool> ImportEntryData(string fileType, List<CSVDataSummary> eslst,
            List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, int? emailId, int? fileTypeId, string droppedFilePath)
        {
            try
            {
                var ndocSet = new List<AsycudaDocumentSet>();
                if (overWriteExisting == true) ndocSet = docSet;

                var ed = (from es in eslst
                    group es by new {es.EntryDataId, es.EntryDataDate, es.CustomerName, es.Currency}
                    into g
                    select new
                    {
                        EntryData =
                            new
                            {
                                EntryDataId = g.Key.EntryDataId,
                                EntryDataDate = g.Key.EntryDataDate,
                                AsycudaDocumentSetId = docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.AsycudaDocumentSetId ?? docSet.First().AsycudaDocumentSetId,
                                ApplicationSettingsId = docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.ApplicationSettingsId ?? docSet.First().ApplicationSettingsId,
                                CustomerName = g.Key.CustomerName,
                                Tax = g.Sum(x => x.Tax),
                                Supplier = string.IsNullOrEmpty(g.Max(x => x.SupplierCode))
                                    ? null
                                    : g.Max(x => x.SupplierCode?.ToUpper()),
                                Currency = g.Key.Currency,
                                EmailId = emailId,
                                FileTypeId = fileTypeId,
                                DocumentType = g.FirstOrDefault(x => x.DocumentType != "")?.DocumentType,
                                SupplierInvoiceNo = g.FirstOrDefault(x => x.SupplierInvoiceNo != "")?.SupplierInvoiceNo,
                                PONumber = g.FirstOrDefault(x => x.PONumber != "")?.PONumber,
                                SourceFile = droppedFilePath,
                                
                            },
                        EntryDataDetails = g.Where(x => !string.IsNullOrEmpty(x.ItemNumber)).Select(x =>
                            new EntryDataDetails()
                            {
                                EntryDataId = x.EntryDataId,
                                //Can't set entrydata_id here cuz this is from data
                                ItemNumber = x.ItemNumber.ToUpper(),
                                ItemDescription = x.ItemDescription,
                                Cost = Convert.ToDouble(x.Cost),
                                TotalCost = Convert.ToDouble(x.TotalCost),
                                Quantity = Convert.ToDouble(x.Quantity),
                                FileLineNumber = x.LineNumber,
                                Units = x.Units,
                                Freight = x.Freight,
                                Weight = x.Weight,
                                InternalFreight = x.InternalFreight,
                                InvoiceQty = x.InvoiceQuantity,
                                ReceivedQty = x.ReceivedQuantity,
                                TaxAmount = x.Tax,
                                CNumber = x.CNumber,
                                PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                                Comment = x.Comment,
                                InventoryItemId = x.InventoryItemId,
                                EffectiveDate = x.EffectiveDate,

                            }),
                        f = g.Select(x => new
                        {
                            TotalWeight = x.TotalWeight,
                            TotalFreight = x.TotalFreight,
                            TotalInternalFreight = x.TotalInternalFreight,
                            TotalOtherCost = x.TotalOtherCost,
                            TotalInsurance = x.TotalInsurance,
                            TotalDeductions = x.TotalDeductions,
                            InvoiceTotal = x.InvoiceTotal,
                            Packages = x.Packages,
                            WarehouseNo = x.WarehouseNo,




                        }),
                        InventoryItems = g.DistinctBy(x => new {x.ItemNumber, x.ItemAlias})
                            .Select(x => new {x.ItemNumber, x.ItemAlias})
                    }).ToList();

                if (ed == null) return true;


                List<EntryData> eLst = null;

                //Parallel.ForEach(ed, new ParallelOptions() { MaxDegreeOfParallelism = 3 },//Environment.ProcessorCount * 1
                //    async item =>
                    foreach (var item in ed)

                    {
                        if (!item.EntryDataDetails.Any())
                            throw new ApplicationException(item.EntryData.EntryDataId + " has no details");

                        List<EntryDataDetails> details = new List<EntryDataDetails>();


                    // check Existing items
                    //var oldeds = await GetEntryData(item.EntryData.EntryDataId, docSet,
                    //    item.EntryData.ApplicationSettingsId).ConfigureAwait(false);

                    var oldeds = new EntryDataDSContext().EntryData
                            .Include("AsycudaDocumentSets")
                            .Include("EntryDataDetails")
                            .Where(x => x.EntryDataId == item.EntryData.EntryDataId 
                                        && x.ApplicationSettingsId == item.EntryData.ApplicationSettingsId ).ToList()
                            .Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any())
                            .ToList();

                    var olded = overWriteExisting ? null : oldeds.FirstOrDefault();
                        if (oldeds.Any())
                        {
                            if (overWriteExisting)
                            {
                                foreach (var itm in oldeds)
                                {
                                   await ClearEntryDataDetails(itm).ConfigureAwait(false);
                                   await DeleteEntryData(itm).ConfigureAwait(false); 
                                }
                                
                                details = item.EntryDataDetails.ToList();
                            }
                            else
                            {

                                foreach (var doc in docSet)
                                {
                                    
                                    var l = 0;
                                    foreach (var nEd in item.EntryDataDetails.ToList())
                                    {
                                        l += 1;

                                        var oEd = olded.EntryDataDetails.FirstOrDefault(x =>
                                            x.LineNumber == l && x.ItemNumber == nEd.ItemNumber &&
                                            x.EntryData.AsycudaDocumentSets.Any(z =>
                                                z.AsycudaDocumentSetId == doc.AsycudaDocumentSetId));

                                        //if (l != item.EntryDataDetails.Count()) continue;
                                        if (oEd != null && (Math.Abs(nEd.Quantity - oEd.Quantity) < .0001 &&
                                                            Math.Abs(nEd.Cost - oEd.Cost) < .0001))
                                        {

                                            continue;
                                        }

                                        if (ndocSet.FirstOrDefault(x =>
                                                x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId) ==
                                            null) ndocSet.Add(doc);
                                        if (details.FirstOrDefault(x =>
                                                x.ItemNumber == nEd.ItemNumber && x.LineNumber == nEd.LineNumber) ==
                                            null) details.Add(nEd);
                                        if (oEd != null)
                                            new EntryDataDetailsService()
                                                .DeleteEntryDataDetails(oEd.EntryDataDetailsId.ToString()).Wait();

                                    }

                                }


                            }
                        }

                        if (olded == null)
                        {
                            details = item.EntryDataDetails.ToList();
                            ndocSet = docSet;
                        }

                        if (overWriteExisting || olded == null)
                        {



                            switch (fileType)
                            {
                                case "Sales":

                                    var EDsale = new Sales(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        INVNumber = item.EntryData.EntryDataId,
                                        CustomerName = item.EntryData.CustomerName,
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        SourceFile = item.EntryData.SourceFile,
                                        TrackingState = TrackingState.Added,

                                    };
                                    if (item.EntryData.DocumentType != "")
                                        EDsale.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDsale);


                                    olded = await CreateSales(EDsale).ConfigureAwait(false);
                                    break;
                                case "INV":
                                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                                        Math.Abs((double) item.f.Sum(x => x.InvoiceTotal)) < .001)
                                        throw new ApplicationException(
                                            $"{item.EntryData.EntryDataId} has no Invoice Total. Please check File.");
                                    var EDinv = new Invoices(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => x.TotalDeductions),
                                        Packages = item.f.Sum(x => x.Packages),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),

                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        PONumber = string.IsNullOrEmpty(item.EntryData.PONumber)
                                            ? null
                                            : item.EntryData.PONumber,

                                    };
                                    if (!string.IsNullOrEmpty(item.EntryData.DocumentType))
                                        EDinv.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDinv);
                                    olded = await CreateInvoice(EDinv).ConfigureAwait(false);

                                    break;
                                case "PO":
                                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                                        Math.Abs((double) item.f.Sum(x => x.InvoiceTotal)) < .001)
                                        throw new ApplicationException(
                                            $"{item.EntryData.EntryDataId} has no Invoice Total. Please check File.");
                                    var EDpo = new PurchaseOrders(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        PONumber = item.EntryData.EntryDataId,
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        Packages = item.f.Sum(x => x.Packages),
                                        
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        SupplierInvoiceNo = string.IsNullOrEmpty(item.EntryData.SupplierInvoiceNo)
                                            ? null
                                            : item.EntryData.SupplierInvoiceNo,
                                        

                                    };
                                    foreach (var warehouseNo in item.f.Where(x => !string.IsNullOrEmpty(x.WarehouseNo)))
                                    {
                                        //var poList = warehouseNo.WarehouseNo.Split(new string[] {"|", ","},
                                        //    StringSplitOptions.RemoveEmptyEntries);
                                        //foreach (var w in poList)
                                        //{
                                            EDpo.WarehouseInfo.Add(new WarehouseInfo()
                                            {
                                                WarehouseNo = warehouseNo.WarehouseNo,
                                                Packages = warehouseNo.Packages,
                                                EntryData_PurchaseOrders = EDpo,
                                                TrackingState = TrackingState.Added
                                            });
                                        //}
                                    }
                                    if (!string.IsNullOrEmpty(item.EntryData.DocumentType))
                                        EDpo.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDpo);
                                    olded = await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
                                    break;
                                case "OPS":
                                    var EDops = new OpeningStock(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        OPSNumber = item.EntryData.EntryDataId,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                    };
                                    if (item.EntryData.DocumentType != "")
                                        EDops.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDops);
                                    olded = await CreateOpeningStock(EDops).ConfigureAwait(false);
                                    break ;
                                case "ADJ":
                                    var EDadj = new Adjustments(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        TrackingState = TrackingState.Added,
                                        SupplierCode = item.EntryData.Supplier,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        Type = "ADJ"
                                    };
                                    if (item.EntryData.DocumentType != "")
                                        EDadj.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDadj);
                                    olded = await CreateAdjustments(EDadj).ConfigureAwait(false);
                                    break;
                                case "DIS":
                                    var EDdis = new Adjustments(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        Type = "DIS"
                                    };
                                    if (item.EntryData.DocumentType != "")
                                        EDdis.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDdis);
                                    olded = await CreateAdjustments(EDdis).ConfigureAwait(false);
                                    break;
                                case "RCON":
                                    var EDrcon = new Adjustments(true)
                                    {
                                        ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                        EntryDataId = item.EntryData.EntryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => x.TotalWeight),
                                        InvoiceTotal = item.f.Sum(x => x.InvoiceTotal),
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        Type = "RCON"
                                    };
                                    if (item.EntryData.DocumentType != "")
                                        EDrcon.DocumentType = new EDDocumentTypes(true)
                                        {
                                            DocumentType = item.EntryData.DocumentType,
                                            TrackingState = TrackingState.Added

                                        };
                                    AddToDocSet(ndocSet, EDrcon);
                                    olded = await CreateAdjustments(EDrcon).ConfigureAwait(false);
                                    break;
                                default:
                                    throw new ApplicationException("Unknown FileType");

                            }
                        }
                        else
                        {
                            olded.EmailId = emailId;
                        }



                        using (var ctx = new EntryDataDSContext())
                        {
                            var lineNumber = 0;
                            foreach (var e in overWriteExisting || olded == null
                                ? details
                                : details.Where(x =>
                                    olded.EntryDataDetails.FirstOrDefault(z =>
                                        z.ItemNumber == x.ItemNumber && z.LineNumber == x.LineNumber) == null))
                            {
                                lineNumber += 1;
                                ctx.EntryDataDetails.Add(new EntryDataDetails(true)
                                {
                                    EntryDataId = e.EntryDataId,
                                    EntryData_Id = olded?.EntryData_Id ?? 0,
                                    ItemNumber = e.ItemNumber.Truncate(20),
                                    InventoryItemId = e.InventoryItemId,
                                    ItemDescription = e.ItemDescription,
                                    Quantity = e.Quantity,
                                    Cost = e.Cost,
                                    TotalCost = e.TotalCost,
                                    Units = e.Units,
                                    TrackingState = TrackingState.Added,
                                    Freight = e.Freight,
                                    Weight = e.Weight,
                                    InternalFreight = e.InternalFreight,
                                    ReceivedQty = e.ReceivedQty,
                                    InvoiceQty = e.InvoiceQty,
                                    CNumber = string.IsNullOrEmpty(e.CNumber) ? null : e.CNumber,
                                    PreviousInvoiceNumber = string.IsNullOrEmpty(e.PreviousInvoiceNumber)
                                        ? null
                                        : e.PreviousInvoiceNumber,
                                    Comment = string.IsNullOrEmpty(e.Comment) ? null : e.Comment,
                                    FileLineNumber = e.FileLineNumber ?? lineNumber,
                                    LineNumber = e.EntryDataDetailsId == 0 ? lineNumber : e.LineNumber,
                                    EffectiveDate = e.EffectiveDate,
                                    TaxAmount = e.TaxAmount,
                                });
                            }
                            ctx.SaveChanges();
                            if (!overWriteExisting && olded != null)
                            {

                                //update entrydatadetails
                                foreach (var itm in olded.EntryDataDetails)
                                {
                                    var d = details.FirstOrDefault(z =>
                                        z.ItemNumber == itm.ItemNumber && z.LineNumber == itm.LineNumber &&
                                        (Math.Abs(z.Cost - itm.Cost) > 0 || Math.Abs(z.Quantity - itm.Quantity) > 0 ||
                                         Math.Abs(z.TaxAmount.GetValueOrDefault() - itm.TaxAmount.GetValueOrDefault()) >
                                         0));
                                    if (d == null || d.Quantity == 0) continue;

                                    itm.Quantity = d.Quantity;
                                    itm.Cost = d.Cost;
                                    itm.TaxAmount = d.TaxAmount;
                                    ctx.ApplyChanges(itm);
                                }

                                ctx.SaveChanges();
                                AddToDocSet(ndocSet, olded);
                                await UpdateEntryData(olded).ConfigureAwait(false);

                            }

                        }

                        using (var ctx = new InventoryDSContext() {StartTracking = true})
                        {

                            foreach (var e in item.InventoryItems
                                .Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber).ToList())
                            {
                                var inventoryItem = ctx.InventoryItems
                                    .Include("InventoryItemAlias")
                                    .First(x => x.ApplicationSettingsId == item.EntryData.ApplicationSettingsId &&
                                                x.ItemNumber == e.ItemNumber);
                                if (inventoryItem == null) continue;
                                {
                                    if (inventoryItem.InventoryItemAlias.FirstOrDefault(x =>
                                            x.AliasName == e.ItemAlias) ==
                                        null)
                                    {
                                        inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
                                        {
                                            InventoryItemId = inventoryItem.Id,
                                            AliasName = e.ItemAlias.Truncate(20),

                                        });

                                    }
                                }

                            }

                            ctx.SaveChanges();

                        }


                    }//);




                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void AddToDocSet(List<AsycudaDocumentSet> docSet, EntryData entryData)
        {
            foreach (var doc in docSet.DistinctBy(x => x.AsycudaDocumentSetId))
            {
                if ((new EntryDataDSContext()).AsycudaDocumentSetEntryData.FirstOrDefault(
                        x => x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId && x.EntryData_Id == entryData.EntryData_Id) != null) continue;
                if (entryData.AsycudaDocumentSets.FirstOrDefault(
                        x => x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId) != null) continue;
                entryData.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                {
                    AsycudaDocumentSetId = doc.AsycudaDocumentSetId,
                    EntryData_Id = entryData.EntryData_Id,
                    TrackingState = TrackingState.Added
                });
            }
        }

        private async Task<Adjustments> CreateAdjustments(Adjustments eDadj)
        {
            using (var ctx = new AdjustmentsService())
            {
                return await ctx.CreateAdjustments(eDadj).ConfigureAwait(false);
            }
        }

        static ConcurrentDictionary<string, List<EntryData>> loadedEntryData = new ConcurrentDictionary<string, List<EntryData>>();
        private async Task<List<EntryData>> GetEntryData(string entryDataId, List<AsycudaDocumentSet> docSet,
            int applicationSettingsId)
        {
            //if (!loadedEntryData.ContainsKey($"{entryDataId}|{applicationSettingsId}"))
            //{
                using (var ctx = new EntryDataDSContext())
                {
                    var entryDatas = ctx.EntryData
                        .Include("AsycudaDocumentSets")
                        .Include("EntryDataDetails")
                        .Where(x => x.EntryDataId == entryDataId && x.ApplicationSettingsId == applicationSettingsId)
                        .ToList();
                    //var entryDatas = (await ctx.GetEntryDataByExpressionLst(new List<string>()
                    //{
                    //    $"EntryDataId == \"{entryDataId}\"",
                    //    // $"EntryDataDate == \"{entryDateTime.ToString("yyyy-MMM-dd")}\"",
                    //    $"ApplicationSettingsId == \"{applicationSettingsId}\"",
                    //}, new List<string>() {"AsycudaDocumentSets", "EntryDataDetails"}).ConfigureAwait(false));
                    
                    loadedEntryData.AddOrUpdate($"{entryDataId}|{applicationSettingsId}", entryDatas.ToList(), (k,v) => v);
                    //eLst.FirstOrDefault(x => x.EntryDataId == item.e.EntryDataId && x.EntryDataDate != item.e.EntryDataDate);
                }
           // }

            return loadedEntryData[$"{entryDataId}|{applicationSettingsId}"].Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any()).ToList();
        }


        private void GetMappings(Dictionary<string, int> mapping, string[] headings)
        {
            for (var i = 0; i < headings.Count(); i++)
            {
                var h = headings[i].Trim().ToUpper();

                if (h == "") continue;

                if ("Invoice|INVNO|Reciept #|NUM|Invoice #|Invoice#|Order Reference".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("EntryDataId", i);
                    continue;
                }

                if ("DATE|Invoice Date".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("EntryDataDate", i);
                    continue;
                }

                if ("ItemNumber|ITEM-#|Item Code|Product Code".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("ItemNumber", i);
                    continue;
                }

                if ("DESCRIPTION|MEMO|Item Description|ItemDescription|Description 1".ToUpper().Split('|')
                    .Any(x => x == h.ToUpper()))
                {
                    mapping.Add("ItemDescription", i);
                    continue;
                }

                if ("QUANTITY|QTY".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Quantity", i);
                    continue;
                }

                if ("ItemAlias".ToUpper().Split('|').Any(x => x == h.ToUpper())) //Manufact. SKU|
                {
                    mapping.Add("ItemAlias", i);
                    continue;
                }

                if ("PRICE|COST|USD".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Cost", i);
                    continue;
                }

                if ("TotalCost|Total Cost".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalCost", i);
                    continue;
                }

                if ("UNITS".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Units", i);
                    continue;
                }

                if ("Customer".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("CustomerName", i);
                    continue;
                }

                if ("TAX1|Tax".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Tax", i);
                    continue;
                }

                if ("TariffCode".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TariffCode", i);
                    continue;
                }

                if ("Freight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Freight", i);
                    continue;
                }

                if ("Weight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Weight", i);
                    continue;
                }

                if ("InternalFreight|Internal Freight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("InternalFreight", i);
                    continue;
                }

                if ("Insurance".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Insurance", i);
                    continue;
                }

                if ("Other Cost|OtherCost".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("OtherCost", i);
                    continue;
                }

                if ("Deductions".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Deductions", i);
                    continue;
                }

                if ("Total Freight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalFreight", i);
                    continue;
                }

                if ("Total Weight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalWeight", i);
                    continue;
                }

                if ("TotalInternalFreight|Total Internal Freight".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalInternalFreight", i);
                    continue;
                }

                if ("Total Insurance".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalInsurance", i);
                    continue;
                }

                if ("Total Other Cost|TotalOtherCost".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalOtherCost", i);
                    continue;
                }

                if ("Total Deductions".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("TotalDeductions", i);
                    continue;
                }

                //-------------------------
                if ("Cnumber".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("CNumber", i);
                    continue;
                }

                if ("Invoice Quantity|From Quantity".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("InvoiceQuantity", i);
                    continue;
                }

                if ("Received Quantity|To Quantity".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("ReceivedQuantity", i);
                    continue;
                }

                if ("Currency".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Currency", i);
                    continue;
                }

                if ("Comment".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Comment", i);
                    continue;
                }

                if ("PreviousInvoiceNumber".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("PreviousInvoiceNumber", i);
                    continue;
                }

                if ("EffectiveDate|Effective Date".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("EffectiveDate", i);
                    continue;
                }

                if ("Supplier|Supplier Code".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("SupplierCode", i);
                    continue;
                }

                if ("SupplierName|Supplier Name".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("SupplierName", i);
                    continue;
                }

                if ("SupplierAddress|Supplier Address".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("SupplierAddress", i);
                    continue;
                }

                if ("CountryCode(2)|Country Code|CountryCode".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("CountryCode", i);
                    continue;
                }

                if ("Invoice Total|InvoiceTotal".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("InvoiceTotal", i);
                    continue;
                }

                if ("Document Type|DocumentType".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("DocumentType", i);
                    continue;
                }

                if ("Supplier Invoice#|SupplierInvoice#".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("SupplierInvoiceNo", i);
                    continue;
                }

                if ("Inventory Source|InventorySource".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("InventorySource", i);
                    continue;
                }

                if ("Packages|Package".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("Packages", i);
                    continue;
                }

                if ("Warehouse|WarehouseNo".ToUpper().Split('|').Any(x => x == h.ToUpper()))
                {
                    mapping.Add("WarehouseNo", i);
                    continue;
                }

            }
        }


        private List<CSVDataSummary> GetCSVDataSummayList(string[] lines, Dictionary<string, int> mapping,
            string[] headings)
        {
            int i = 0;
            try
            {
                var eslst = new List<CSVDataSummary>();

                for (i = 1; i < lines.Count(); i++)
                {

                    var d = GetCSVDataFromLine(lines[i], mapping, headings);
                    if (d != null)
                    {
                        d.LineNumber = i;
                        eslst.Add(d);
                    }
                }

                return eslst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }



        private CSVDataSummary GetCSVDataFromLine(string line, Dictionary<string, int> map, string[] headings)
        {
            var ImportChecks =
                new Dictionary<string, Func<CSVDataSummary, Dictionary<string, int>, string[], Tuple<bool, string>>>()
                {
                    {
                        "EntryDataId",
                        (c, mapping, splits) =>
                            new Tuple<bool, string>(Regex.IsMatch(splits[mapping["EntryDataId"]], @"\d+E\+\d+"),
                                "Invoice # contains Excel E+ Error")
                    },
                    {
                        "ItemNumber",
                        (c, mapping, splits) =>
                            new Tuple<bool, string>(Regex.IsMatch(splits[mapping["ItemNumber"]], @"\d+E\+\d+"),
                                "ItemNumber contains Excel E+ Error")
                    },

                };

            var ImportActions = new Dictionary<string, Action<CSVDataSummary, Dictionary<string, int>, string[]>>()
            {
                {"EntryDataId", (c, mapping, splits) => c.EntryDataId = splits[mapping["EntryDataId"]].Trim().Replace("PO/GD/","")},
                {
                    "EntryDataDate",
                    (c, mapping, splits) =>
                    {
                        DateTime date = DateTime.MinValue;
                        var strDate = string.IsNullOrEmpty(splits[mapping["EntryDataDate"]])
                                ? DateTime.MinValue.ToShortDateString()
                                : splits[mapping["EntryDataDate"]].Replace("�", "");
                        if(DateTime.TryParse(strDate, out date) == false)
                            DateTime.TryParseExact(strDate, "dd'/'MM'/'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out date);
                        c.EntryDataDate = date;
                    }
                },
                {"ItemNumber", (c, mapping, splits) => c.ItemNumber = splits[mapping["ItemNumber"]]},
                {
                    "ItemAlias",
                    (c, mapping, splits) =>
                        c.ItemAlias = mapping.ContainsKey("ItemAlias") ? splits[mapping["ItemAlias"]] : ""
                },
                {"ItemDescription", (c, mapping, splits) => c.ItemDescription = splits[mapping["ItemDescription"]]},
                {
                    "Cost",
                    (c, mapping, splits) => c.Cost = !mapping.ContainsKey("Cost")
                        ? 0
                        : Convert.ToSingle(string.IsNullOrEmpty(splits[mapping["Cost"]])
                            ? "0"
                            : splits[mapping["Cost"]].Replace("$", "").Replace("�", "").Replace("USD", "").Trim())
                },
                {
                    "Quantity",
                    (c, mapping, splits) => c.Quantity = Convert.ToSingle(splits[mapping["Quantity"]].Replace("�", ""))
                },
                {
                    "Units",
                    (c, mapping, splits) => c.Units = mapping.ContainsKey("Units") ? splits[mapping["Units"]] : ""
                },
                {
                    "CustomerName",
                    (c, mapping, splits) => c.CustomerName =
                        mapping.ContainsKey("CustomerName") ? splits[mapping["CustomerName"]] : ""
                },
                {
                    "Tax",
                    (c, mapping, splits) =>
                        c.Tax = Convert.ToSingle(mapping.ContainsKey("Tax") ? splits[mapping["Tax"]] : "0")
                },
                {
                    "TariffCode",
                    (c, mapping, splits) =>
                        c.TariffCode = mapping.ContainsKey("TariffCode") ? splits[mapping["TariffCode"]] : ""
                },
                {
                    "SupplierCode",
                    (c, mapping, splits) => c.SupplierCode =
                        mapping.ContainsKey("SupplierCode") ? splits[mapping["SupplierCode"]] : ""
                },
                {
                    "Freight",
                    (c, mapping, splits) =>
                        c.Freight = Convert.ToSingle(
                            mapping.ContainsKey("Freight") && !string.IsNullOrEmpty(splits[mapping["Freight"]])
                                ? splits[mapping["Freight"]]
                                : "0")
                },
                {
                    "Weight",
                    (c, mapping, splits) =>
                        c.Weight = Convert.ToSingle(
                            mapping.ContainsKey("Weight") && !string.IsNullOrEmpty(splits[mapping["Weight"]])
                                ? splits[mapping["Weight"]]
                                : "0")
                },
                {
                    "InternalFreight",
                    (c, mapping, splits) => c.InternalFreight = Convert.ToSingle(
                        mapping.ContainsKey("InternalFreight") &&
                        !string.IsNullOrEmpty(splits[mapping["InternalFreight"]])
                            ? splits[mapping["InternalFreight"]]
                            : "0")
                },
                {
                    "TotalFreight",
                    (c, mapping, splits) => c.TotalFreight = Convert.ToSingle(
                        mapping.ContainsKey("TotalFreight") && !string.IsNullOrEmpty(splits[mapping["TotalFreight"]])
                            ? splits[mapping["TotalFreight"]]
                            : "0")
                },
                {
                    "TotalWeight",
                    (c, mapping, splits) => c.TotalWeight = Convert.ToSingle(
                        mapping.ContainsKey("TotalWeight") && !string.IsNullOrEmpty(splits[mapping["TotalWeight"]])
                            ? splits[mapping["TotalWeight"]]
                            : "0")
                },
                {
                    "TotalInternalFreight",
                    (c, mapping, splits) => c.TotalInternalFreight = Convert.ToSingle(
                        mapping.ContainsKey("TotalInternalFreight") &&
                        !string.IsNullOrEmpty(splits[mapping["TotalInternalFreight"]])
                            ? splits[mapping["TotalInternalFreight"]]
                            : "0")
                },
                {
                    "TotalOtherCost",
                    (c, mapping, splits) => c.TotalOtherCost = Convert.ToSingle(
                        mapping.ContainsKey("TotalOtherCost") &&
                        !string.IsNullOrEmpty(splits[mapping["TotalOtherCost"]])
                            ? splits[mapping["TotalOtherCost"]]
                            : "0")
                },
                {
                    "TotalInsurance",
                    (c, mapping, splits) => c.TotalInsurance = Convert.ToSingle(
                        mapping.ContainsKey("TotalInsurance") &&
                        !string.IsNullOrEmpty(splits[mapping["TotalInsurance"]])
                            ? splits[mapping["TotalInsurance"]]
                            : "0")
                },
                {
                    "TotalDeductions",
                    (c, mapping, splits) => c.TotalDeductions = Convert.ToSingle(
                        mapping.ContainsKey("TotalDeductions") &&
                        !string.IsNullOrEmpty(splits[mapping["TotalDeductions"]])
                            ? splits[mapping["TotalDeductions"]]
                            : "0")
                },
                {
                    "CNumber",
                    (c, mapping, splits) => c.CNumber = mapping.ContainsKey("CNumber") ? splits[mapping["CNumber"]] : ""
                },
                {
                    "InvoiceQuantity",
                    (c, mapping, splits) => c.InvoiceQuantity = mapping.ContainsKey("InvoiceQuantity")
                        ? Convert.ToSingle(splits[mapping["InvoiceQuantity"]])
                        : 0
                },
                {
                    "ReceivedQuantity",
                    (c, mapping, splits) => c.ReceivedQuantity =
                        mapping.ContainsKey("ReceivedQuantity") && splits.Length >= mapping["ReceivedQuantity"]
                            ? Convert.ToSingle(splits[mapping["ReceivedQuantity"]])
                            : 0
                },
                {
                    "Currency",
                    (c, mapping, splits) =>
                        c.Currency = mapping.ContainsKey("Currency") ? splits[mapping["Currency"]] : ""
                },
                {
                    "Comment",
                    (c, mapping, splits) => c.Comment = mapping.ContainsKey("Comment") ? splits[mapping["Comment"]] : ""
                },
                {
                    "PreviousInvoiceNumber",
                    (c, mapping, splits) => c.PreviousInvoiceNumber = mapping.ContainsKey("PreviousInvoiceNumber")
                        ? splits[mapping["PreviousInvoiceNumber"]]
                        : ""
                },
                {
                    "EffectiveDate",
                    (c, mapping, splits) => c.EffectiveDate =
                        mapping.ContainsKey("EffectiveDate") && !string.IsNullOrEmpty(splits[mapping["EffectiveDate"]])
                            ? DateTime.Parse(splits[mapping["EffectiveDate"]], CultureInfo.CurrentCulture)
                            : (DateTime?) null
                },
                {
                    "TotalCost",
                    (c, mapping, splits) => c.Cost = !string.IsNullOrEmpty(splits[mapping["TotalCost"]])
                        ? Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) / c.Quantity
                        : c.Cost
                },
                {
                    "InvoiceTotal",
                    (c, mapping, splits) => c.InvoiceTotal = Convert.ToSingle(
                        mapping.ContainsKey("InvoiceTotal") && !string.IsNullOrEmpty(splits[mapping["InvoiceTotal"]])
                            ? splits[mapping["InvoiceTotal"]]
                            : "0")
                },
                {
                    "SupplierName",
                    (c, mapping, splits) => c.SupplierName =
                        mapping.ContainsKey("SupplierName") ? splits[mapping["SupplierName"]] : ""
                },
                {
                    "SupplierAddress",
                    (c, mapping, splits) => c.SupplierAddress =
                        mapping.ContainsKey("SupplierAddress") ? splits[mapping["SupplierAddress"]] : ""
                },
                {
                    "CountryCode",
                    (c, mapping, splits) => c.SupplierCountryCode =
                        mapping.ContainsKey("CountryCode") ? splits[mapping["CountryCode"]] : ""
                },
                {
                    "DocumentType",
                    (c, mapping, splits) => c.DocumentType =
                        mapping.ContainsKey("DocumentType") ? splits[mapping["DocumentType"]] : ""
                },
                {
                    "SupplierInvoiceNo",
                    (c, mapping, splits) => c.SupplierInvoiceNo = mapping.ContainsKey("SupplierInvoiceNo")
                        ? splits[mapping["SupplierInvoiceNo"]]
                        : ""
                },
                {
                    "InventorySource",
                    (c, mapping, splits) => c.SupplierInvoiceNo = mapping.ContainsKey("InventorySource")
                        ? splits[mapping["InventorySource"]]
                        : ""
                },
                {
                    "Packages",
                    (c, mapping, splits) => c.Packages = Convert.ToInt32(mapping.ContainsKey("Packages") && !string.IsNullOrEmpty(splits[mapping["Packages"]])
                        ? splits[mapping["Packages"]]
                        : "0")
                },
                {
                    "WarehouseNo",
                    (c, mapping, splits) => c.WarehouseNo = mapping.ContainsKey("WarehouseNo")
                        ? splits[mapping["WarehouseNo"]]
                        : ""
                },

            };

            try
            {
                if (string.IsNullOrEmpty(line)) return null;
                var splits = line.CsvSplit().Select(x => x.Trim()).ToArray();
                if (splits.Length < headings.Length) return null;
                if (!map.Keys.Contains("EntryDataId"))
                    throw new ApplicationException("Invoice# not Mapped");
                if (!map.Keys.Contains("ItemNumber"))
                    throw new ApplicationException("ItemNumber not Mapped");

                if (splits[map["EntryDataId"]] != "" && splits[map["ItemNumber"]] != "")
                {
                    var res = new CSVDataSummary();
                    res.SourceRow = line;
                    foreach (var key in map.Keys)
                    {
                        try
                        {
                            if (ImportChecks.ContainsKey(key))
                            {
                                var err = ImportChecks[key].Invoke(res, map, splits);
                                if (err.Item1) throw new ApplicationException(err.Item2);
                            }


                            ImportActions[key].Invoke(res, map, splits);
                        }
                        catch (Exception e)
                        {
                            var message =
                                $"Could not Import '{headings[map[key]]}' from Line:'{line}'. Error:{e.Message}";
                            Console.WriteLine(e);
                            throw new ApplicationException(message);
                        }

                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public class CSVDataSummary
        {
            public string PONumber;

            public string EntryDataId { get; set; }
            public DateTime? EntryDataDate { get; set; }
            public string ItemNumber { get; set; }
            public string ItemAlias { get; set; }
            public string ItemDescription { get; set; }
            public double? Quantity { get; set; }
            public double? Cost { get; set; }
            public string Units { get; set; }
            public string CustomerName { get; set; }
            public double? Tax { get; set; }
            public string TariffCode { get; set; }

            public string SupplierCode { get; set; }

            public double? Freight { get; set; }

            public double? Weight { get; set; }

            public double? InternalFreight { get; set; }
            public double? Insurance { get; set; }
            public double? OtherCost { get; set; }
            public double? Deductions { get; set; }
            public double? TotalFreight { get; set; }

            public double? TotalWeight { get; set; }
            public double? TotalInsurance { get; set; }
            public double? TotalOtherCost { get; set; }
            public double? TotalDeductions { get; set; }

            public double? TotalInternalFreight { get; set; }
            public string CNumber { get; set; }
            public double? InvoiceQuantity { get; set; }
            public double? ReceivedQuantity { get; set; }
            public string Currency { get; set; }
            public string Comment { get; set; }

            public string PreviousInvoiceNumber { get; set; }
            public DateTime? EffectiveDate { get; set; }

            public string SupplierName { get; set; }
            public string SupplierAddress { get; set; }
            public string SupplierCountryCode { get; set; }
            public double? InvoiceTotal { get; set; }
            public string DocumentType { get; set; }
            public string SupplierInvoiceNo { get; set; }
            public double? TotalCost { get; set; }
            public int? LineNumber { get; set; }
            public int InventoryItemId { get; set; }
            public string SourceRow { get; set; }

            public string Instructions { get; set; }
            public string InventorySource { get; set; }
            public int Packages { get; set; }

            public string WarehouseNo { get; set; }
        }

        private async Task ImportSuppliers(List<CSVDataSummary> eslst, int applicationSettingsId, string fileType)
        {
            try
            {


                var itmlst = eslst
                    .GroupBy(x => new {x.SupplierCode, x.SupplierName, x.SupplierAddress, x.SupplierCountryCode})
                    .ToList();

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true && fileType == "PO")
                {
                    if (itmlst.All(x => string.IsNullOrEmpty(x.Key?.SupplierCode ?? x.Key?.SupplierName)))
                    {
                        throw new ApplicationException(
                            $"Supplier Code/Name Missing for :{itmlst.Where(x => string.IsNullOrEmpty(x.Key?.SupplierCode ?? x.Key?.SupplierName)).Select(x => x.FirstOrDefault()?.EntryDataId).Aggregate((current, next) => current + ", " + next)}");
                    }
                }


                using (var ctx = new SuppliersService() {StartTracking = true})
                {
                    foreach (var item in itmlst.Where(x =>
                        !string.IsNullOrEmpty(x.Key?.SupplierCode) || !string.IsNullOrEmpty(x.Key?.SupplierAddress)))
                    {

                        var i = (await ctx.GetSuppliersByExpression(
                            $"{(item.Key.SupplierCode == null ? "SupplierName == \"" + item.Key.SupplierName.ToUpper() + "\"" : "SupplierCode == \"" + item.Key.SupplierCode.ToUpper() + "\"")} && ApplicationSettingsId == \"{applicationSettingsId}\"",
                            null, true).ConfigureAwait(false)).FirstOrDefault();
                        if (i != null) continue;
                        i = new Suppliers(true)
                        {
                            ApplicationSettingsId = applicationSettingsId,
                            SupplierCode = item.Key.SupplierCode?.ToUpper() ?? item.Key.SupplierName.ToUpper(),
                            SupplierName = item.Key.SupplierName,
                            Street = item.Key.SupplierAddress,
                            CountryCode = item.Key.SupplierCountryCode,

                            TrackingState = TrackingState.Added
                        };

                        await ctx.CreateSuppliers(i).ConfigureAwait(false);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task ImportInventory(List<CSVDataSummary> eslst, int applicationSettingsId, string fileType)
        {
            try
            {


                var itmlst = eslst.Where(x => x.ItemNumber != null)
                            .GroupBy(g => new { ItemNumber = g.ItemNumber.ToUpper(), g.ItemDescription, g.TariffCode})
                            .ToList();
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType)
                {
                    case "INV":

                        inventorySource = dctx.InventorySources.First(x => x.Name == "Supplier");
                        break;
                    case "PO":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "ADJ":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "Sales":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "DIS":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    default:
                        throw new ApplicationException("Unknown CSV FileType");

                }
            }

            using (var ctx = new InventoryDSContext() {StartTracking = true})
            {
                var inventoryItems = ctx.InventoryItems.Include("InventoryItemSources.InventorySource")
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId).ToList();
                foreach (var item in itmlst)
                {

                    
                    var i = inventoryItems.Any(x => x.ItemNumber == item.Key.ItemNumber &&  x.InventoryItemSources.FirstOrDefault()?.InventorySource == inventorySource)
                        ? inventoryItems.FirstOrDefault(x => x.ItemNumber == item.Key.ItemNumber && x.InventoryItemSources.FirstOrDefault()?.InventorySource == inventorySource)
                        : inventoryItems.FirstOrDefault(x => x.ItemNumber == item.Key.ItemNumber);

                    if (i == null || i.InventoryItemSources.FirstOrDefault()?.InventorySource != inventorySource)
                    {
                        i = new InventoryItem(true)
                        {
                            ApplicationSettingsId = applicationSettingsId,
                            Description = item.Key.ItemDescription,
                            ItemNumber = item.Key.ItemNumber.Truncate(20),
                            InventoryItemSources = new List<InventoryItemSource>(){ new InventoryItemSource(true)
                            {
                                InventorySourceId = inventorySource.Id,
                                TrackingState = TrackingState.Added
                            }},
                            TrackingState = TrackingState.Added
                        };
                        if (!string.IsNullOrEmpty(item.Key.TariffCode)) i.TariffCode = item.Key.TariffCode;



                        i = ctx.InventoryItems.Add(i);
                        ctx.SaveChanges();

                        }
                    else
                    {
                        if (i.Description != item.Key.ItemDescription || i.TariffCode != item.Key.TariffCode)
                        {
                            i.StartTracking();
                            i.Description = item.Key.ItemDescription;
                            if (!string.IsNullOrEmpty(item.Key.TariffCode)) i.TariffCode = item.Key.TariffCode;
                           // await ctx.UpdateInventoryItem(i).ConfigureAwait(false);
                            
                        }

                    }

                        foreach (var line in item)
                        {
                            line.InventoryItemId = i.Id;
                        }
                }

                ctx.SaveChanges();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


private async Task<OpeningStock> CreateOpeningStock(OpeningStock EDops)
        {
            using (var ctx = new OpeningStockService())
            {
                return await ctx.CreateOpeningStock(EDops).ConfigureAwait(false);
            }
        }
        private async Task<PurchaseOrders> CreatePurchaseOrders(PurchaseOrders EDpo)
        {
            using (var ctx = new PurchaseOrdersService())
            {
                return await ctx.CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            }
        }

        private async Task<Invoices> CreateInvoice(Invoices EDinv)
        {
            using (var ctx = new InvoicesService())
            {
                return await ctx.CreateInvoices(EDinv).ConfigureAwait(false);
            }
        }

        private async Task<Sales> CreateSales(Sales EDsale)
        {
            using (var ctx = new SalesService())
            {
                return await ctx.CreateSales(EDsale).ConfigureAwait(false);
            }
        }

        private async Task DeleteEntryData(EntryData olded)
        {
            using (var ctx = new EntryDataService())
            {
                await ctx.DeleteEntryData(olded.EntryData_Id.ToString()).ConfigureAwait(false);
            }
        }

        private async Task UpdateEntryData(EntryData olded)
        {
            using (var ctx = new EntryDataService())
            {
                await ctx.UpdateEntryData(olded).ConfigureAwait(false);
            }
        }

        private async Task ClearEntryDataDetails(EntryData olded)
        {
            using (var ctx = new EntryDataDetailsService())
            {
                foreach (var itm in olded.EntryDataDetails.ToList())
                {
                    await ctx.DeleteEntryDataDetails(itm.EntryDataDetailsId.ToString()).ConfigureAwait(false);
                }
            }
        }
    }
}