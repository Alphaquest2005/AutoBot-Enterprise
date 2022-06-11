using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class EntryDataImporter
    {
        public dynamic GalToLtrRate = 3.785;


        public async Task<bool> ImportEntryData(DataFile dataFile)
        {
            try
            {
                if (dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Sales &&
                    !(((IDictionary<string, object>)dataFile.Data.First()).ContainsKey("Tax") ||
                      ((IDictionary<string, object>)dataFile.Data.First()).ContainsKey("TotalTax")))
                    throw new ApplicationException("Sales file dose not contain Tax");


                var ed = GetRawEntryData(dataFile.FileType, dataFile.Data, dataFile.DocSet, dataFile.EmailId, dataFile.DroppedFilePath);

                if (ed == null) return true;

                //Parallel.ForEach(ed, new ParallelOptions() { MaxDegreeOfParallelism = 3 },//Environment.ProcessorCount * 1
                //    async item =>
                foreach (var item in Enumerable
                             .Where<((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int
                                 ApplicationSettingsId, dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic
                                 Currency, string EmailId, int FileTypeId, dynamic DocumentType, dynamic
                                 SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                                 Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
                                 EntryDataDetails,
                                 IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight,
                                     double TotalOtherCost, double TotalInsurance, double TotalDeductions, double
                                     InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f,
                                 IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)>(ed, x =>
                                 x.EntryData.EntryDataId != null && x.EntryData.EntryDataDate != null))

                {
                    await SaveEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, dataFile.EmailId, item).ConfigureAwait(false);

                    UpdateInventoryItems(item);
                } //);


                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SaveEntryData(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            string emailId,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            string entryDataId = item.EntryData.EntryDataId;
            if (!item.EntryDataDetails.Any())
                throw new ApplicationException(entryDataId + " has no details");

            var data = await GetDataToUpDate(fileType, docSet, overWriteExisting, item).ConfigureAwait(false);


            await SaveEntryDataDetails(docSet, overWriteExisting, data).ConfigureAwait(false);
        }

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetDataToUpDate(
            FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
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
                        InvoiceQty = data.existingEntryData.EntryType == "ADJ" && e.InvoiceQty == 0 &&
                                     e.ReceivedQty == 0
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            int applicationSettingsId = item.EntryData.ApplicationSettingsId;
            string entryDataId = item.EntryData.EntryDataId;
            EntryData entryData = null;
            switch (fileType.FileImporterInfos.EntryType)
            {
                case FileTypeManager.EntryTypes.Sales:


                    entryData = await CreateSales(docSet, item, applicationSettingsId, entryDataId)
                        .ConfigureAwait(false);
                    break;
                case FileTypeManager.EntryTypes.Inv:
                    entryData = await CreateInvoice(docSet, item, entryDataId, applicationSettingsId)
                        .ConfigureAwait(false);

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
                    entryData = await CreateRCON(docSet, item, applicationSettingsId, entryDataId)
                        .ConfigureAwait(false);
                    break;
                default:
                    throw new ApplicationException("Unknown FileType");
            }

            return entryData;
        }

        private async Task<EntryData> CreateDIS(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            int applicationSettingsId, string entryDataId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            int applicationSettingsId, string entryDataId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            int applicationSettingsId, string entryDataId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            int applicationSettingsId, string entryDataId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            string entryDataId, int applicationSettingsId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            string entryDataId, int applicationSettingsId)
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item,
            int applicationSettingsId, string entryDataId)
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

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetDataToUpdate(
            List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            string entryDataId;
            List<EntryDataDetails> details = new List<EntryDataDetails>();


            List<EntryData> existingEntryDataList =
                GetExistingEntryData(item.EntryData.EntryDataId, item.EntryData.ApplicationSettingsId);

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
                    details = LoadExistingDetails(docSet, existingEntryData.EntryDataDetails,
                        item.EntryDataDetails.ToList());
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
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
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

        private static List<EntryDataDetails> LoadExistingDetails(List<AsycudaDocumentSet> docSet,
            List<EntryDataDetails> oldDetails, List<EntryDataDetails> newDetails)
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


                    if (!DetailsContainsNewEntryDataDetails(details, newEntryDataDetails))
                        details.Add(newEntryDataDetails);

                    if (oldEntryDataDetails != null)
                        new EntryDataDetailsService()
                            .DeleteEntryDataDetails(oldEntryDataDetails.EntryDataDetailsId.ToString()).Wait();
                }
            }

            return details;
        }

        private static bool DetailsContainsNewEntryDataDetails(List<EntryDataDetails> details,
            EntryDataDetails newEntryDataDetails)
        {
            return details.FirstOrDefault(x =>
                       x.ItemNumber == newEntryDataDetails.ItemNumber &&
                       x.LineNumber == newEntryDataDetails.LineNumber) !=
                   null;
        }

        private static bool EntryDataDetailsMatch(EntryDataDetails newEntryDataDetails,
            EntryDataDetails oldEntryDataDetails)
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

        private List<((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId,
                dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId,
                dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation,
                dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
                EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    ,
                    double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)>
            GetRawEntryData(FileTypes fileType, List<dynamic> eslst, List<AsycudaDocumentSet> docSet, string emailId,
                string droppedFilePath)
        {
            var ed = eslst.Select(x => (dynamic)x)
                .GroupBy(es => (es.EntryDataId, es.EntryDataDate, es.CustomerName))
                .Select(g => (
                    EntryData: (
                        g.Key.EntryDataId,
                        g.Key.EntryDataDate,
                        AsycudaDocumentSetId:
                        docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.AsycudaDocumentSetId ??
                        docSet.First().AsycudaDocumentSetId,
                        ApplicationSettingsId:
                        docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.ApplicationSettingsId ??
                        docSet.First().ApplicationSettingsId,
                        g.Key.CustomerName,
                        ((dynamic)g.FirstOrDefault())?.Tax,
                        Supplier:
                        string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                            ? null
                            : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                        EmailId: emailId,
                        FileTypeId: fileType.Id,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))
                        ?.FinancialInformation,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Vendor != ""))?.Vendor,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                        SourceFile: droppedFilePath
                    ),
                    EntryDataDetails: g.Where(x => !string.IsNullOrEmpty(x.ItemNumber))
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
                                Convert.ToDouble((double)(x.Gallons * GalToLtrRate ??
                                                          Convert.ToDouble((double)(x.Liters ?? 0.0)))),
                        }),
                    f: g.Select(x => (
                        TotalWeight: Convert.ToDouble((double)(x.TotalWeight ?? 0.0)),
                        TotalFreight: Convert.ToDouble((double)(x.TotalFreight ?? 0.0)),
                        TotalInternalFreight: Convert.ToDouble((double)(x.TotalInternalFreight ?? 0.0)),
                        TotalOtherCost: Convert.ToDouble((double)(x.TotalOtherCost ?? 0.0)),
                        TotalInsurance: Convert.ToDouble((double)(x.TotalInsurance ?? 0.0)),
                        TotalDeductions: Convert.ToDouble((double)(x.TotalDeductions ?? 0.0)),
                        InvoiceTotal: Convert.ToDouble((double)(x.InvoiceTotal ?? 0.0)),
                        TotalTax: Convert.ToDouble((double)(x.TotalTax ?? 0.0)),
                        Packages: Convert.ToInt32((int)(x.Packages ?? 0)),
                        x.WarehouseNo
                    )),
                    InventoryItems: g.DistinctBy(x => (x.ItemNumber, x.ItemAlias))
                        .Select(x => (x.ItemNumber, x.ItemAlias))
                )).ToList();


            return ed;
        }

        private static void AddToDocSet(List<AsycudaDocumentSet> docSet, EntryData entryData)
        {
            foreach (var doc in docSet.DistinctBy(x => x.AsycudaDocumentSetId))
            {
                if ((new EntryDataDSContext()).AsycudaDocumentSetEntryData.FirstOrDefault(
                        x => x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId &&
                             x.EntryData_Id == entryData.EntryData_Id) != null) continue;
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

namespace WaterNut.DataSpace
{
}