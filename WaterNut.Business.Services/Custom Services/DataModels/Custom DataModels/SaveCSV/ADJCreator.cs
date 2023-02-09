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
    public class ADJCreator : IEntryDataCreator
    {
        public string EntryDataType { get; }

        public ADJCreator(string entryDataType)
        {
            EntryDataType = entryDataType;
        }

        public async Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item,
            int applicationSettingsId, string entryDataId)
        {
            EntryData entryData;
            dynamic EDadj = Create(docSet, item, applicationSettingsId, entryDataId);
            entryData = await CreateAdjustments(EDadj).ConfigureAwait(false);
            new AddToDocSetSelector().Execute(docSet, EDadj);
            return entryData;
        }

        public EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
            var EDadj = new Adjustments(true)
            {
                ApplicationSettingsId = applicationSettingsId,
                EntryDataId = entryDataId,
                EntryType = EntryDataType,
                EntryDataDate = (DateTime)item.EntryData.EntryDataDate,
                TrackingState = TrackingState.Added,
                SupplierCode = item.EntryData.Supplier,
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
                Type = EntryDataType
            };
            if (item.EntryData.DocumentType != "")
                EDadj.DocumentType = new EDDocumentTypes(true)
                {
                    DocumentType = item.EntryData.DocumentType,
                    TrackingState = TrackingState.Added
                };
            
            return EDadj;
        }

        private async Task<Adjustments> CreateAdjustments(Adjustments eDadj)
        {
            using (var ctx = new AdjustmentsService())
            {
                return await ctx.CreateAdjustments(eDadj).ConfigureAwait(false);
            }
        }
    }
}