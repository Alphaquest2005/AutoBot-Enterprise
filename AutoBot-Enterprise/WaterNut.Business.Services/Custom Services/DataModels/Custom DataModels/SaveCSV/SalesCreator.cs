using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Data;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;


namespace WaterNut.DataSpace
{
    public class SalesCreator: IEntryDataCreator
    {
       
        public async Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item,
            int applicationSettingsId, string entryDataId)
        {
            dynamic EDsale = Create(docSet, item, applicationSettingsId, entryDataId);
             EntryData entryData = await SaveSales(EDsale).ConfigureAwait(false);
            return entryData;
        }

        public EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, int applicationSettingsId, string entryDataId)
        {
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
                InvoiceTotal = item.Totals.Sum(x => (double)x.InvoiceTotal),
                SourceFile = item.EntryData.SourceFile,
                TrackingState = TrackingState.Added,
            };
            if (item.EntryData.DocumentType != "")
                EDsale.DocumentType = new EDDocumentTypes(true)
                {
                    DocumentType = item.EntryData.DocumentType,
                    TrackingState = TrackingState.Added
                };


            new AddToDocSetSelector().Execute(docSet, EDsale);
            return EDsale;
        }

        private async Task<Sales> SaveSales(Sales EDsale)
        {
            using (var ctx = new SalesService())
            {
                return await ctx.CreateSales(EDsale).ConfigureAwait(false);
            }
        }
    }
}