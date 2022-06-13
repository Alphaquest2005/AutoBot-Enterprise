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
    public class OPSCreator: IEntryDataCreator
    {
        public async Task<EntryData> Create(List<AsycudaDocumentSet> docSet,
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
            EntryDataDetailsCreator.AddToDocSet(docSet, EDops);
            entryData = await CreateOpeningStock(EDops).ConfigureAwait(false);
            return entryData;
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