using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AllocationQS.Business.Entities; // Assuming AllocationsModel is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void Ex9AllAllocatedSales(bool overwrite)
        {
            try
            {
                Console.WriteLine("Ex9 All Allocated Sales");

                //var saleInfo = BaseDataModel.CurrentSalesInfo();
                //if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;

                var fdocSet =
                    new CoreEntitiesContext().AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .First(); // InvalidOperationException if no match

                var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(fdocSet.AsycudaDocumentSetId).Result; // Assuming GetAsycudaDocumentSet exists
                if (overwrite)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait(); // Assuming ClearAsycudaDocumentSet exists
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0);// don't overwrite previous entries // Assuming UpdateAsycudaDocumentSetLastNumber exists
                }

                var filterExpression =
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                    $"&& (InvoiceDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate:MM/01/yyyy}\" " + // Assuming OpeningStockDate exists
                    $" && InvoiceDate <= \"{DateTime.Now:MM/dd/yyyy HH:mm:ss}\")" +
                    //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                    "&& (TaxAmount == 0 || TaxAmount != 0)" +
                    // "&& (ItemNumber == \"WA99004\")" +//A002416,A002402,X35019044,AB111510
                    // "&&  PreviousItem_Id == 388376" +
                    "&& PreviousItem_Id != null" +
                    //"&& (xBond_Item_Id == 0)" + not relevant because it could be assigned to another sale but not exwarehoused
                    "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                    "&& (PiQuantity < pQtyAllocated)" + // Assuming these properties exist in the context of the dynamic query

                    //////////// I left this because i only want to use interface to remove All ALLOCATED
                    //"&& (pQuantity > PiQuantity)" +
                    //"&& (pQuantity - pQtyAllocated  < 0.001)" + // prevents spill over allocations
                    "&& (Status == null || Status == \"\")" +
                    (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible" // Assuming AllowNonXEntries exists
                        ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))" // Assuming these properties exist
                        : "") +
                    ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\""); // Assuming pRegistrationDate exists

                // Assuming AllocationsModel.Instance.CreateEx9.Execute exists with this signature
                AllocationsModel.Instance.CreateEx9.Execute(filterExpression, false, false, false, docset, "Sales", "Historic", true, true, true, false, false, false, true, true, true, false).Wait();
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
}