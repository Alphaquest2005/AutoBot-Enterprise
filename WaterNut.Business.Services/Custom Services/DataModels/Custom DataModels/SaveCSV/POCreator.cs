using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    public class POCreator: IEntryDataCreator
    {
        public async Task<EntryData> Create(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId)
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
            EntryDataDetailsCreator.AddToDocSet(docSet, EDpo);
            entryData = await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            return entryData;
        }

        private async Task<PurchaseOrders> CreatePurchaseOrders(PurchaseOrders EDpo)
        {
            using (var ctx = new PurchaseOrdersService())
            {
                return await ctx.CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            }
        }

     
    }
}