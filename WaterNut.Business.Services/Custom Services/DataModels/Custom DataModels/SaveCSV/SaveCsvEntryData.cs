using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common.CSV;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using ExcelDataReader;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using MoreLinq;
using Org.BouncyCastle.Utilities;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using InventoryItemSource = InventoryDS.Business.Entities.InventoryItemSource;

namespace WaterNut.DataSpace
{
    public class SaveCsvEntryData
    {
        private const double lb2Kg = 0.453592;
        private static readonly SaveCsvEntryData instance;

        static SaveCsvEntryData()
        {
            instance = new SaveCsvEntryData();
        }

        public static SaveCsvEntryData Instance
        {
            get { return instance; }
        }

        public async Task<bool> ExtractEntryData(FileTypes fileType, string[] lines, string[] headings, 
            List<AsycudaDocumentSet> docSet, bool overWriteExisting, int emailId, 
            string droppedFilePath)
        {
            try
            {


                if (docSet == null)
                {
                    throw new ApplicationException("Please select Document Set before proceding!");

                }
                var mappingFileType = GetHeadingFileType(headings);
                if (mappingFileType != null && mappingFileType.Id != fileType.Id && mappingFileType.Id != fileType.ParentFileTypeId) fileType = mappingFileType;
                if (!fileType.FileTypeMappings.Any()) fileType = BaseDataModel.GetFileType(fileType);
                var mapping = new Dictionary<FileTypeMappings, int>();
                GetMappings(mapping, headings, fileType);

                if (fileType.Type == "Sales" && !mapping.Keys.Any(x => x.DestinationName == "Tax" || x.DestinationName == "TotalTax"))
                    throw new ApplicationException("Sales file dose not contain Tax");

                if (!mapping.Any()) throw new ApplicationException($"No Mapping for FileType: {fileType.Id}");
                


                var eslst = GetCSVDataSummayList(lines, mapping, headings);

                if (eslst == null) return true;
                

                return await ProcessCsvSummaryData(fileType, docSet, overWriteExisting, emailId, 
                    droppedFilePath, eslst).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nex = new ApplicationException($"Error Importing File: {droppedFilePath} - {e.Message}", e);
                Console.WriteLine(nex);
                throw nex;
            }
        }

        private FileTypes GetHeadingFileType(string[] heading)
        {
           
            using (var ctx = new CoreEntitiesContext())
            {
                var resLst =
                    ctx.FileTypes.Include(x => x.FileTypeMappings).Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.FileTypeMappings.Any())
                        .SelectMany(x => x.FileTypeMappings.Where(z => heading.Select(h => h.ToUpper().Trim()).Contains(z.OriginalName.ToUpper().Trim())))
                        .GroupBy(x => x.FileTypes)
                        .OrderByDescending(x => x.Count())
                        .Where(x => x.Key.IsImportable == null || x.Key.IsImportable == true) 
                        .ToList();

                 var res = resLst.FirstOrDefault();
                return res != null ? BaseDataModel.GetFileType(res.Key) : null;
            }

           
        }

