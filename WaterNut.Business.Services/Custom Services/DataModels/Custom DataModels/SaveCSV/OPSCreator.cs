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
    public class OPSCreator: IEntryDataCreator
    {
        public async Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item,
            int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            dynamic EDops = Create(docSet, item, applicationSettingsId, entryDataId);
            entryData = await CreateOpeningStock(EDops).ConfigureAwait(false);
            return entryData;
        }

        public EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            var EDops = new OpeningStock(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                OPSNumber = entryDataId,
                EntryType = "OPS",
                TrackingState = TrackingState.Added,
                TotalFreight = item.Totals.Sum(x => (double)x.TotalFreight),
                TotalInternalFreight = item.Totals.Sum(x => (double)x.TotalInternalFreight),
                TotalWeight = item.Totals.Sum(x => (double)x.TotalWeight),
                TotalOtherCost = item.Totals.Sum(x => (double)x.TotalOtherCost),
                TotalInsurance = item.Totals.Sum(x => (double)x.TotalInsurance),
                TotalDeduction = item.Totals.Sum(x => (double)x.TotalDeductions),
                InvoiceTotal = item.Totals.Sum(x => (double)x.InvoiceTotal),
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
            new AddToDocSetSelector().Execute(docSet, EDops);
            return EDops;
        }

        private async Task<OpeningStock> CreateOpeningStock(OpeningStock EDops)
        {
            using (var ctx = new OpeningStockService())
            {
                return await ctx.CreateOpeningStock(EDops).ConfigureAwait(false);
            }
        }
    }
}