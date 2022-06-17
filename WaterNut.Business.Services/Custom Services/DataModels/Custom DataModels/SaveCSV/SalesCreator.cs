using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Data;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;


namespace WaterNut.DataSpace
{
    public class SalesCreator: IEntryDataCreator
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


            EntryDataDetailsCreator.AddToDocSet(docSet, EDsale);


            entryData = await CreateSales(EDsale).ConfigureAwait(false);
            return entryData;
        }

        private async Task<Sales> CreateSales(Sales EDsale)
        {
            using (var ctx = new SalesService())
            {
                return await ctx.CreateSales(EDsale).ConfigureAwait(false);
            }
        }
    }
}