        private void ProcessCsvRider(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            int? emailId,  string droppedFilePath, List<dynamic> eslst)
        {
            try
            {
                const double kg = lb2Kg;

                var csvRiders = eslst.Select(x => ((IDictionary<string, object>) x))
                    .GroupBy(x => new
                    {
                        ETA = x[nameof(ShipmentRider.ETA)],
                        Date = x["Date"],
                        Code = x[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim()
                    }).ToList();



                var rawRiders = csvRiders
                    .Select(x => new{
                        ETA = x.Key.ETA,
                        DocumentDate = x.Key.Date,
                        ShipmentRiderDetails = x.Select(z => new 
                        {
                            Consignee = z[nameof(ShipmentRiderDetails.Consignee)]?.ToString().Trim()?? z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Code = z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Shipper = z[nameof(ShipmentRiderDetails.Shipper)]?.ToString().Trim(),
                            TrackingNumber = z[nameof(ShipmentRiderDetails.TrackingNumber)]?.ToString().Trim(),
                            Pieces = int.TryParse(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim(),out var test)? test : 0,//Convert.ToInt32(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim())
                            WarehouseCode = z[nameof(ShipmentRiderDetails.WarehouseCode)]?.ToString().Trim(),
                            InvoiceNumber = z[nameof(ShipmentRiderDetails.InvoiceNumber)]?.ToString().Trim().ReplaceRegex(@"[^0-9a-zA-Z\s\-/\,]+",""),
                            InvoiceTotal = z.ContainsKey(nameof(ShipmentRiderDetails.InvoiceTotal)) ? z[nameof(ShipmentRiderDetails.InvoiceTotal)]?.ToString().Trim() : "0",
                            GrossWeightLB = Convert.ToDouble(z["GrossWeightLB"]?.ToString().Trim()),
                            CubicFeet = z.ContainsKey("VolumeCF") ? Convert.ToDouble(z["VolumeCF"]?.ToString().Trim()) : 0.0
                            

                        }).OrderByDescending(z => z.Pieces).ToList()

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var rawRider in rawRiders.Where(x => x.ETA != null))
                    {
                        DateTime eta = (DateTime) rawRider.ETA;
                        var existingRider = ctx.ShipmentRider.Where(x => x.ETA == eta).ToList();
                        if (existingRider.Any()) ctx.ShipmentRider.RemoveRange(existingRider);
                        
                        var invoiceLst = rawRider.ShipmentRiderDetails.Select(x => new
                            {
                                WarehouseCode = x.WarehouseCode,
                                Totalpkgs = x.Pieces,
                                totalKgs = x.GrossWeightLB * kg,
                                totalCF = x.CubicFeet,
                                Number = (x.InvoiceNumber??"").Contains(',') ? x.InvoiceNumber.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray() : (x.InvoiceNumber??"").Split('/').Where(z => !string.IsNullOrEmpty(z)).ToArray(),
                                Total =  x.InvoiceTotal.Contains(',') ? x.InvoiceTotal.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray() : x.InvoiceTotal.Split('/').Where(z => !string.IsNullOrEmpty(z)).ToArray(),
                                
                        })
                            .ToList();
                        var riderDetails = new List<ShipmentRiderDetails>();
                        foreach (var i in invoiceLst)
                        {
                            var totalst = i.Total.Select(x => Convert.ToDouble(string.IsNullOrEmpty(x)?"1":x.Replace("$","").Trim())).ToList();
                            var totalSum = totalst.Sum();
                            var usedPieces = 0;
                            var usedKgs = 0.0;
                            var usedCF = 0.0;
                            for (int j = 0; j < i.Number.Length; j++)
                            {
                                
                                var rate =  (totalSum == 0 || j >= totalst.Count || totalst[j] == 0) ? (1 / Convert.ToDouble(i.Number.Length) ) : totalst[j] / totalSum;
                                var pkgs = j == i.Number.Length - 1
                                    ? i.Totalpkgs - usedPieces
                                    : Convert.ToInt32(i.Totalpkgs * rate);
                                if (pkgs == 0) pkgs = 1;
                                var kgs = j == i.Number.Length - 1
                                    ? i.totalKgs - usedKgs
                                    : Convert.ToDouble(i.totalKgs * rate);
                                var cf = j == i.Number.Length - 1
                                    ? i.totalCF - usedCF
                                    : Convert.ToDouble(i.totalCF * rate);

                                List<ShipmentRiderDetails> res = new List<ShipmentRiderDetails>();
                                if (usedPieces >= i.Totalpkgs)
                                {
                                    var riderDetail = riderDetails.FirstOrDefault(x => x.Pieces > 1);
                                    if (riderDetail != null)
                                    {
                                        riderDetail.Pieces -= 1;
                                        res.Add(new ShipmentRiderDetails()
                                        {
                                            Consignee = riderDetail.Consignee,
                                            Code = riderDetail.Code,
                                            Shipper = riderDetail.Shipper,
                                            TrackingNumber = riderDetail.TrackingNumber,
                                            Pieces = 1,
                                            WarehouseCode = i.WarehouseCode.Trim(),//riderDetail.WarehouseCode,
                                            InvoiceNumber = i.Number[j].Trim(),
                                            InvoiceTotal = j >= totalst.Count 
                                            ? 0 : totalst[j],
                                            GrossWeightKg = kgs,
                                            CubicFeet = cf,
                                            TrackingState = TrackingState.Added

                                        });
                                    }
                                    else
                                    {
                                        res = rawRider.ShipmentRiderDetails
                                            .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                                        (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                                z =>
                                                    new ShipmentRiderDetails()
                                                    {
                                                        Consignee = z.Consignee,
                                                        Code = z.Code,
                                                        Shipper = z.Shipper,
                                                        TrackingNumber = z.TrackingNumber,
                                                        Pieces = 0,//pkgs,
                                                        WarehouseCode = z.WarehouseCode.Trim(),
                                                        InvoiceNumber = i.Number[j].Trim(),
                                                        InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                                        GrossWeightKg = kgs,
                                                        CubicFeet = cf,
                                                        TrackingState = TrackingState.Added

                                                    }).ToList();

                                    }

                                }
                                else
                                {
                                   
                                    res = rawRider.ShipmentRiderDetails
                                        .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                                    (x.InvoiceNumber??"").Contains(i.Number[j])).Select(
                                            z =>
                                                new ShipmentRiderDetails()
                                                {
                                                    Consignee = z.Consignee,
                                                    Code = z.Code,
                                                    Shipper = z.Shipper,
                                                    TrackingNumber = z.TrackingNumber,
                                                    Pieces = pkgs,
                                                    WarehouseCode = z.WarehouseCode.Trim(),
                                                    InvoiceNumber = i.Number[j].Trim(),
                                                    InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                                    GrossWeightKg = kgs,
                                                    CubicFeet = cf,
                                                    TrackingState = TrackingState.Added

                                                }).ToList();
                                }


                                usedCF += cf;
                                usedKgs += kgs;
                                usedPieces += pkgs;
                                riderDetails.AddRange(res);
                                if (totalst.Sum() != 0 && i.Number.Length != i.Total.Length) break;
                            }

                        }

                        

                        ctx.ShipmentRider.Add(new ShipmentRider()
                        {
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            ETA = (DateTime)rawRider.ETA,
                            DocumentDate = (DateTime)rawRider.DocumentDate,
                            FileTypeId =fileType.Id,
                            EmailId = emailId.GetValueOrDefault(),
                            SourceFile = droppedFilePath,
                            TrackingState = TrackingState.Added,
                            ShipmentRiderDetails = riderDetails
                        });
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

        private void ProcessCsvExpiredEntries(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, int? emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.ExecuteSqlCommand("delete from ExpiredEntriesLst");
                    foreach (var itm in eslst)
                    {
                        var expireditm = new ExpiredEntriesLst(true)
                        {
                            Office = itm.Office,
                            GeneralProcedure = itm.GeneralProcedure,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            AssessmentDate = itm.AssessmentDate,
                            AssessmentNumber = itm.AssessmentNumber,
                            AssessmentSerial = itm.AssessmentSerial,
                            RegistrationDate = itm.RegistrationDate,
                            RegistrationNumber = itm.RegistrationNumber,
                            RegistrationSerial = itm.RegistrationSerial,
                            Consignee = itm.Consignee,
                            Exporter = itm.Exporter,
                            DeclarantCode = itm.DeclarantCode,
                            DeclarantReference = itm.DeclarantReference,
                            Expiration = itm.Expiration,
                            TrackingState = TrackingState.Added
                        };
                        ctx.ExpiredEntriesLst.Add(expireditm);
                    }

                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand($@"UPDATE xcuda_ASYCUDA_ExtendedProperties
                                                    SET         EffectiveExpiryDate = exp.Expiration
                                                    FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code, 
                                                                                      CAST(ExpiredEntriesLst.Expiration AS datetime) AS Expiration
                                                                     FROM     ExpiredEntriesLst INNER JOIN
                                                                                      AsycudaDocument ON ExpiredEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND ExpiredEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                                                                      ExpiredEntriesLst.RegistrationNumber = AsycudaDocument.CNumber AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber) AS exp INNER JOIN
                                                                     xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        

        public async Task<bool> ProcessCsvSummaryData(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, int emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {
               
                //var instructionslst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList())
                //    .Where(x => x.ContainsKey("Instructions"))
                //                            .Select(x =>x["Instructions"].ToString()).ToList();
                //List<string> instructions = new List<string>();
                //if (instructionslst.Any())
                //{
                //    instructions = instructionslst.SelectMany(x => x.Split(',')).Where(x => !string.IsNullOrEmpty(x)).ToList();
                //}

                //if (instructions.Contains("Append")) overWriteExisting = false;
                //if (instructions.Contains("Replace")) overWriteExisting = true;


                if (fileType.Type == "Rider")
                {
                    ProcessCsvRider(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.Type == "Manifest")
                {
                    ProcessManifest(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.Type == "BL")
                {
                    ProcessBL(fileType, docSet, overWriteExisting, emailId, 
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.Type == "Freight")
                {
                    ProcessFreight(fileType, docSet, overWriteExisting, emailId, 
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.Type == "Shipment Invoice")
                {
                    if(eslst.Any(x => ((List<IDictionary<string, object>>)x).Any(z => z.ContainsKey("InvoiceDetails"))))
                        await ImportInventory(eslst.SelectMany(x => ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x => ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(), docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                    ProcessShipmentInvoice(fileType, docSet, overWriteExisting, emailId, 
                        droppedFilePath, eslst);

                    
                    //await ImportInventory(eslst.SelectMany(x => ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).ToList(), docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                    return true;
                }

                if (fileType.Type == "ExpiredEntries")
                {
                    ProcessCsvExpiredEntries(fileType, docSet, overWriteExisting, emailId, 
                        droppedFilePath, eslst);
                    return true;
                }




                await ImportInventory(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                await ImportSuppliers(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                await ImportEntryDataFile(fileType, eslst.Where(x => !string.IsNullOrEmpty(x.SourceRow)).ToList(),
                    emailId, fileType.Id, droppedFilePath, docSet.First().ApplicationSettingsId).ConfigureAwait(false);
                if (await ImportEntryData(fileType, eslst, docSet, overWriteExisting, emailId,
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

        private void ProcessShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, int emailId, string droppedFilePath, List<object> eslst)
        {
            try
            {
                var itms = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).ToList();
                var lst = itms.Select(x => (IDictionary<string, object>)x)
                    .Select(x => new ShipmentInvoice()
                    {

                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        InvoiceNo = x.ContainsKey("InvoiceNo") ?  x["InvoiceNo"].ToString() : "Unknown",
                        InvoiceDate = x.ContainsKey("InvoiceDate") ?  DateTime.Parse(x["InvoiceDate"].ToString()) : DateTime.MinValue,
                        InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null,//Because of MPI not 
                        SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null,
                        ImportedLines = !x.ContainsKey("InvoiceDetails") ? 0 : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).Count,
                        SupplierCode = x.ContainsKey("SupplierCode") ? x["SupplierCode"].ToString() : null,
                        FileLineNumber = itms.IndexOf(x) + 1,
                        Currency = x.ContainsKey("Currency") ? x["Currency"].ToString() : null,
                        TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null,
                        TotalOtherCost = x.ContainsKey("TotalOtherCost")? Convert.ToDouble(x["TotalOtherCost"].ToString()): (double?) null,
                        TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null,
                        TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null,
                        InvoiceDetails = !x.ContainsKey("InvoiceDetails") ? new List<InvoiceDetails>() : ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                                        //        .Where(z => z["Quantity"] != null && Convert.ToDouble(z["Quantity"].ToString()) > 0 && (z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) : Convert.ToDouble(z["Cost"].ToString())) > 0)
                                              
                                                .Select(z => new InvoiceDetails()
                                                {
                                                    Quantity = Convert.ToDouble(z["Quantity"].ToString()),
                                                    ItemNumber = z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20): null,
                                                    ItemDescription = z["ItemDescription"].ToString().Truncate(255),
                                                    Units = z.ContainsKey("Units") ? z["Units"].ToString() : null,
                                                    Cost = Convert.ToDouble(z["Cost"].ToString()),
                                                    TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) : (double?)null,
                                                    Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0,
                                                    Volume = z.ContainsKey("Gallons") ? new InvoiceDetailsVolume() {Quantity = Convert.ToDouble(z["Gallons"].ToString()), Units = "Gallons", TrackingState = TrackingState.Added, } : null,
                                                    SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA") || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units")) ? Convert.ToInt32(z["SalesFactor"].ToString()) /* * (z.ContainsKey("Multiplier")  ? Convert.ToInt32(z["Multiplier"].ToString()) : 1) */ : 1,
                                                    LineNumber = ((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1,
                                                    FileLineNumber = Convert.ToInt32(z["FileLineNumber"].ToString()),
                                                    InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"]: (int?)null,
                                                    TrackingState = TrackingState.Added,

                                                }).ToList(),
                        InvoiceExtraInfo = !x.ContainsKey("ExtraInfo")? new List<InvoiceExtraInfo>(): ((List<IDictionary<string, object>>)x["ExtraInfo"])
                            .Where(z => z.Keys.Any())
                            .SelectMany(z => z)
                            .Select(z => new InvoiceExtraInfo()
                            {
                                Info = z.Key.ToString(),
                                Value = z.Value.ToString(),
                                TrackingState = TrackingState.Added,
                            }).ToList(),

                        EmailId = emailId,
                        SourceFile = droppedFilePath,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

               
                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var invoice in lst)
                    {
                       
                        var existingManifest =
                            ctx.ShipmentInvoice.FirstOrDefault(
                                x => x.InvoiceNo == invoice.InvoiceNo);
                        if (existingManifest != null)
                            ctx.ShipmentInvoice.Remove(existingManifest);

                        invoice.InvoiceDetails = AutoFixImportErrors(invoice);
                        invoice.ImportedLines = invoice.InvoiceDetails.Count;

                        if (Math.Abs(invoice.SubTotal.GetValueOrDefault()) < 0.01)
                        {
                            invoice.SubTotal = invoice.ImportedSubTotal;
                        }

                        if (!invoice.InvoiceDetails.Any())
                            throw new ApplicationException(
                                $"No Invoice Details");

                        if (invoice.ImportedTotalDifference > 0.001)
                            throw new ApplicationException(
                                $"Imported Total Difference for Invoice > 0: {invoice.ImportedTotalDifference}");

                        ctx.ShipmentInvoice.Add(invoice);

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

        private static List<InvoiceDetails> AutoFixImportErrors(ShipmentInvoice invoice)
        {
            var details = invoice.InvoiceDetails;

            foreach (var err in details.Where(x =>x.TotalCost.GetValueOrDefault() > 0 && Math.Abs(((x.Quantity * x.Cost) - x.Discount.GetValueOrDefault()) - x.TotalCost.GetValueOrDefault()) > 0.01))
            {
                
            }

            if (invoice.SubTotal > 0
                && Math.Abs((double) (invoice.SubTotal - details.Sum(x => x.Cost * x.Quantity))) > 0.01)
                details = details.DistinctBy(x => new {x.ItemNumber, x.Quantity, x.TotalCost})
                    .ToList();


            var invoiceInvoiceDetails = details ;
            return invoiceInvoiceDetails;
        }

        private void ProcessFreight(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, int emailId, string droppedFilePath, List<object> eslst)
        {
            try
            {
                
                var lst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>)x))
                    .Select(x => new ShipmentFreight()
                    {

                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        InvoiceNumber = x["InvoiceNumber"].ToString(),
                        Consignee = x["Consignee"].ToString(),
                        Currency = x.ContainsKey("Currency") ?  x["Currency"].ToString().Truncate(3): null,
                        InvoiceDate =(DateTime) x["ETA"],
                        DueDate = x.ContainsKey("DueDate") ? DateTime.Parse(x["DueDate"].ToString()) : DateTime.MinValue,
                        ETA = (DateTime)x["ETA"],
                        BLNumber = x.ContainsKey("BLNumber") ? x["BLNumber"].ToString(): null,
                        InvoiceTotal = Convert.ToDouble(x["InvoiceTotal"].ToString()),
                        ShipmentFreightDetails = ((List<IDictionary<string, object>>)x["ShipmentFreightDetails"])
                                                .Select(z => new ShipmentFreightDetails()
                                                {
                                                    Quantity = Convert.ToDouble(z["Quantity"].ToString()),
                                                    Description = z["Description"].ToString(),
                                                    WarehouseCode = z["WarehouseCode"]?.ToString(),
                                                    Rate = Convert.ToDouble(z["Rate"].ToString()),
                                                    Total = Convert.ToDouble(z["Amount"].ToString()),
                                                    TrackingState = TrackingState.Added,

                                                }).ToList(),

                        EmailId = emailId,
                       // SourceFile = filename,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var manifest in lst)
                    {
                        var filename = BaseDataModel.SetFilename(droppedFilePath, manifest.BLNumber, "-Freight.pdf");
                        manifest.SourceFile = filename;
                        var existingManifest =
                            ctx.ShipmentFreight.FirstOrDefault(
                                x => x.InvoiceNumber == manifest.InvoiceNumber);
                        if (existingManifest != null)
                            ctx.ShipmentFreight.Remove(existingManifest);
                        ctx.ShipmentFreight.Add(manifest);

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


        double CF2M3 = 0.0283168;
        private void ProcessBL(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, int emailId,  string droppedFilePath, List<object> eslst)
        {
            try
            {
                
                var lst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>)x))
                    .Select(x => new ShipmentBL()
                    {
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        //RegistrationNumber = x["RegistrationNumber"].ToString(),
                        //x.ContainsKey("TotalOtherCost")? Convert.ToDouble(x["TotalOtherCost"].ToString()): (double?) null,
                        Reference = x.ContainsKey("Reference") ? x["Reference"].ToString(): null,
                        Voyage = x.ContainsKey("Voyage") ? x["Voyage"].ToString(): null,
                        //ETD = DateTime.ParseExact(x["ETD"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        //ETA = DateTime.ParseExact(x["ETA"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Vessel = x.ContainsKey("Vessel") ? x["Vessel"].ToString(): null,
                        BLNumber = x["BLNumber"].ToString(),
                        //LineNumber = Convert.ToInt32(x["LineNumber"].ToString()),
                        Container = x.ContainsKey("Container") ? x["Container"].ToString(): null,
                        Seals = x.ContainsKey("Seals") ? x["Seals"].ToString(): null,
                        Type = x.ContainsKey("Type") ? x["Type"].ToString(): null,
                        //CargoReporter = x["CargoReporter"].ToString(),
                        //Exporter = x["Exporter"].ToString(),
                        //Consignee = x["Consignee"].ToString(),
                        //Notify = x["Notify"].ToString(),
                        PackagesNo = Convert.ToInt32(x["PackagesNo"].ToString()),
                        PackagesType = x.ContainsKey("PackagesType") ? x["PackagesType"].ToString(): "PK",
                        WeightKG = x.ContainsKey("WeightKG") ? Convert.ToDouble(x["WeightKG"].ToString()): x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString())* lb2Kg : 0,
                        VolumeM3 = x.ContainsKey("VolumeM3") ? Convert.ToDouble(x["VolumeM3"].ToString()) : x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString())* CF2M3 : 0,

                        WeightLB = x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString()): (double?) null,
                        VolumeCF = x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString()): (double?) null,
                        //Freight = Convert.ToDouble(x["Freight"].ToString()),
                        //LocationOfGoods = x["LocationOfGoods"].ToString(),
                        //Goods = x["Goods"].ToString(),
                        //Marks = x["Marks"].ToString(),
                        //Containers = Convert.ToInt32(x["Containers"].ToString()),
                        ShipmentBLDetails = ((List<IDictionary<string, object>>)x["ShipmentBLDetails"])
                                                .Select(z => new ShipmentBLDetails()
                                                                {
                                                                    Quantity = Convert.ToInt32(z["Quantity"].ToString()),
                                                                    Marks = z["Marks"].ToString(),
                                                                    PackageType = z["PackageType"].ToString(),
                                                                    TrackingState = TrackingState.Added,
                                                               
                                                            }).ToList(),

                        EmailId = emailId,
                       // SourceFile = droppedFilePath,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var manifest in lst)
                    {
                        var blDetails = manifest.ShipmentBLDetails
                            .DistinctBy(x => new {x.Marks, x.Quantity, x.PackageType}).ToList();

                        manifest.ShipmentBLDetails = blDetails;

                        var detailsQty = manifest.ShipmentBLDetails.Sum(x => x.Quantity);
                        if (manifest.PackagesNo != detailsQty)
                        {
                            throw new ApplicationException(
                                $"BL Details Quantity don't add up to BL Total Packages! - BL{manifest.PackagesNo} vs Details{detailsQty}");
                        }


                        var filename = BaseDataModel.SetFilename(droppedFilePath,manifest.BLNumber, "-BL.pdf");
                        if(!File.Exists(filename)) File.Copy(droppedFilePath, filename);
                        manifest.SourceFile = filename;
                        var existingManifest =
                            ctx.ShipmentBL.FirstOrDefault(
                                x => x.BLNumber == manifest.BLNumber);
                        if (existingManifest != null)
                            ctx.ShipmentBL.Remove(existingManifest);
                        ctx.ShipmentBL.Add(manifest);

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

        private void ProcessManifest(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, int emailId, string droppedFilePath, List<object> eslst)
        {
            try
            {
               

                var lst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>) x))
                    .Select(x => new ShipmentManifest()
                    {
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        RegistrationDate = DateTime.Parse(x["RegistrationDate"].ToString()),
                        RegistrationNumber = x["RegistrationNumber"].ToString(),
                        CustomsOffice = x["CustomsOffice"].ToString(),
                        Voyage = x["Voyage"].ToString(),
                        ETD = x.ContainsKey("ETD") ? DateTime.Parse(x["ETD"].ToString()): DateTime.MinValue,
                        ETA = DateTime.Parse(x["ETA"].ToString()),
                        Vessel = x["Vessel"].ToString(),
                        WayBill = x["WayBill"].ToString(),
                        LineNumber = Convert.ToInt32(x["LineNumber"].ToString()),
                        LoadingPort = x.ContainsKey("ETD") ? x["LoadingPort"].ToString():null,
                        ModeOfTransport = x["ModeOfTransport"].ToString(),
                        TypeOfBL = x["TypeOfBL"].ToString(),
                        //CargoReporter = x["CargoReporter"].ToString(),
                        //Exporter = x["Exporter"].ToString(),
                        //Consignee = x["Consignee"].ToString(),
                        //Notify = x["Notify"].ToString(),
                        Packages = Convert.ToInt32(x["Packages"].ToString()),
                        PackageType = x["PackageType"].ToString(),
                        GrossWeightKG = Convert.ToDouble(x["GrossWeightKG"].ToString()),
                        Volume = x.ContainsKey("Volume") ? Convert.ToDouble(x["Volume"].ToString()):0.0,
                        //Freight = Convert.ToDouble(x["Freight"].ToString()),
                        LocationOfGoods = x["LocationOfGoods"].ToString(),
                        Goods = x.ContainsKey("Goods")?x["Goods"].ToString():"",
                        Marks = x.ContainsKey("Marks") ? x["Marks"].ToString() : "",
                        //Containers = Convert.ToInt32(x["Containers"].ToString()),
                        EmailId = emailId,
                       // SourceFile = filename,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var manifest in lst)
                    {
                        var filename = BaseDataModel.SetFilename(droppedFilePath,manifest.WayBill, "-Manifest.pdf");
                        manifest.SourceFile = filename;
                        var existingManifest =
                            ctx.ShipmentManifest.FirstOrDefault(
                                x => x.RegistrationNumber == manifest.RegistrationNumber && x.WayBill == manifest.WayBill);
                        if (existingManifest != null)
                            ctx.ShipmentManifest.Remove(existingManifest);
                        ctx.ShipmentManifest.Add(manifest);

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

        private async Task ImportEntryDataFile(FileTypes fileType, List<dynamic> eslst, int? emailId,
            int? fileTypeId, string droppedFilePath, int applicationSettingsId)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    eslst.ForEach(x =>
                    {
                        if (!((IDictionary<string, object>)x).ContainsKey("InvoiceQuantity")) x.InvoiceQuantity = 0;
                        if (!((IDictionary<string, object>)x).ContainsKey("ReceivedQuantity")) x.ReceivedQuantity = 0;
                        if (!((IDictionary<string, object>)x).ContainsKey("Tax")) x.Tax = 0.0;
                        if (!((IDictionary<string, object>)x).ContainsKey("TotalCost")) x.TotalCost = 0.0;
                       
                    });

                    ctx.Database.ExecuteSqlCommand($@"delete from EntryDataFiles where SourceFile = '{droppedFilePath}'");
                    foreach (var line in eslst)
                    {
                        var drow = new EntryDataFiles(true)
                        {
                            TrackingState = TrackingState.Added,
                            SourceFile = droppedFilePath,
                            ApplicationSettingsId = applicationSettingsId,
                            SourceRow = line.SourceRow,
                            FileType = fileType.Type,
                            EmailId = emailId,
                            FileTypeId = fileTypeId,
                            EntryDataId = line.EntryDataId,
                            EntryDataDate = line.EntryDataDate,
                            Cost = line.Cost,
                            InvoiceQty = line.InvoiceQuantity,
                            ItemDescription = line.ItemDescription,
                            ItemNumber = ((string)Convert.ToString(line.ItemNumber)).Truncate(20),
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        private async Task<bool> ImportEntryData(FileTypes fileType, List<dynamic> eslst,
            List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, int? emailId,  string droppedFilePath)
        {
            try
            {
                var ndocSet = new List<AsycudaDocumentSet>();
                if (overWriteExisting == true) ndocSet = docSet;

               

                var ed = (from es in eslst.Select(x => (dynamic)x)
                    group es by new {es.EntryDataId, es.EntryDataDate, es.CustomerName}//, es.Currency
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
                                Tax = ((dynamic)g.FirstOrDefault())?.Tax,
                                
                                Supplier = string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                                    ? null
                                    : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                                Currency = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                                EmailId = emailId,
                                FileTypeId = fileType.Id,
                                DocumentType = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                                SupplierInvoiceNo = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                                PreviousCNumber = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                                FinancialInformation = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))?.FinancialInformation,

                                PONumber = ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                                SourceFile = droppedFilePath,
                                
                            },
                        EntryDataDetails = g.Where(x => !string.IsNullOrEmpty(x.ItemNumber)).Select(x =>
                            new EntryDataDetails()
                            {
                                EntryDataId = x.EntryDataId,
                                //Can't set entrydata_id here cuz this is from data
                                ItemNumber = x.ItemNumber.ToUpper(),
                                ItemDescription = x.ItemDescription,
                                Cost = x.Cost??0,
                                TotalCost = Convert.ToDouble(x.TotalCost ?? 0.0),
                                Quantity = Convert.ToDouble(x.Quantity ?? 0.0),
                                FileLineNumber = x.LineNumber,
                                Units = x.Units,
                                Freight = Convert.ToDouble(x.Freight ?? 0.0),
                                Weight = Convert.ToDouble(x.Weight ?? 0.0),
                                InternalFreight = Convert.ToDouble(x.InternalFreight ?? 0.0),
                                InvoiceQty = Convert.ToDouble(x.InvoiceQuantity ?? 0.0),
                                ReceivedQty = Convert.ToDouble(x.ReceivedQuantity ?? 0.0),
                                TaxAmount = x.Tax??0,
                                CNumber = x.PreviousCNumber,
                                PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                                Comment = x.Comment,
                                InventoryItemId = x.InventoryItemId,
                                EffectiveDate = x.EffectiveDate,
                                VolumeLiters = Convert.ToDouble(x.Gallons * GalToLtrRate ?? Convert.ToDouble(x.Liters ?? 0.0)),                                

                            }),
                        f = g.Select(x => new
                        {
                            TotalWeight = Convert.ToDouble(x.TotalWeight ?? 0.0),
                            TotalFreight = Convert.ToDouble(x.TotalFreight ?? 0.0),
                            TotalInternalFreight = Convert.ToDouble(x.TotalInternalFreight ?? 0.0),
                            TotalOtherCost = Convert.ToDouble(x.TotalOtherCost ?? 0.0),
                            TotalInsurance = Convert.ToDouble(x.TotalInsurance ?? 0.0),
                            TotalDeductions = Convert.ToDouble(x.TotalDeductions ?? 0.0),
                            InvoiceTotal = Convert.ToDouble(x.InvoiceTotal ?? 0.0),
                            TotalTax = Convert.ToDouble(x.TotalTax ?? 0.0),
                            Packages = Convert.ToInt32(x.Packages ?? 0),
                            WarehouseNo = x.WarehouseNo,




                        }),
                        InventoryItems = g.DistinctBy(x => new {x.ItemNumber, x.ItemAlias})
                            .Select(x => new {x.ItemNumber, x.ItemAlias})
                    }).ToList();

                if (ed == null) return true;


                List<EntryData> eLst = null;

                //Parallel.ForEach(ed, new ParallelOptions() { MaxDegreeOfParallelism = 3 },//Environment.ProcessorCount * 1
                //    async item =>
                    foreach (var item in ed.Where(x => x.EntryData.EntryDataId != null))

                    {
                        string entryDataId = item.EntryData.EntryDataId;
                        if (!item.EntryDataDetails.Any())
                            throw new ApplicationException(entryDataId + " has no details");

                        List<EntryDataDetails> details = new List<EntryDataDetails>();


                    // check Existing items
                    //var oldeds = await GetEntryData(item.EntryData.EntryDataId, docSet,
                    //    item.EntryData.ApplicationSettingsId).ConfigureAwait(false);

                        int applicationSettingsId = item.EntryData.ApplicationSettingsId;
                        var oldeds = new EntryDataDSContext().EntryData
                            .Include("AsycudaDocumentSets")
                            .Include("EntryDataDetails")
                            .Where(x => x.EntryDataId == entryDataId 
                                        && x.ApplicationSettingsId == applicationSettingsId ).ToList()
                            // this was to prevent deleting entrydata from other folders discrepancy with piece here and there with same entry data. but i changed the discrepancy to work with only one folder.
                            //.Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any())
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



                            switch (fileType.Type)
                            {
                                case "Sales":

                                    var EDsale = new Sales(true)
                                    {
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryType = "Sales",
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        INVNumber = entryDataId,
                                        CustomerName = item.EntryData.CustomerName,
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        Tax = item.EntryData.Tax,//item.f.Sum(x => x.TotalTax),
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
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
                                            $"{entryDataId} has no Invoice Total. Please check File.");
                                    var EDinv = new Invoices(true)
                                    {
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryType = "INV",
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                                        Packages = item.f.Sum(x => x.Packages),
                                        InvoiceTotal = item.f.Sum(x => (double) x.InvoiceTotal),

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
                                            $"{entryDataId} has no Invoice Total. Please check File.");
                                    

                                    var EDpo = new PurchaseOrders(true)
                                    {
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        PONumber = entryDataId,
                                        EntryType = "PO",
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
                                        Packages = item.f.Sum(x => (int)x.Packages),

                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        SupplierInvoiceNo = string.IsNullOrEmpty(item.EntryData.SupplierInvoiceNo)
                                            ? null
                                            : item.EntryData.SupplierInvoiceNo,
                                        FinancialInformation = string.IsNullOrEmpty(item.EntryData.FinancialInformation)
                                            ? null
                                            : item.EntryData.FinancialInformation,
                                        PreviousCNumber = string.IsNullOrEmpty(item.EntryData.PreviousCNumber)
                                            ? null
                                            : item.EntryData.PreviousCNumber,


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
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        OPSNumber = entryDataId,
                                        EntryType = "OPS",
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
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
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryType = "ADJ",
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        TrackingState = TrackingState.Added,
                                        SupplierCode = item.EntryData.Supplier,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
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
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryType = "ADJ",
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        SupplierCode = item.EntryData.Supplier,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                                        TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                                        TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
                                        EmailId = item.EntryData.EmailId == 0 ? null : item.EntryData.EmailId,
                                        FileTypeId = item.EntryData.FileTypeId,
                                        SourceFile = item.EntryData.SourceFile,
                                        Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                            ? null
                                            : item.EntryData.Currency,
                                        Type = "DIS"
                                    };
                                    if (!string.IsNullOrEmpty(item.EntryData.DocumentType))
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
                                        ApplicationSettingsId = applicationSettingsId,
                                        EntryDataId = entryDataId,
                                        EntryType = "ADJ",
                                        EntryDataDate = (DateTime) item.EntryData.EntryDataDate,
                                        TrackingState = TrackingState.Added,
                                        TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                                        TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                                        TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                                        InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
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
                                    VolumeLiters = e.VolumeLiters,
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

                        using (var ctx = new InventoryDSContext() { StartTracking = true })
                        {

                            foreach (var e in item.InventoryItems
                                .Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber).ToList())
                            {
                                string itemNumber = e.ItemNumber;
                                var inventoryItem = ctx.InventoryItems
                                    .Include("InventoryItemAlias")
                                    .First(x => x.ApplicationSettingsId == applicationSettingsId &&
                                                x.ItemNumber == itemNumber);
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
        private dynamic GalToLtrRate = 3.785;

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


        private void GetMappings(Dictionary<FileTypeMappings, int> mapping, string[] headings, FileTypes fileType)
        {

            for (var i = 0; i < headings.Count(); i++)
            {
                var h = headings[i].Trim().ToUpper();

                if (h == "") continue;

                var ftm = fileType.FileTypeMappings.FirstOrDefault(x => x.OriginalName.ToUpper().Trim() == h.Trim());
                if (ftm != null)
                {
                    mapping.Add(ftm, i);
                }
                else
                {
                    
                }
            }

        }


        private List<dynamic> GetCSVDataSummayList(string[] lines, Dictionary<FileTypeMappings, int> mapping,
            string[] headings)
        {
            int i = 0;
            try
            {
                var eslst = new List<dynamic>();

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



        private dynamic GetCSVDataFromLine(string line, Dictionary<FileTypeMappings, int> map, string[] headings)
        {
            var ImportChecks =
                new Dictionary<string, Func<dynamic, Dictionary<string, int>, string[], Tuple<bool, string>>>()
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

            var ImportActions = new Dictionary<string, Action<dynamic, Dictionary<string, int>, string[]>>()
            {
                {"EntryDataId", (c, mapping, splits) => c.EntryDataId = splits[mapping["EntryDataId"]].Trim().Replace("PO/GD/","").Replace("SHOP/GR_","")},
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
                {"ItemNumber", (c, mapping, splits) => c.ItemNumber = splits[mapping["ItemNumber"]].Replace("[","")},
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
                //{
                //    "Units",
                //    (c, mapping, splits) => c.Units = mapping.ContainsKey("Units") ? splits[mapping["Units"]] : ""
                //},
                //{
                //    "CustomerName",
                //    (c, mapping, splits) => c.CustomerName =
                //        mapping.ContainsKey("CustomerName") ? splits[mapping["CustomerName"]] : ""
                //},
                //{
                //    "Tax",
                //    (c, mapping, splits) =>
                //        c.Tax = Convert.ToSingle(mapping.ContainsKey("Tax") ? splits[mapping["Tax"]] : "0")
                //},
                //{
                //    "TotalTax",
                //    (c, mapping, splits) =>
                //        c.Tax = Convert.ToSingle(mapping.ContainsKey("TotalTax") ? splits[mapping["TotalTax"]] : "0")
                //},
                //{
                //    "TariffCode",
                //    (c, mapping, splits) =>
                //        c.TariffCode = mapping.ContainsKey("TariffCode") ? splits[mapping["TariffCode"]] : ""
                //},
                //{
                //    "SupplierCode",
                //    (c, mapping, splits) => c.SupplierCode =
                //        mapping.ContainsKey("SupplierCode") ? splits[mapping["SupplierCode"]] : ""
                //},
                //{
                //    "Freight",
                //    (c, mapping, splits) =>
                //        c.Freight = Convert.ToSingle(
                //            mapping.ContainsKey("Freight") && !string.IsNullOrEmpty(splits[mapping["Freight"]])
                //                ? splits[mapping["Freight"]]
                //                : "0")
                //},
                //{
                //    "Weight",
                //    (c, mapping, splits) =>
                //        c.Weight = Convert.ToSingle(
                //            mapping.ContainsKey("Weight") && !string.IsNullOrEmpty(splits[mapping["Weight"]])
                //                ? splits[mapping["Weight"]]
                //                : "0")
                //},
                //{
                //    "InternalFreight",
                //    (c, mapping, splits) => c.InternalFreight = Convert.ToSingle(
                //        mapping.ContainsKey("InternalFreight") &&
                //        !string.IsNullOrEmpty(splits[mapping["InternalFreight"]])
                //            ? splits[mapping["InternalFreight"]]
                //            : "0")
                //},
                //{
                //    "TotalFreight",
                //    (c, mapping, splits) => c.TotalFreight = Convert.ToSingle(
                //        mapping.ContainsKey("TotalFreight") && !string.IsNullOrEmpty(splits[mapping["TotalFreight"]])
                //            ? splits[mapping["TotalFreight"]]
                //            : "0")
                //},
                //{
                //    "TotalWeight",
                //    (c, mapping, splits) => c.TotalWeight = Convert.ToSingle(
                //        mapping.ContainsKey("TotalWeight") && !string.IsNullOrEmpty(splits[mapping["TotalWeight"]])
                //            ? splits[mapping["TotalWeight"]]
                //            : "0")
                //},
                //{
                //    "TotalInternalFreight",
                //    (c, mapping, splits) => c.TotalInternalFreight = Convert.ToSingle(
                //        mapping.ContainsKey("TotalInternalFreight") &&
                //        !string.IsNullOrEmpty(splits[mapping["TotalInternalFreight"]])
                //            ? splits[mapping["TotalInternalFreight"]]
                //            : "0")
                //},
                //{
                //    "TotalOtherCost",
                //    (c, mapping, splits) => c.TotalOtherCost = Convert.ToSingle(
                //        mapping.ContainsKey("TotalOtherCost") &&
                //        !string.IsNullOrEmpty(splits[mapping["TotalOtherCost"]])
                //            ? splits[mapping["TotalOtherCost"]]
                //            : "0")
                //},
                //{
                //    "TotalInsurance",
                //    (c, mapping, splits) => c.TotalInsurance = Convert.ToSingle(
                //        mapping.ContainsKey("TotalInsurance") &&
                //        !string.IsNullOrEmpty(splits[mapping["TotalInsurance"]])
                //            ? splits[mapping["TotalInsurance"]]
                //            : "0")
                //},
                //{
                //    "TotalDeductions",
                //    (c, mapping, splits) => c.TotalDeductions = Convert.ToSingle(
                //        mapping.ContainsKey("TotalDeductions") &&
                //        !string.IsNullOrEmpty(splits[mapping["TotalDeductions"]])
                //            ? splits[mapping["TotalDeductions"]]
                //            : "0")
                //},
                //{
                //    "pCNumber",
                //    (c, mapping, splits) => c.CNumber = mapping.ContainsKey("pCNumber") ? splits[mapping["pCNumber"]] : ""
                //},
                //{
                //    "InvoiceQuantity",
                //    (c, mapping, splits) => c.InvoiceQuantity = mapping.ContainsKey("InvoiceQuantity")
                //        ? Convert.ToSingle(splits[mapping["InvoiceQuantity"]])
                //        : 0
                //},
                //{
                //    "ReceivedQuantity",
                //    (c, mapping, splits) => c.ReceivedQuantity =
                //        mapping.ContainsKey("ReceivedQuantity") && splits.Length >= mapping["ReceivedQuantity"]
                //            ? Convert.ToSingle(splits[mapping["ReceivedQuantity"]])
                //            : 0
                //},
                //{
                //    "Currency",
                //    (c, mapping, splits) =>
                //        c.Currency = mapping.ContainsKey("Currency") ? splits[mapping["Currency"]] : ""
                //},
                //{
                //    "Comment",
                //    (c, mapping, splits) => c.Comment = mapping.ContainsKey("Comment") ? splits[mapping["Comment"]] : ""
                //},
                //{
                //    "PreviousInvoiceNumber",
                //    (c, mapping, splits) => c.PreviousInvoiceNumber = mapping.ContainsKey("PreviousInvoiceNumber")
                //        ? splits[mapping["PreviousInvoiceNumber"]]
                //        : ""
                //},
                //{
                //    "EffectiveDate",
                //    (c, mapping, splits) => c.EffectiveDate =
                //        mapping.ContainsKey("EffectiveDate") && !string.IsNullOrEmpty(splits[mapping["EffectiveDate"]])
                //            ? DateTime.Parse(splits[mapping["EffectiveDate"]], CultureInfo.CurrentCulture)
                //            : (DateTime?) null
                //},
                {
                    "TotalCost",
                    (c, mapping, splits) => c.Cost = !string.IsNullOrEmpty(splits[mapping["TotalCost"]]) && Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) != 0.0
                        ? Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) / (c.Quantity?? Convert.ToSingle(splits[mapping["Quantity"]].Replace("�", "")))
                        : c.Cost
                },
                {
                    "SupplierItemNumber",
                    (c, mapping, splits) =>
                    {
                        c.ItemNumber = !string.IsNullOrEmpty(splits[mapping["POItemNumber"]])
                            ? splits[mapping["POItemNumber"]]
                            : splits[mapping["SupplierItemNumber"]];
                        
                    }
                },
                {
                    "SupplierItemDescription",
                    (c, mapping, splits) => c.ItemDescription = !string.IsNullOrEmpty(splits[mapping["POItemDescription"]]) ? splits[mapping["POItemDescription"]] : splits[mapping["SupplierItemDescription"]]
                },
                {
                    "SupplierInvoiceNo",
                    (c, mapping, splits) => {c.EntryDataId = !string.IsNullOrEmpty(splits[mapping["EntryDataId"]]) ? splits[mapping["EntryDataId"]].Trim().Replace("PO/GD/","").Replace("SHOP/GR_","") : splits[mapping["SupplierInvoiceNo"]];
                                             c.SupplierInvoiceNo =  splits[mapping["SupplierInvoiceNo"]]; }
                },
                {
                    "POItemNumber",
                    (c, mapping, splits) => c.ItemNumber = !string.IsNullOrEmpty(splits[mapping["POItemNumber"]]) ? splits[mapping["POItemNumber"]] : splits[mapping["SupplierItemNumber"]]
                },
                {
                    "POItemDescription",
                    (c, mapping, splits) => c.ItemDescription = !string.IsNullOrEmpty(splits[mapping["POItemDescription"]]) ? splits[mapping["POItemDescription"]] : splits[mapping["SupplierItemDescription"]]
                },
               
                //{
                //    "InvoiceTotal",
                //    (c, mapping, splits) => c.InvoiceTotal = Convert.ToSingle(
                //        mapping.ContainsKey("InvoiceTotal") && !string.IsNullOrEmpty(splits[mapping["InvoiceTotal"]])
                //            ? splits[mapping["InvoiceTotal"]]
                //            : "0")
                //},
                //{
                //    "SupplierName",
                //    (c, mapping, splits) => c.SupplierName =
                //        mapping.ContainsKey("SupplierName") ? splits[mapping["SupplierName"]] : ""
                //},
                //{
                //    "SupplierAddress",
                //    (c, mapping, splits) => c.SupplierAddress =
                //        mapping.ContainsKey("SupplierAddress") ? splits[mapping["SupplierAddress"]] : ""
                //},
                //{
                //    "CountryCode",
                //    (c, mapping, splits) => c.SupplierCountryCode =
                //        mapping.ContainsKey("CountryCode") ? splits[mapping["CountryCode"]] : ""
                //},
                //{
                //    "DocumentType",
                //    (c, mapping, splits) => c.DocumentType =
                //        mapping.ContainsKey("DocumentType") ? splits[mapping["DocumentType"]] : ""
                //},
                //{
                //    "SupplierInvoiceNo",
                //    (c, mapping, splits) => c.SupplierInvoiceNo = mapping.ContainsKey("SupplierInvoiceNo")
                //        ? splits[mapping["SupplierInvoiceNo"]]
                //        : ""
                //},
                //{
                //    "InventorySource",
                //    (c, mapping, splits) => c.SupplierInvoiceNo = mapping.ContainsKey("InventorySource")
                //        ? splits[mapping["InventorySource"]]
                //        : ""
                //},
                //{
                //    "Packages",
                //    (c, mapping, splits) => c.Packages = Convert.ToInt32(mapping.ContainsKey("Packages") && !string.IsNullOrEmpty(splits[mapping["Packages"]])
                //        ? splits[mapping["Packages"]]
                //        : "0")
                //},
                //{
                //    "WarehouseNo",
                //    (c, mapping, splits) => c.WarehouseNo = mapping.ContainsKey("WarehouseNo")
                //        ? splits[mapping["WarehouseNo"]]
                //        : ""
                //},
                //{
                //    "FinancialInformation",
                //    (c, mapping, splits) => c.FinancialInformation = mapping.ContainsKey("FinancialInformation")
                //        ? splits[mapping["FinancialInformation"]]
                //        : ""
                //},
                //{
                //    "PreviousCNumber",
                //    (c, mapping, splits) => c.PreviousCNumber = mapping.ContainsKey("PreviousCNumber")
                //        ? splits[mapping["PreviousCNumber"]]
                //        : ""
                //},
                //{
                //    "Office",
                //    (c, mapping, splits) => c.Office = mapping.ContainsKey("Office")
                //        ? splits[mapping["Office"]]
                //        : ""
                //},
                //{
                //    "GeneralProcedure",
                //    (c, mapping, splits) => c.GeneralProcedure = mapping.ContainsKey("GeneralProcedure")
                //        ? splits[mapping["GeneralProcedure"]]
                //        : ""
                //},
                //{
                //    "AssessmentDate",
                //    (c, mapping, splits) => c.AssessmentDate = mapping.ContainsKey("AssessmentDate")
                //        ? splits[mapping["AssessmentDate"]]
                //        : ""
                //},
                //{
                //    "AssessmentNumber",
                //    (c, mapping, splits) => c.AssessmentNumber = mapping.ContainsKey("AssessmentNumber")
                //        ? splits[mapping["AssessmentNumber"]]
                //        : ""
                //},
                //{
                //    "AssessmentSerial",
                //    (c, mapping, splits) => c.AssessmentSerial = mapping.ContainsKey("AssessmentSerial")
                //        ? splits[mapping["AssessmentSerial"]]
                //        : ""
                //},
                //{
                //    "RegistrationDate",
                //    (c, mapping, splits) => c.RegistrationDate = mapping.ContainsKey("RegistrationDate")
                //        ? splits[mapping["RegistrationDate"]]
                //        : ""
                //},
                //{
                //    "RegistrationNumber",
                //    (c, mapping, splits) => c.RegistrationNumber = mapping.ContainsKey("RegistrationNumber")
                //        ? splits[mapping["RegistrationNumber"]]
                //        : ""
                //},
                //{
                //    "RegistrationSerial",
                //    (c, mapping, splits) => c.RegistrationSerial = mapping.ContainsKey("RegistrationSerial")
                //        ? splits[mapping["RegistrationSerial"]]
                //        : ""
                //},
                //{
                //    "Consignee",
                //    (c, mapping, splits) => c.Consignee = mapping.ContainsKey("Consignee")
                //        ? splits[mapping["Consignee"]]
                //        : ""
                //},
                //{
                //    "Exporter",
                //    (c, mapping, splits) => c.Exporter = mapping.ContainsKey("Exporter")
                //        ? splits[mapping["Exporter"]]
                //        : ""
                //},
                //{
                //    "DeclarantCode",
                //    (c, mapping, splits) => c.DeclarantCode = mapping.ContainsKey("DeclarantCode")
                //        ? splits[mapping["DeclarantCode"]]
                //        : ""
                //},
                //{
                //    "DeclarantReference",
                //    (c, mapping, splits) => c.DeclarantReference = mapping.ContainsKey("DeclarantReference")
                //        ? splits[mapping["DeclarantReference"]]
                //        : ""
                //},
                //{
                //    "ExpirationDate",
                //    (c, mapping, splits) => c.ExpirationDate = mapping.ContainsKey("ExpirationDate")
                //        ? splits[mapping["ExpirationDate"]]
                //        : ""
                //},
                {
                    "Total",
                    (c, mapping, splits) => {}
                },

            };
            
            try
            {
                if (string.IsNullOrEmpty(line)) return null;
                var splits = line.CsvSplit().Select(x => x.Trim()).ToArray();
                if (splits.Length < headings.Length) return null;
                //if (!map.Keys.Contains("EntryDataId"))
                //    throw new ApplicationException("Invoice# not Mapped");
                //if (!map.Keys.Contains("ItemNumber"))
                //    throw new ApplicationException("ItemNumber not Mapped");

                //if (splits[map["EntryDataId"]] != "" && splits[map["ItemNumber"]] != "")
                //{
                    dynamic res = new BetterExpando();
                    res.SourceRow = line;
                    foreach (var key in map.Keys)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(splits[map[key]]))
                            {
                                if(((IDictionary<string, object>)res).ContainsKey(key.DestinationName)  &&    string.IsNullOrEmpty(((IDictionary<string, object>)res)[key.DestinationName] as string))
                                     ((IDictionary<string, object>) res)[key.DestinationName] = "";
                                continue;
                            }

                            if (ImportChecks.ContainsKey(key.DestinationName))
                            {
                                
                                var err = ImportChecks[key.DestinationName].Invoke(res, map.ToDictionary(x => x.Key.DestinationName, x => x.Value), splits);
                                if (err.Item1) throw new ApplicationException(err.Item2);
                            }

                            if (ImportActions.ContainsKey(key.DestinationName))
                            {
                                ImportActions[key.DestinationName].Invoke(res, map.ToDictionary(x => x.Key.DestinationName, x => x.Value), splits);
                            }
                            else
                            {
                                ((IDictionary<string, object>)res)[key.DestinationName] =  GetMappingValue(key, splits, map[key]);
                            }
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
               // }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }



        private object GetMappingValue(FileTypeMappings key, string[] splits, int index)
        {
            try
            {


                var val = splits[index];
                foreach (var regEx in key.FileTypeMappingRegExs)
                {
                    val = Regex.Replace(val, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                        RegexOptions.IgnoreCase);
                }

                if (key.DataType == "Date")
                {
                    DateTime rdate;
                    if (DateTime.TryParse(val, out rdate))
                    {
                        return rdate;
                    }

                    var formatStrings = new List<string>() { "M/y", "M/d/y", "M-d-y", "dd/MM/yyyy", "dd/M/yyyy" };
                    foreach (String formatString in formatStrings)
                    {
                        if (DateTime.TryParseExact(val, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out rdate))
                            return rdate;
                    }

                    return rdate; //DateTime.Parse(val);
                }

                if (key.DataType == "Number") return Convert.ToSingle(string.IsNullOrEmpty(val) ? "0" : val);
                return val;
            }
            catch (Exception)
            {

                throw;
            }
        }



       
        private async Task ImportSuppliers(List<dynamic> eslst, int applicationSettingsId, FileTypes fileType)
        {
            try
            {
               
                var itmlst = eslst
                    .GroupBy(x => new {x.SupplierCode, x.SupplierName, x.SupplierAddress, x.CountryCode})
                    .ToList();

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true && fileType.Type == "PO")
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
                            CountryCode = item.Key.CountryCode,

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

        private async Task ImportInventory(List<dynamic> eslst, int applicationSettingsId, FileTypes fileType)
        {
            try
            {
               
               

               var itmlst = eslst.Where(x => x.ItemNumber != null)
                            .GroupBy(g => new { ItemNumber = g.ItemNumber.ToUpper(), g.ItemDescription, g.TariffCode})
                            .ToList();
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType.Type)
                {
                    case "Shipment Invoice":
                    case "INV":

                        inventorySource = dctx.InventorySources.FirstOrDefault(x => x.Name == "Supplier");
                        break;
                    case "PO":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "OPS":
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
                var inventoryItems = ctx.InventoryItems
                    .Include("InventoryItemSources.InventorySource")
                    .Include("InventoryItemAlias")
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
                            ItemNumber = ((string)item.Key.ItemNumber).Truncate(20),
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

                    var invItemCodes = item.Select(x => new {x.SupplierItemNumber, x.SupplierItemDescription}).Where(x => !string.IsNullOrEmpty(x.SupplierItemNumber) && i.ItemNumber != x.SupplierItemNumber )
                        .DistinctBy(x => x.SupplierItemNumber).ToList();
                    foreach (var invItemCode in invItemCodes)
                    {
                        string supplierItemNumber = invItemCode.SupplierItemNumber.ToString();
                        var invItem = ctx.InventoryItems.FirstOrDefault(x => x.ItemNumber == supplierItemNumber);
                        if(invItem == null)
                        { invItem = new InventoryItem(true)
                            {
                                ApplicationSettingsId = applicationSettingsId,
                                Description = invItemCode.SupplierItemDescription,
                                ItemNumber = ((string)invItemCode.SupplierItemNumber).Truncate(20),
                                InventoryItemSources = new List<InventoryItemSource>(){ new InventoryItemSource(true)
                                {
                                    InventorySourceId = inventorySource.Id,
                                    TrackingState = TrackingState.Added
                                }},
                                TrackingState = TrackingState.Added
                            };
                            ctx.InventoryItems.Add(invItem);
                            ctx.SaveChanges();
                        } 
                        if (i.InventoryItemAlias.FirstOrDefault(x => x.AliasName == supplierItemNumber) == null)
                        {
                            i.InventoryItemAlias.Add(new InventoryItemAlia(true)
                            {
                                InventoryItemId = i.Id,
                                AliasName = supplierItemNumber.Truncate(20),
                                AliasId = invItem.Id,
                                TrackingState = TrackingState.Added

                            });

                        }
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