using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;

namespace WaterNut.DataSpace
{
    public class POCreator: IEntryDataCreator
    {
        public async Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            dynamic EDpo = Create(docSet, item, applicationSettingsId, entryDataId);
            EntryData entryData = await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            return entryData;
        }

        public EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                Math.Abs((double)item.Totals.Sum(x => x.InvoiceTotal)) < .001)
                throw new ApplicationException(
                    $"{entryDataId} has no Template Total. Please check File.");


            var EDpo = new PurchaseOrders(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryDataDate = (DateTime)(item.EntryData.EntryDataDate ?? DateTime.Now),
                PONumber = entryDataId,
                EntryType = "PO",
                SupplierCode = item.EntryData.Supplier,
                TrackingState = TrackingState.Added,


                TotalFreight = item.Totals.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.Totals.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.Totals.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.Totals.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.Totals.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.Totals.Sum(x => (double)x.TotalDeductions),
                InvoiceTotal = item.Totals.Sum(x => (double)x.InvoiceTotal),
                Packages = item.Totals.Sum(x => (int)x.Packages),

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
            foreach (var warehouseNo in item.Totals.Where(x => !string.IsNullOrEmpty(x.WarehouseNo)))
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
            new AddToDocSetSelector().Execute(docSet, EDpo);
            return EDpo;
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