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
    public class InvoiceCreator : IEntryDataCreator
    {
        public async Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            dynamic EDinv = Create(docSet, item, applicationSettingsId, entryDataId);
            entryData = await CreateInvoice(EDinv).ConfigureAwait(false);
            return entryData;
        }

        public EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true &&
                Math.Abs((double)item.Totals.Sum(x => x.InvoiceTotal)) < .001)
                throw new ApplicationException(
                    $"{entryDataId} has no Template Total. Please check File.");
            var EDinv = new Invoices(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = "INV",
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                SupplierCode = item.EntryData.Supplier,
                TrackingState = TrackingState.Added,
                TotalFreight = item.Totals.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.Totals.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.Totals.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.Totals.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.Totals.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.Totals.Sum(x => (double)x.TotalDeductions),
                Packages = item.Totals.Sum(x => x.Packages),
                InvoiceTotal = item.Totals.Sum(x => (double)x.InvoiceTotal),

                EmailId = item.EntryData.EmailId,
                FileTypeId = item.EntryData.FileTypeId,
                SourceFile = item.EntryData.SourceFile,
                Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                    ? null
                    : item.EntryData.Currency,
                PONumber = string.IsNullOrEmpty(item.EntryData.EntryDataId)
                    ? null
                    : item.EntryData.EntryDataId,
            };
            if (!string.IsNullOrEmpty(item.EntryData.DocumentType))
                EDinv.DocumentType = new EDDocumentTypes(true)
                {
                    DocumentType = item.EntryData.DocumentType,
                    TrackingState = TrackingState.Added
                };
            new AddToDocSetSelector().Execute(docSet, EDinv);
            return EDinv;
        }

        private async Task<Invoices> CreateInvoice(Invoices EDinv)
        {
            using (var ctx = new InvoicesService())
            {
                return await ctx.CreateInvoices(EDinv).ConfigureAwait(false);
            }
        }


 
    }
}