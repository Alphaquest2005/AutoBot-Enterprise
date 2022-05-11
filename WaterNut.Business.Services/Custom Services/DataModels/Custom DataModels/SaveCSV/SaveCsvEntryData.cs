using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
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

        public async Task<bool> ExtractEntryData(FileTypes suggestedfileType, string[] lines, string[] headings, 
            List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, 
            string droppedFilePath)
        {
            try
            {
                var fileType =FileTypeManager.GetHeadingFileType(headings, suggestedfileType);

                var data = new CSVDataExtractor(fileType, lines, headings, emailId).Execute();

                if (data == null) return true;


                
                return await ProcessCsvSummaryData(fileType, docSet, overWriteExisting, emailId, 
                    droppedFilePath, data).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nex = new ApplicationException($"Error Importing File: {droppedFilePath} - {e.Message}", e);
                Console.WriteLine(nex);
                throw nex;
            }
        }






        private void ProcessCsvRider(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            string emailId,  string droppedFilePath, List<dynamic> eslst)
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
                        x.Key.ETA,
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
                    foreach (var rawRider in rawRiders)//.Where(x => x.ETA != null) // bad data duh make sense
                    {
                        DateTime eta = (DateTime) (rawRider.ETA ?? DateTime.MinValue);
                        var existingRider = ctx.ShipmentRider.Where(x => x.ETA == eta).ToList()
                            .Where(x => new FileInfo(x.SourceFile).Name == new FileInfo(droppedFilePath).Name);
                        if (existingRider.Any()) ctx.ShipmentRider.RemoveRange(existingRider);
                        
                        var invoiceLst = rawRider.ShipmentRiderDetails.Select(x => new
                            {
                                x.WarehouseCode,
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
                            ETA = (DateTime)(rawRider.ETA ?? DateTime.MinValue),
                            DocumentDate = (DateTime)(rawRider.DocumentDate ?? DateTime.MinValue),
                            FileTypeId =fileType.Id,
                            EmailId = emailId,
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

        private void ProcessCsvExpiredEntries(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> eslst)
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


        private void ProcessCsvCancelledEntries(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.ExecuteSqlCommand("delete from CancelledEntriesLst");
                    foreach (var itm in eslst)
                    {
                        var expireditm = new CancelledEntriesLst(true)
                        {
                            Office = itm.Office,
                            GeneralProcedure = itm.GeneralProcedure,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            RegistrationDate = itm.RegistrationDate,
                            RegistrationNumber = itm.RegistrationNumber,
                            TrackingState = TrackingState.Added
                        };
                        ctx.CancelledEntriesLst.Add(expireditm);
                    }

                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand($@"UPDATE xcuda_ASYCUDA_ExtendedProperties
                                                        SET         Cancelled = 1
                                                        FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code
                                                                            FROM     CancelledEntriesLst INNER JOIN
                                                                                            AsycudaDocument ON CancelledEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND CancelledEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                                                                            CancelledEntriesLst.RegistrationNumber = AsycudaDocument.CNumber ) AS exp INNER JOIN
                                                                            xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<bool> ProcessCsvSummaryData(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {

                if (docSet.Any(x =>
                        x.ApplicationSettingsId !=
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    throw new ApplicationException("Doc Set not belonging to Current Company");


                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Rider)
                {
                    ProcessCsvRider(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Manifest)
                {
                    ProcessManifest(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.BL)
                {
                    ProcessBL(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Freight)
                {
                    ProcessFreight(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice)
                {

                    if (eslst.FirstOrDefault() is IDictionary<string, object> itm && itm.Keys.Contains("EntryDataId"))
                    {


                        var entrydataid = itm["EntryDataId"];
                        var xeslst = ConvertCSVToShipmentInvoice(eslst);
                        var xdroppedFilePath = new CoreEntitiesContext().Attachments.Where(x =>
                                x.FilePath.Contains(entrydataid + ".pdf")).OrderByDescending(x => x.Id).FirstOrDefault()
                            ?.FilePath;
                        if (xeslst == null) return false;
                        await ImportInventory(
                            xeslst.SelectMany(x =>
                                ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                                ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                            docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                        var invoicePOs = xeslst.SelectMany(x =>
                                ((List<IDictionary<string, object>>)x).Select(z =>
                                    new { InvoiceNo = z["InvoiceNo"], PONumber = z["PONumber"] }))
                            .Distinct()
                            .ToDictionary(x => x.InvoiceNo.ToString(), x => x.PONumber?.ToString() ?? "");
                        ProcessShipmentInvoice(fileType, docSet, overWriteExisting, emailId,
                            xdroppedFilePath, xeslst, invoicePOs);

                    }
                    else
                    {
                        await ImportInventory(
                            eslst.SelectMany(x =>
                                ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                                ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                            docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);



                        ProcessShipmentInvoice(fileType, docSet, overWriteExisting, emailId,
                            droppedFilePath, eslst, null);

                        return true;
                    }



                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ExpiredEntries)
                {
                    ProcessCsvExpiredEntries(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.CancelledEntries)
                {
                    ProcessCsvCancelledEntries(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }


                {
                    await ImportInventory(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                    await ImportSuppliers(eslst, docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                    await ImportEntryDataFile(fileType, eslst.Where(x => !string.IsNullOrEmpty(x.SourceRow)).ToList(),
                            emailId, fileType.Id, droppedFilePath, docSet.First().ApplicationSettingsId)
                        .ConfigureAwait(false);
                    if (await ImportEntryData(fileType, eslst, docSet, overWriteExisting, emailId,
                                droppedFilePath)
                            .ConfigureAwait(false)) return true;
                }
                


                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private List<dynamic> ConvertCSVToShipmentInvoice(List<dynamic> eslst)
        {
           

            var ed = (eslst.Select(x => (dynamic)x)
                .GroupBy(es => new { es.EntryDataId, es.EntryDataDate, es.CustomerName })
                .Select(g => new
                {
                    EntryData = new
                    {
                        g.Key.EntryDataId,
                        g.Key.EntryDataDate,
                        g.Key.CustomerName,
                        ((dynamic)g.FirstOrDefault())?.Tax,
                        Supplier =
                            string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                                ? null
                                : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))?.FinancialInformation,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                    },
                    EntryDataDetails = g.Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .Select(x => new
                        {
                            x.EntryDataId,
                            //Can't set entrydata_id here cuz this is from data
                            ItemNumber = ((string)x.ItemNumber.ToUpper()).Truncate(20),
                            x.ItemDescription,
                            x.SupplierItemNumber,
                            x.SupplierItemDescription,
                            Cost = x.Cost ?? 0,
                            TotalCost = Convert.ToDouble((double)(x.TotalCost ?? 0.0)),
                            Quantity = Convert.ToDouble((double)(x.Quantity ?? 0.0)),
                            FileLineNumber = x.LineNumber,
                            x.Units,
                            Freight = Convert.ToDouble((double)(x.Freight ?? 0.0)),
                            Weight = Convert.ToDouble((double)(x.Weight ?? 0.0)),
                            InternalFreight = Convert.ToDouble((double)(x.InternalFreight ?? 0.0)),
                            InvoiceQty = Convert.ToDouble((double)(x.InvoiceQuantity ?? 0.0)),
                            ReceivedQty = Convert.ToDouble((double)(x.ReceivedQuantity ?? 0.0)),
                            TaxAmount = x.Tax ?? 0,
                            CNumber = x.PreviousCNumber,
                            CLineNumber = (int?)x.PreviousCLineNumber,
                            x.PreviousInvoiceNumber,
                            x.Comment,
                            InventoryItemId = x.ItemId ?? 0,
                            x.EffectiveDate,
                            VolumeLiters =
                                Convert.ToDouble(
                                    x.Gallons * GalToLtrRate ?? Convert.ToDouble((double)(x.Liters ?? 0.0))),
                        }),
                    f = g.Select(x => new
                    {
                        TotalWeight = Convert.ToDouble((double)(x.TotalWeight ?? 0.0)),
                        TotalFreight = Convert.ToDouble((double)(x.TotalFreight ?? 0.0)),
                        TotalInternalFreight = Convert.ToDouble((double)(x.TotalInternalFreight ?? 0.0)),
                        TotalOtherCost = Convert.ToDouble((double)(x.TotalOtherCost ?? 0.0)),
                        TotalInsurance = Convert.ToDouble((double)(x.TotalInsurance ?? 0.0)),
                        TotalDeductions = Convert.ToDouble((double)(x.TotalDeductions ?? 0.0)),
                        InvoiceTotal = Convert.ToDouble((double)(x.InvoiceTotal ?? 0.0)),
                        TotalTax = Convert.ToDouble((double)(x.TotalTax ?? 0.0)),
                        Packages = Convert.ToInt32((int)(x.Packages ?? 0)),
                        x.WarehouseNo,
                    }),
                    InventoryItems = g.DistinctBy(x => new { x.ItemNumber, x.ItemAlias })
                        .Select(x => new { x.ItemNumber, x.ItemAlias })
                })).ToList();


            var res = new List<dynamic>();
            foreach (var itm in ed)
            {
                dynamic header = (IDictionary<string, object>)new BetterExpando();
                header.InvoiceNo = itm.EntryData.SupplierInvoiceNo;
                header.PONumber = itm.EntryData.EntryDataId;
                header.InvoiceDate = itm.EntryData.EntryDataDate;
                header.InvoiceTotal = itm.f.Sum(x => (double)x.InvoiceTotal);
                header.TotalInternalFreight = itm.f.Sum(x => (double)x.TotalInternalFreight);
                header.TotalOtherCost = itm.f.Sum(x => (double)x.TotalOtherCost);
                header.TotalDeduction = itm.f.Sum(x => (double)x.TotalDeductions);
                header.SupplierCode = itm.EntryData.Supplier;


                dynamic extraInfo = (IDictionary<string, object>)new BetterExpando();
                extraInfo.PONumber = itm.EntryData.PONumber;

                dynamic invoiceDetails = new List<IDictionary<string, object>>();

                foreach (var entryDataDetail in itm.EntryDataDetails)
                {
                    if (string.IsNullOrEmpty(entryDataDetail.SupplierItemNumber)) return null;
                    dynamic edd = (IDictionary<string, object>)new BetterExpando();
                    edd.ItemNumber = entryDataDetail.SupplierItemNumber;
                    edd.ItemDescription = entryDataDetail.SupplierItemDescription;
                    edd.Quantity = entryDataDetail.Quantity;
                    edd.Cost = entryDataDetail.Cost;
                    edd.TotalCost = entryDataDetail.TotalCost;
                    edd.FileLineNumber = entryDataDetail.FileLineNumber;

                    invoiceDetails.Add((IDictionary<string, object>)edd);
                }

                header.ExtraInfo = new List<IDictionary<string, object>>() { extraInfo };
                header.InvoiceDetails = invoiceDetails;

                res.Add(new List<IDictionary<string, object>>(){(IDictionary<string, object>)header });
            }


            return res;
        }

        private void ProcessShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst, Dictionary<string, string> invoicePOs)
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
                                                .Where(z => z.ContainsKey("ItemDescription"))
                                              
                                                .Select(z => new InvoiceDetails()
                                                {
                                                    Quantity = Convert.ToDouble(z["Quantity"].ToString()),
                                                    ItemNumber = z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20): null,
                                                    ItemDescription = z["ItemDescription"].ToString().Truncate(255),
                                                    Units = z.ContainsKey("Units") ? z["Units"].ToString() : null,
                                                    Cost = z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) : Convert.ToDouble(z["TotalCost"].ToString()) / (Convert.ToDouble(z["Quantity"].ToString()) == 0 ? 1 : Convert.ToDouble(z["Quantity"].ToString())),
                                                    TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) : Convert.ToDouble(z["Cost"].ToString()) * Convert.ToDouble(z["Quantity"].ToString()),
                                                    Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0,
                                                    Volume = z.ContainsKey("Gallons") ? new InvoiceDetailsVolume() {Quantity = Convert.ToDouble(z["Gallons"].ToString()), Units = "Gallons", TrackingState = TrackingState.Added, } : null,
                                                    SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA") || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units")) ? Convert.ToInt32(z["SalesFactor"].ToString()) /* * (z.ContainsKey("Multiplier")  ? Convert.ToInt32(z["Multiplier"].ToString()) : 1) */ : 1,
                                                    LineNumber = z.ContainsKey("Instance") ? Convert.ToInt32(z["Instance"].ToString()) :((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1,
                                                    FileLineNumber = Convert.ToInt32(z["FileLineNumber"].ToString()),
                                                    Section = z.ContainsKey("Section") ? z["Section"].ToString( ): null,
                                                    InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"]: (int?)null,
                                                    TrackingState = TrackingState.Added,

                                                }).ToList(),
                        InvoiceExtraInfo = !x.ContainsKey("ExtraInfo")? new List<InvoiceExtraInfo>(): ((List<IDictionary<string, object>>)x["ExtraInfo"])
                            .Where(z => z.Keys.Any())
                            .SelectMany(z => z)
                            .Where(z => z.Value != null)
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

                        if (invoicePOs != null && lst.Count > 1 && invoice != lst.First())
                        {
                            invoice.SourceFile = invoice.SourceFile.Replace($"{invoicePOs[lst.First().InvoiceNo]}",
                                invoicePOs[invoice.InvoiceNo]);
                        }

                        ctx.ShipmentInvoice.Add(invoice);
                        
                        ctx.SaveChanges();

                        //----------ALLOW IMPORTS AND CHANGE THE XLSX TO HIGHLIGHT ERRORS
                        //if (invoice.ImportedTotalDifference > 0.001 )
                        //    throw new ApplicationException(
                        //        $"Imported Total Difference for Invoice > 0: {invoice.ImportedTotalDifference}");
                    }

                    
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

            //if (invoice.SubTotal > 0
            //    && Math.Abs((double) (invoice.SubTotal - details.Sum(x => x.Cost * x.Quantity))) > 0.01)
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<InvoiceDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.TotalCost == detail.TotalCost && x.ItemNumber == detail.ItemNumber &&
                            x.Quantity == detail.Quantity && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

                //details = details.DistinctBy(x => new { ItemNumber = x.ItemNumber.ToUpper(), x.Quantity, x.TotalCost})
                //    .ToList();


          //  var invoiceInvoiceDetails = lst ;
            return lst;
        }

        private void ProcessFreight(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst)
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
        private void ProcessBL(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId,  string droppedFilePath, List<object> eslst)
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
                        PackagesType = x.ContainsKey("PackagesType") ? x["PackagesType"].ToString().Truncate(10): "PK",
                        WeightKG = x.ContainsKey("WeightKG") ? Convert.ToDouble(x["WeightKG"].ToString()): x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString())* lb2Kg : 0,
                        VolumeM3 = x.ContainsKey("VolumeM3") ? Convert.ToDouble(x["VolumeM3"].ToString()) : x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString())* CF2M3 : 0,

                        WeightLB = x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString()): (double?) null,
                        VolumeCF = x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString()): (double?) null,



                        Freight = x.ContainsKey("Freight") ? Convert.ToDouble(x["Freight"].ToString()): (double?) null,
                        FreightCurrency = x.ContainsKey("FreightCurrency") ? x["FreightCurrency"].ToString(): null,
                        //LocationOfGoods = x["LocationOfGoods"].ToString(),
                        //Goods = x["Goods"].ToString(),
                        //Marks = x["Marks"].ToString(),
                        //Containers = Convert.ToInt32(x["Containers"].ToString()),
                        ShipmentBLDetails = ((List<IDictionary<string, object>>)x["ShipmentBLDetails"])
                                                .Select(z => new ShipmentBLDetails()
                                                                {
                                                                    Quantity = z.ContainsKey("Quantity") ? Convert.ToInt32(z["Quantity"].ToString()): 0,
                                                                    Marks = z.ContainsKey("Marks") ? z["Marks"].ToString(): "",
                                                                    PackageType = z.ContainsKey("PackageType") ? z["PackageType"].ToString().Truncate(10): "",
                                                                    Weight = z.ContainsKey("Weight") ? z["Weight"].ToString() : "",
                                                    Section = z.ContainsKey("Section") ? z["Section"].ToString() : null,
                                                                    TrackingState = TrackingState.Added,
                                                               
                                                            }).ToList(),

                        EmailId = emailId,
                       // SourceFile = droppedFilePath,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var bl in lst)
                    {
                        var blDetails = AutoFixShipmentBlDetails(bl);

                        bl.ShipmentBLDetails = blDetails;

                        var detailsQty = bl.ShipmentBLDetails.Sum(x => x.Quantity);


                        var filename = BaseDataModel.SetFilename(droppedFilePath, bl.BLNumber, "-BL.pdf");
                        if (!File.Exists(filename)) File.Copy(droppedFilePath, filename);
                        bl.SourceFile = filename;
                        var existingBl =
                            ctx.ShipmentBL.FirstOrDefault(
                                x => x.BLNumber == bl.BLNumber);
                        if (existingBl != null)
                            ctx.ShipmentBL.Remove(existingBl);
                        ctx.ShipmentBL.Add(bl);

                        ctx.SaveChanges();
                        if (bl.PackagesNo != detailsQty)
                        {
                            throw new ApplicationException(
                                $"BL Details Quantity don't add up to BL Total Packages! - BL{bl.PackagesNo} vs Details{detailsQty}");
                        }

                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ShipmentBLDetails> AutoFixShipmentBlDetails(ShipmentBL bl)
        {
            var details = bl.ShipmentBLDetails.Any(x => !string.IsNullOrEmpty(x.Weight))? bl.ShipmentBLDetails.Where(x => !string.IsNullOrEmpty(x.Weight)).ToList() :bl.ShipmentBLDetails.ToList() ;

           
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<ShipmentBLDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.Quantity == detail.Quantity && x.Marks == detail.Marks &&
                            x.PackageType == detail.PackageType && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

            return lst;
            
        }

        private void ProcessManifest(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst)
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
                        var bls = ctx.ShipmentBL
                            .Where(x => x.BLNumber == manifest.WayBill || x.Voyage == manifest.Voyage).ToList();
                        if (bls.Any() && bls.All(x => x.PackagesNo != manifest.Packages))
                        {
                            throw new ApplicationException(
                                $"Manifest:{manifest.RegistrationNumber} Packages <> BL:{bls.Select(x => x.BLNumber).Aggregate((o, n) => o + ", " + n)} Packages");
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

        private async Task ImportEntryDataFile(FileTypes fileType, List<dynamic> eslst, string emailId,
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
                            FileType = fileType.FileImporterInfos.EntryType,
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
            bool overWriteExisting, string emailId,  string droppedFilePath)
        {
            try
            {
               


                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Sales && !(((IDictionary<string, object>)eslst.First()).ContainsKey("Tax") || ((IDictionary<string, object>)eslst.First()).ContainsKey("TotalTax")))
                    throw new ApplicationException("Sales file dose not contain Tax");

             


                var ed = GetRawEntryData(fileType, eslst, docSet, emailId, droppedFilePath);

                if (ed == null) return true;

                //Parallel.ForEach(ed, new ParallelOptions() { MaxDegreeOfParallelism = 3 },//Environment.ProcessorCount * 1
                //    async item =>
                foreach (var item in ed.Where(x =>
                             x.EntryData.EntryDataId != null && x.EntryData.EntryDataDate != null))

                {
                    await SaveEntryData(fileType, docSet, overWriteExisting, emailId, item).ConfigureAwait(false);

                    UpdateInventoryItems(item);
                } //);




                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveEntryData(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            string entryDataId = item.EntryData.EntryDataId;
            if (!item.EntryDataDetails.Any())
                throw new ApplicationException(entryDataId + " has no details");

            var data = await GetDataToUpDate(fileType, docSet, overWriteExisting, item).ConfigureAwait(false);


            await SaveEntryDataDetails(docSet, overWriteExisting, data).ConfigureAwait(false);
        }

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetDataToUpDate(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            var emailId = item.EntryData.EmailId;
            var data = await GetDataToUpdate(docSet, overWriteExisting, item).ConfigureAwait(false);


            if (overWriteExisting || data.existingEntryData == null)
            {
                data.existingEntryData = await GetNewEntryData(fileType, docSet, item).ConfigureAwait(false);
            }
            else
            {
                data.existingEntryData.EmailId = emailId;
            }

            return data;
        }

        private async Task SaveEntryDataDetails(List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            (dynamic existingEntryData, List<EntryDataDetails> details) data)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var lineNumber = 0;

                foreach (var e in data.details)
                {
                    lineNumber += 1;
                    ctx.EntryDataDetails.Add(new EntryDataDetails(true)
                    {
                        EntryDataId = e.EntryDataId,
                        EntryData_Id = data.existingEntryData.EntryData_Id ?? 0,
                        ItemNumber = ((string)e.ItemNumber).Truncate(20),
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
                        InvoiceQty = data.existingEntryData.EntryType == "ADJ" && e.InvoiceQty == 0 && e.ReceivedQty == 0
                            ? e.Quantity
                            : e.InvoiceQty,
                        CNumber = string.IsNullOrEmpty(e.CNumber) ? null : e.CNumber,
                        CLineNumber = e.CLineNumber,
                        PreviousInvoiceNumber = string.IsNullOrEmpty(e.PreviousInvoiceNumber)
                            ? null
                            : e.PreviousInvoiceNumber,
                        Comment = string.IsNullOrEmpty(e.Comment) ? null : e.Comment,
                        FileLineNumber = e.FileLineNumber ?? lineNumber,
                        LineNumber = e.EntryDataDetailsId == 0 ? lineNumber : e.LineNumber,
                        EffectiveDate = data.existingEntryData.EntryType == "ADJ"
                            ? e.EffectiveDate ?? data.existingEntryData.EntryDataDate
                            : e.EffectiveDate,
                        TaxAmount = e.TaxAmount,
                        VolumeLiters = e.VolumeLiters,
                    });
                }

                ctx.SaveChanges();
                if (!overWriteExisting && data.existingEntryData != null)
                {
                    //update entrydatadetails
                    foreach (var itm in data.details)
                    {
                        var d = data.details.FirstOrDefault(z =>
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
                    AddToDocSet(docSet, data.existingEntryData);
                    await UpdateEntryData(data.existingEntryData).ConfigureAwait(false).ConfigureAwait(false);
                }
            }
        }

        private async Task<EntryData> GetNewEntryData(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            int applicationSettingsId = item.EntryData.ApplicationSettingsId;
            string entryDataId = item.EntryData.EntryDataId;
            EntryData entryData = null;
            switch (fileType.FileImporterInfos.EntryType)
            {
                case FileTypeManager.EntryTypes.Sales:


                    entryData = await CreateSales(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Inv:
                    entryData = await CreateInvoice(docSet, item, entryDataId, applicationSettingsId).ConfigureAwait(false);

                    break;

                case FileTypeManager.EntryTypes.Po:
                case FileTypeManager.EntryTypes.ShipmentInvoice:
                    entryData = await CreatePO(docSet, item, entryDataId, applicationSettingsId).ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Ops:
                    entryData = await CreateOPS(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Adj:
                    entryData = await CreateADJ(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Dis:
                    entryData = await CreateDIS(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Rcon:
                    entryData = await CreateRCON(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);
                    break;
                default:
                    throw new ApplicationException("Unknown FileType");
            }

            return entryData;
        }

        private async Task<EntryData> CreateDIS(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            var EDdis = new Adjustments(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "DIS",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                SupplierCode = item.EntryData.Supplier,
                TrackingState = TrackingState.Added,
                TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
                EmailId = item.EntryData.EmailId,
                FileTypeId = item.EntryData.FileTypeId,
                SourceFile = item.EntryData.SourceFile,
                Vendor = item.EntryData.Vendor,
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
            AddToDocSet(docSet, EDdis);
            entryData = await CreateAdjustments(EDdis).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreateRCON(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            var EDrcon = new Adjustments(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "RCON",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                TrackingState = TrackingState.Added,
                TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
                EmailId = item.EntryData.EmailId,
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
            AddToDocSet(docSet, EDrcon);
            entryData = await CreateAdjustments(EDrcon).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreateADJ(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            var EDadj = new Adjustments(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "ADJ",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                TrackingState = TrackingState.Added,
                SupplierCode = item.EntryData.Supplier,
                TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),
                EmailId = item.EntryData.EmailId,
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
            AddToDocSet(docSet, EDadj);
            entryData = await CreateAdjustments(EDadj).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreateOPS(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            var EDops = new OpeningStock(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
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
                EmailId = item.EntryData.EmailId,
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
            AddToDocSet(docSet, EDops);
            entryData = await CreateOpeningStock(EDops).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreatePO(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, string entryDataId, int applicationSettingsId)
        {
            EntryData entryData;
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                Math.Abs((double)item.f.Sum(x => x.InvoiceTotal)) < .001)
                throw new ApplicationException(
                    $"{entryDataId} has no Invoice Total. Please check File.");


            var EDpo = new PurchaseOrders(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
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

                EmailId = item.EntryData.EmailId,
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
                EDpo.WarehouseInfo.Add(new WarehouseInfo()
                {
                    WarehouseNo = warehouseNo.WarehouseNo,
                    Packages = warehouseNo.Packages,
                    EntryData_PurchaseOrders = EDpo,
                    TrackingState = TrackingState.Added
                });
            }

            if (!string.IsNullOrEmpty(item.EntryData.DocumentType))
                EDpo.DocumentType = new EDDocumentTypes(true)
                {
                    DocumentType = item.EntryData.DocumentType,
                    TrackingState = TrackingState.Added
                };
            AddToDocSet(docSet, EDpo);
            entryData = await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreateInvoice(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, string entryDataId, int applicationSettingsId)
        {
            EntryData entryData;
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                Math.Abs((double)item.f.Sum(x => x.InvoiceTotal)) < .001)
                throw new ApplicationException(
                    $"{entryDataId} has no Invoice Total. Please check File.");
            var EDinv = new Invoices(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "INV",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                SupplierCode = item.EntryData.Supplier,
                TrackingState = TrackingState.Added,
                TotalFreight = item.f.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.f.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.f.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.f.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.f.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.f.Sum(x => (double)x.TotalDeductions),
                Packages = item.f.Sum(x => x.Packages),
                InvoiceTotal = item.f.Sum(x => (double)x.InvoiceTotal),

                EmailId = item.EntryData.EmailId,
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
            AddToDocSet(docSet, EDinv);
            entryData = await CreateInvoice(EDinv).ConfigureAwait(false);
            return entryData;
        }

        private async Task<EntryData> CreateSales(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            var EDsale = new Sales(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "Sales",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                INVNumber = entryDataId,
                CustomerName = item.EntryData.CustomerName,
                EmailId = item.EntryData.EmailId,
                FileTypeId = item.EntryData.FileTypeId,
                Tax = item.EntryData.Tax, //item.f.Sum(x => x.TotalTax),
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


            AddToDocSet(docSet, EDsale);


            entryData = await CreateSales(EDsale).ConfigureAwait(false);
            return entryData;
        }

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetDataToUpdate(List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            string entryDataId;
            List<EntryDataDetails> details = new List<EntryDataDetails>();


            List<EntryData> existingEntryDataList = GetExistingEntryData(item.EntryData.EntryDataId, item.EntryData.ApplicationSettingsId);

            var existingEntryData = overWriteExisting ? null : existingEntryDataList.FirstOrDefault();
            if (existingEntryDataList.Any())
            {
                if (overWriteExisting)
                {
                    await DeleteExistingEntryData(existingEntryDataList).ConfigureAwait(false);

                    details = item.EntryDataDetails.ToList();
                }
                else
                {
                    details = LoadExistingDetails(docSet, existingEntryData.EntryDataDetails, item.EntryDataDetails.ToList());
                }
            }
            else
            {
                details = item.EntryDataDetails.ToList();
            }

            return (existingEntryData, details);
        }

        private static void UpdateInventoryItems(
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            int applicationSettingsId = item.EntryData.ApplicationSettingsId;
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var e in item.InventoryItems
                             .Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber &&
                                         x.ItemAlias != null).ToList())
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
                            string aliasName = ((string)e.ItemAlias).Truncate(20);
                            var aliasItm = ctx.InventoryItems
                                .FirstOrDefault(x => x.ApplicationSettingsId == applicationSettingsId &&
                                                     x.ItemNumber == aliasName);
                            if (aliasItm == null)
                                throw new ApplicationException(
                                    $"No Alias Inventory Item Found... need to add it before creating Alias {aliasName} for InventoryItem {inventoryItem.ItemNumber}");

                            inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
                            {
                                InventoryItemId = inventoryItem.Id,
                                AliasName = aliasName,
                                AliasItemId = aliasItm.Id,
                            });
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }

        private async Task DeleteExistingEntryData(List<EntryData> existingEntryDataList)
        {
            foreach (var itm in existingEntryDataList)
            {
                await ClearEntryDataDetails(itm).ConfigureAwait(false);
                await DeleteEntryData(itm).ConfigureAwait(false);
            }
        }

        private static List<EntryDataDetails> LoadExistingDetails(List<AsycudaDocumentSet> docSet, List<EntryDataDetails> oldDetails, List<EntryDataDetails> newDetails)
        {
            var details = new List<EntryDataDetails>();
            foreach (var doc in docSet)
            {
                var l = 0;
                foreach (var newEntryDataDetails in newDetails)
                {
                    l += 1;

                    var oldEntryDataDetails = GetOldEntryDataDetails(oldDetails, l, newEntryDataDetails, doc);

                    
                    if (oldEntryDataDetails != null && EntryDataDetailsMatch(newEntryDataDetails, oldEntryDataDetails))
                        continue;


                    
                    if (!DetailsContainsNewEntryDataDetails(details, newEntryDataDetails)) details.Add(newEntryDataDetails);

                    if (oldEntryDataDetails != null) new EntryDataDetailsService().DeleteEntryDataDetails(oldEntryDataDetails.EntryDataDetailsId.ToString()).Wait();
                }
            }

            return details;
        }

        private static bool DetailsContainsNewEntryDataDetails(List<EntryDataDetails> details, EntryDataDetails newEntryDataDetails)
        {
            return details.FirstOrDefault(x =>
                       x.ItemNumber == newEntryDataDetails.ItemNumber && x.LineNumber == newEntryDataDetails.LineNumber) != null;
        }

        private static bool EntryDataDetailsMatch(EntryDataDetails newEntryDataDetails, EntryDataDetails oldEntryDataDetails)
        {
            return (Math.Abs(newEntryDataDetails.Quantity - oldEntryDataDetails.Quantity) < .0001 &&
                    Math.Abs(newEntryDataDetails.Cost - oldEntryDataDetails.Cost) < .0001);
        }

        private static EntryDataDetails GetOldEntryDataDetails(List<EntryDataDetails> existingEntryDataDetails, int l,
            EntryDataDetails newEntryDataDetails, AsycudaDocumentSet doc)
        {
            var oldEntryDataDetails = existingEntryDataDetails.FirstOrDefault(x =>
                x.LineNumber == l && x.ItemNumber == newEntryDataDetails.ItemNumber &&
                x.EntryData.AsycudaDocumentSets.Any(z =>
                    z.AsycudaDocumentSetId == doc.AsycudaDocumentSetId));
            return oldEntryDataDetails;
        }

        private static List<EntryData> GetExistingEntryData(string entryDataId, int applicationSettingsId)
        {
            var oldeds = new EntryDataDSContext().EntryData
                .Include("AsycudaDocumentSets")
                .Include("EntryDataDetails")
                .Where(x => x.EntryDataId == entryDataId
                            && x.ApplicationSettingsId == applicationSettingsId)
                // this was to prevent deleting entrydata from other folders discrepancy with piece here and there with same entry data. but i changed the discrepancy to work with only one folder.
                //.Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any())
                .ToList();
            return oldeds;
        }

        // new List<IDictionary<string, object>>(){(IDictionary<string, object>) header
        private List<((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)> GetRawEntryData(FileTypes fileType, List<dynamic> eslst, List<AsycudaDocumentSet> docSet, string emailId, string droppedFilePath)
        {
            var ed = eslst.Select(x => (dynamic)x)
                .GroupBy(es => (es.EntryDataId, es.EntryDataDate, es.CustomerName ))
                .Select(g =>(
                    EntryData : (
                        g.Key.EntryDataId,
                        g.Key.EntryDataDate,
                        AsycudaDocumentSetId :
                            docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.AsycudaDocumentSetId ??
                            docSet.First().AsycudaDocumentSetId,
                        ApplicationSettingsId :
                            docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.ApplicationSettingsId ??
                            docSet.First().ApplicationSettingsId,
                        g.Key.CustomerName,
                        ((dynamic)g.FirstOrDefault())?.Tax,
                        Supplier :
                            string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                                ? null
                                : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                        EmailId : emailId,
                        FileTypeId : fileType.Id,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))
                            ?.FinancialInformation,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Vendor != ""))?.Vendor,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                        SourceFile : droppedFilePath
                    ),
                    EntryDataDetails : g.Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .Select(x => new EntryDataDetails()
                        {
                            EntryDataId = x.EntryDataId,
                            //Can't set entrydata_id here cuz this is from data
                            ItemNumber = ((string)x.ItemNumber.ToUpper()).Truncate(20),
                            ItemDescription = x.ItemDescription,
                            Cost = x.Cost ?? 0,
                            TotalCost = Convert.ToDouble((double)(x.TotalCost ?? 0.0)),
                            Quantity = Convert.ToDouble((double)(x.Quantity ?? 0.0)),
                            FileLineNumber = x.LineNumber,
                            Units = x.Units,
                            Freight = Convert.ToDouble((double)(x.Freight ?? 0.0)),
                            Weight = Convert.ToDouble((double)(x.Weight ?? 0.0)),
                            InternalFreight = Convert.ToDouble((double)(x.InternalFreight ?? 0.0)),
                            InvoiceQty = Convert.ToDouble((double)(x.InvoiceQuantity ?? 0.0)),
                            ReceivedQty = Convert.ToDouble((double)(x.ReceivedQuantity ?? 0.0)),
                            TaxAmount = x.Tax ?? 0,
                            CNumber = x.PreviousCNumber,
                            CLineNumber = (int?)x.PreviousCLineNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            Comment = x.Comment,
                            InventoryItemId = x.InventoryItemId ?? 0,
                            EffectiveDate = x.EffectiveDate,
                            VolumeLiters =
                                Convert.ToDouble(x.Gallons * GalToLtrRate ??
                                                 Convert.ToDouble((double)(x.Liters ?? 0.0))),
                        }),
                    f : g.Select(x => (
                        TotalWeight : Convert.ToDouble((double)(x.TotalWeight ?? 0.0)),
                        TotalFreight : Convert.ToDouble((double)(x.TotalFreight ?? 0.0)),
                        TotalInternalFreight : Convert.ToDouble((double)(x.TotalInternalFreight ?? 0.0)),
                        TotalOtherCost : Convert.ToDouble((double)(x.TotalOtherCost ?? 0.0)),
                        TotalInsurance : Convert.ToDouble((double)(x.TotalInsurance ?? 0.0)),
                        TotalDeductions : Convert.ToDouble((double)(x.TotalDeductions ?? 0.0)),
                        InvoiceTotal : Convert.ToDouble((double)(x.InvoiceTotal ?? 0.0)),
                        TotalTax : Convert.ToDouble((double)(x.TotalTax ?? 0.0)),
                        Packages : Convert.ToInt32((int)(x.Packages ?? 0)),
                        x.WarehouseNo
                    )),
                    InventoryItems : g.DistinctBy(x => ( x.ItemNumber, x.ItemAlias ))
                        .Select(x => (x.ItemNumber, x.ItemAlias))
                )).ToList();

          
            return ed;
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






  







       
        private async Task ImportSuppliers(List<dynamic> eslst, int applicationSettingsId, FileTypes fileType)
        {
            try
            {
               
                var itmlst = eslst
                    .GroupBy(x => new {x.SupplierCode, x.SupplierName, x.SupplierAddress, x.CountryCode})
                    .ToList();

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true && fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Po)
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

        private async Task ImportInventory(List<dynamic> data, int applicationSettingsId, FileTypes fileType)
        {
            try
            {



                var itmlst = InventoryItemDataUtils.CreateItemGroupList(data);


                 var inventorySource = GetInventorySource(fileType);

                 ProcessInventoryItemLst(applicationSettingsId, itmlst, inventorySource);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void ProcessInventoryItemLst(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {

            var data = InventoryItemDataUtils.GetInventoryItemFromData(applicationSettingsId, inventoryDataList, inventorySource);

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .ForEach(x => x.Item.TariffCode = x.Data.Key.TariffCode);

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Data.Data.ForEach(z => z.ItemDescription = x.Item.Description));

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Item.Description = x.Data.Key.ItemDescription);


            data.existingInventoryItem
                .Where(i => i.Item.InventoryItemSources.All(x => x.InventorySource.Id != inventorySource.Id))
                .ForEach(x =>
                    x.Item.InventoryItemSources.Add(CreateItemSource(inventorySource, x.Item)));

            InventoryItemDataUtils.SaveInventoryItems(data.existingInventoryItem);
            ///////////////

            data.existingInventoryItem.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            data.existingInventoryItem
                .Select(x => (DataItem: x, Code: GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


            data.existingInventoryItem
                .Select(x => (DataItem: x, Code: GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


           

          


            data.newInventoryItems.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            data.newInventoryItems
                .Select(x => (DataItem: x, Code: GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


            data.newInventoryItems
                .Select(x => (DataItem: x, Code: GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));

        }

        



        private static void SaveInventoryCodes(int applicationSettingsId, InventorySource inventorySource,
            List<(string SupplierItemNumber, string SupplierItemDescription)> itemCodes,
            InventoryItem i)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var invItemCode in itemCodes)
                {


                    var supplierItemNumber = invItemCode.SupplierItemNumber.ToString();
                    var invItem = ctx.InventoryItems.FirstOrDefault(x =>
                        x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber == supplierItemNumber);
                    if (invItem == null)
                    {
                        invItem = new InventoryItem(true)
                        {
                            ApplicationSettingsId = applicationSettingsId,
                            Description = invItemCode.SupplierItemDescription,
                            ItemNumber = ((string)invItemCode.SupplierItemNumber).Truncate(20),
                            InventoryItemSources = new List<InventoryItemSource>()
                            {
                                new InventoryItemSource(true)
                                {
                                    InventorySourceId = inventorySource.Id,
                                    TrackingState = TrackingState.Added
                                }
                            },
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
                            AliasName = ((string)supplierItemNumber).Truncate(20),
                            AliasItemId = invItem.Id,
                            AliasId = invItem.Id,
                            TrackingState = TrackingState.Added
                        });
                    }

                }

                ctx.SaveChanges();
            }
        }

        private static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryAliasCodes(InventoryData item, InventoryItem i)
        {
            var AliasItemCodes =
                item.Data.Where(x => !string.IsNullOrEmpty(x.ItemAlias))
                    .Select(x => (
                        SupplierItemNumber: (string)x.ItemAlias.ToString(),
                        SupplierItemDescription: (string)x.ItemDescription
                    ))
                    .Where(x => !string.IsNullOrEmpty(x.SupplierItemNumber) &&
                                i.ItemNumber != x.SupplierItemNumber)
                    .DistinctBy(x => x.SupplierItemNumber)
                    .ToList();
            return AliasItemCodes;
        }

        private static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryItemCodes(InventoryData item, InventoryItem i)
        {
            var invItemCodes = item.Data
                .Select(x => (
                    SupplierItemNumber: (string)x.SupplierItemNumber,
                    SupplierItemDescription: (string)x.SupplierItemDescription
                ))
                .Where(x => !string.IsNullOrEmpty(x.SupplierItemNumber) && i.ItemNumber != x.SupplierItemNumber)
                .DistinctBy(x => x.SupplierItemNumber)
                .ToList();
            return invItemCodes;
        }

     

       

     
       


      

        private static InventoryItemSource CreateItemSource(InventorySource inventorySource, InventoryItem i)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                var inventoryItemSource = new InventoryItemSource(true)
                {
                    InventorySourceId = inventorySource.Id,
                    TrackingState = TrackingState.Added,
                    InventoryId = i.Id,
                };
                ctx.InventoryItemSources.Add(inventoryItemSource);
                ctx.SaveChanges();
                return inventoryItemSource;
            }
        }

        private static InventorySource GetInventorySource(FileTypes fileType)
        {
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType.FileImporterInfos.EntryType)
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

            if (inventorySource == null)
                throw new ApplicationException($"No Inventory source setup for FileType:{fileType.FileImporterInfos.EntryType}");
            return inventorySource;
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

    internal class ImportData
    {
        public IDictionary<string, object> res { get; set; }
        public Dictionary<string, int> mapping { get; set; }
        public string[] splits { get; set; }
    }