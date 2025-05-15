using System;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot;

using Serilog;

public partial class EX9Utils
{
    public static async Task Ex9AllAllocatedSales(bool overwrite, ILogger log)
    {
        try
        {
            Console.WriteLine("Ex9 All Allocated Sales");

            //var saleInfo = BaseDataModel.CurrentSalesInfo();
            //if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;


            var fdocSet =
                new CoreEntitiesContext().AsycudaDocumentSetExs.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .First();

            var docset = await BaseDataModel.Instance.GetAsycudaDocumentSet(fdocSet.AsycudaDocumentSetId)
                .ConfigureAwait(false);
            if (overwrite)
            {
                await BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId, log).ConfigureAwait(false);
                BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId,
                    0); // don't overwrite previous entries
            }


            var filterExpression =
                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                $"&& (InvoiceDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate:MM/01/yyyy}\" " +
                $" && InvoiceDate <= \"{DateTime.Now:MM/dd/yyyy HH:mm:ss}\")" +
                //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                "&& (TaxAmount == 0 || TaxAmount != 0)" +
                // "&& (ItemNumber == \"WA99004\")" +//A002416,A002402,X35019044,AB111510
                // "&&  PreviousItem_Id == 388376" +
                "&& PreviousItem_Id != null" +
                //"&& (xBond_Item_Id == 0)" + not relevant because it could be assigned to another sale but not exwarehoused
                "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                "&& (PiQuantity < pQtyAllocated)" +

                //////////// I left this because i only want to use interface to remove All ALLOCATED 
                //"&& (pQuantity > PiQuantity)" +
                //"&& (pQuantity - pQtyAllocated  < 0.001)" + // prevents spill over allocations
                "&& (Status == null || Status == \"\")" +
                (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                    : "") +
                ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");


            await AllocationsModel.Instance.CreateEx9.Execute(filterExpression, false, false, false, docset, "Sales",
                "Historic", true, true, true, false, false, false, true, true, true, false).ConfigureAwait(false);
            //await CreateDutyFreePaidDocument(dfp, res, docSet, "Sales", true,
            //        itemSalesPiSummarylst.Where(x => x.DutyFreePaid == dfp || x.DutyFreePaid == "All")
            //            .ToList(), true, true, true, "Historic", true, ApplyCurrentChecks,
            //        true, false, true)
            //    .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}