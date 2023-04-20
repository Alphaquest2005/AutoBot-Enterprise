using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingPreviouslyAllocatedAsycudaItem;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class SalesAllocator
    {
        private ConcurrentDictionary<int, xcuda_Item> _asycudaItems;

        public bool isDBMem { get; set; } = false;
        static SalesAllocator()
        {
        }

        public SalesAllocator(ConcurrentDictionary<int, xcuda_Item> itemSetsAsycudaItems)
        {
            _asycudaItems = itemSetsAsycudaItems;
            
        }

        public List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>> AllocateSales(bool allocateToLastAdjustment, KeyValuePair<int, (List<int> Lst, KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<dynamic> Value)>> group)> groupItemSets)
        {
            var res = groupItemSets.Value.group.Value.Select(v => v.Value.Select(z => ((KeyValuePair<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet >)z).Value).ToList())
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage)).Select(async x => await AllocateSales(allocateToLastAdjustment, x).ConfigureAwait(false))
                .ToList();

            var alloLst = res.SelectMany(x => x.Result)
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .Select(x =>
                {
                    var entryDataDetails = x.Sales?.Select(s => s).ToList() ?? new List<EntryDataDetails>();

                    var xcudaItems = x.asycudaItems?.Select(a => a).ToList() ?? new List<xcuda_Item>();

                    var dbAllocations = x.Sales?.SelectMany(s => s.AsycudaSalesAllocations?.ToList()??
                                                                 new List<AsycudaSalesAllocations>()).ToList() ;

                    return new KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails>
                        EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>(1,
                        (new List<ExistingAllocations>(),
                            entryDataDetails,
                            xcudaItems,
                            dbAllocations));
                }).ToList();

            return alloLst;





        }

        public async Task<List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)>> AllocateSales(bool allocateToLastAdjustment, List<AllocationsBaseModel.ItemSet> itemSets)
        {
            StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
            var t = 0;
            var exceptions = new ConcurrentQueue<Exception>();
            var itemSetsValues = itemSets.ToList();

            var count = itemSetsValues.Count();
            var res = new List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)>();
            var orderedLst = itemSetsValues.OrderBy(x => x.Key.EntryDataDate).ToList();
            foreach (var itm in orderedLst)
                try
                {
                    t += 1;
                    // Debug.WriteLine($"Processing {itm.Key} - {t} with {itm.SalesList.Count} Sales: {0} of {itm.SalesList.Count}");
                    //StatusModel.Refresh();
                    var sales = SortEntryDataDetailsList(itm);
                    var asycudaItems = SortAsycudaItems(itm);

                    await AllocateSalestoAsycudaByKey(sales, asycudaItems, t, count, allocateToLastAdjustment, orderedLst).ConfigureAwait(false);

                    res.Add((sales, asycudaItems));

                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not Allocate - '{itm.Key}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }



            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return res;
        }

        private  List<xcuda_Item> SortAsycudaItems(AllocationsBaseModel.ItemSet itm)
        {
            var asycudaItems = itm.EntriesList.OrderBy(x => x.AsycudaDocument.AssessmentDate)
                .ThenBy(x => x.IsAssessed == null).ThenBy(x => x.AsycudaDocument.RegistrationDate)
                .ThenBy(x => Convert.ToInt32(x.AsycudaDocument.CNumber))
                .ThenByDescending(x =>
                    x.EntryPreviousItems.Select(z => z.xcuda_PreviousItem.Suplementary_Quantity)
                        .DefaultIfEmpty(0).Sum()) //NUO/44545 2 items with same date choose pIed one first
                .ThenBy(x => x.AsycudaDocument.ReferenceNumber)
                .DistinctBy(x => x.Item_Id)
                .ToList();
            return asycudaItems;
        }

        private  List<EntryDataDetails> SortEntryDataDetailsList(AllocationsBaseModel.ItemSet itm)
        {
            return itm.SalesList
                .OrderBy(x => x.Sales.EntryDataDate)
                .ThenBy(x => x.EntryDataId)
                .ThenBy(x => x.LineNumber ?? x.EntryDataDetailsId)
                .ThenByDescending(x => x.Quantity) /**/.ToList();
        }

        private async Task AllocateSalestoAsycudaByKey(List<EntryDataDetails> saleslst, List<xcuda_Item> asycudaEntries,
            double currentSetNo, int setNo, bool allocateToLastAdjustment,
            List<AllocationsBaseModel.ItemSet> salesSet)
        {
            try
            {
                if (allocateToLastAdjustment && saleslst.All(x => x.Adjustments == null)) return;

                if (asycudaEntries == null || !asycudaEntries.Any())
                {
                    foreach (var item in saleslst)
                        if (item.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == "No Asycuda Entries Found") == null)
                            await AddExceptionAllocation(item, "No Asycuda Entries Found").ConfigureAwait(false);
                    return;
                }

                var startAsycudaItemIndex = 0;
                var CurrentSalesItemIndex = 0;
                var cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, startAsycudaItemIndex);
                var saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);


                while (cAsycudaItm.QtyAllocated == Convert.ToDouble((double)cAsycudaItm.ItemQuantity))
                {
                    if (startAsycudaItemIndex + 1 < asycudaEntries.Count())
                    {
                        startAsycudaItemIndex += 1;
                        cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, startAsycudaItemIndex);
                    }
                    else
                        break;
                }


                for (var s = CurrentSalesItemIndex; s < saleslst.Count(); s++)
                {
                    var CurrentAsycudaItemIndex = startAsycudaItemIndex;// foreach sale start at beginning to search for possible qty to allocate



                    if (CurrentSalesItemIndex != s)
                    {
                        if (saleitm.AsycudaSalesAllocations.Count == 0) Debugger.Break();
                        CurrentSalesItemIndex = s;
                        saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);
                    }


                    // StatusModel.Refresh();

                    var saleitmQtyToallocate = saleitm.Quantity - saleitm.QtyAllocated;
                    if (saleitmQtyToallocate > 0 && CurrentAsycudaItemIndex == asycudaEntries.Count())
                    {
                        // over allocate to handle out of stock in case returns deal with it
                        await AllocateSaleItem(cAsycudaItm, saleitm, saleitmQtyToallocate, null)
                            .ConfigureAwait(false);
                        continue;
                    }

                    if (cAsycudaItm.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse &&
                        (cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
                    {
                        if (CurrentAsycudaItemIndex == 0)
                        {
                            await AddExceptionAllocation(saleitm, cAsycudaItm, "Early Sales" ).ConfigureAwait(false);
                            continue;
                        }
                    }

                    for (var i = CurrentAsycudaItemIndex; i < asycudaEntries.Count(); i++)
                    {
                        // reset in event earlier dat
                        if (saleitmQtyToallocate == 0) break;
                        if (CurrentAsycudaItemIndex != i || GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex).Item_Id != cAsycudaItm.Item_Id)
                        {
                            if (i < 0) i = 0;
                            CurrentAsycudaItemIndex = i;
                            cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex);

                        }
                        await Task.Run(() => Task.Run(() => Debug.WriteLine($"Processing {saleitm.ItemNumber} - {currentSetNo} of {setNo} with {saleslst.Count} Sales: {s} of {saleslst.Count} : {CurrentAsycudaItemIndex} of {asycudaEntries.Count}"))).ConfigureAwait(false);


                        var asycudaItmQtyToAllocate = GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out var subitm);


                        if (asycudaItmQtyToAllocate == 0 && saleitmQtyToallocate > 0)
                        {
                            CurrentAsycudaItemIndex += 1;
                            continue;
                        }

                        if (cAsycudaItm.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse && (cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
                        {
                            if (saleitm.Quantity < 0 && i > 2)
                            {
                                // if return step back to previous item
                                i -= 2;
                                continue;
                            }
                            await AddExceptionAllocation(saleitm, cAsycudaItm , "Early Sales").ConfigureAwait(false);
                            break;
                        }
                       

                        if (saleitmQtyToallocate < 0 && Enumerable.Where<AsycudaSalesAllocations>(cAsycudaItm.AsycudaSalesAllocations, x => x.DutyFreePaid == saleitm.DutyFreePaid).Sum(x => x.QtyAllocated) == 0)
                        {
                            var previousI = GetPreviousAllocatedAsycudaItem(asycudaEntries, saleslst, saleitm, i,salesSet);
                            if (previousI != i && previousI != i - 1)
                            {
                                i = previousI;
                                continue;
                            }
                        }
                        
                        if (asycudaItmQtyToAllocate < 0 &&
                            (CurrentAsycudaItemIndex != asycudaEntries.Count - 1 && asycudaEntries[i + 1].AsycudaDocument.AssessmentDate <= saleitm.Sales.EntryDataDate))
                        {
                            if (saleitmQtyToallocate > 0) continue;
                        } 
                        
                       

                        if (cAsycudaItm.QtyAllocated == 0 && saleitmQtyToallocate < 0 && CurrentSalesItemIndex > 0)
                        {
                            if (CurrentAsycudaItemIndex == 0)
                            {
                                await AddExceptionAllocation(saleitm, "Returned More than Sold").ConfigureAwait(false);
                                break;
                            }
                            i -= 2;

                        }
                        else
                        {
                            if ((asycudaItmQtyToAllocate > 0 && asycudaItmQtyToAllocate >= saleitmQtyToallocate) ||
                                CurrentAsycudaItemIndex == asycudaEntries.Count - 1)
                            {
                                var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, saleitmQtyToallocate, subitm)
                                    .ConfigureAwait(false);
                                saleitmQtyToallocate = ramt;

                                if (GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm) == 0 && ramt != 0)
                                {
                                    CurrentAsycudaItemIndex += 1;
                                    continue;
                                }

                                if (ramt == 0) break;
                                if (ramt < 0) /// step back 2 so it jumps 1
                                {
                                    if (i == 0)
                                    {
                                        if (CurrentSalesItemIndex == 0 && saleslst.Count == 1)
                                            i = GetPreviousAllocatedAsycudaItem(asycudaEntries,saleslst, saleitm, i, salesSet);

                                    }
                                    else
                                        i -= 2;

                                }
                            }
                            else
                            {

                                if (asycudaItmQtyToAllocate <= 0) asycudaItmQtyToAllocate = saleitmQtyToallocate;
                                var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, asycudaItmQtyToAllocate, subitm)
                                    .ConfigureAwait(false);
                                if (saleitmQtyToallocate < 0 && asycudaItmQtyToAllocate < 0)
                                {
                                    saleitmQtyToallocate += asycudaItmQtyToAllocate * -1;
                                    if (GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm) == 0)
                                    {
                                        CurrentAsycudaItemIndex += 1;
                                    }
                                }
                                else
                                {
                                    saleitmQtyToallocate -= asycudaItmQtyToAllocate;
                                }

                                // set here just incase
                                if (saleitmQtyToallocate == 0) break;
                                if (saleitmQtyToallocate < 0)
                                {
                                    throw new ApplicationException("saleitmQtyToallocate < 0 check this out");
                                }
                            }
                        }

                    }

                    if (saleitm.AsycudaSalesAllocations.Count == 0)
                    {
                        await AddExceptionAllocation(saleitm, "Over Sold").ConfigureAwait(false);
                        //Debugger.Break();
                    }


                }

            }


            catch (Exception e)
            {
                throw e;
            }
        }

        private int GetPreviousAllocatedAsycudaItem(List<xcuda_Item> asycudaEntries,
            List<EntryDataDetails> salesItems, EntryDataDetails saleitm, int i,
            List<AllocationsBaseModel.ItemSet> salesSet)
        {
            return isDBMem == true
                ? new GetPreviousAllocatedAsycudaItemDB().Execute(asycudaEntries, saleitm, i)
                : new GetPreviousAllocatedASycudaItemMemory(salesSet.SelectMany(x => x.SalesList).ToList()).Execute(asycudaEntries, saleitm, i);
        }

       

        private async Task AddExceptionAllocation(EntryDataDetails saleitm, xcuda_Item cAsycudaItm, string error)
        {
            var existingStock = CheckExistingStock(saleitm, cAsycudaItm);
            await AddExceptionAllocation(saleitm, error + existingStock).ConfigureAwait(false);
        }

        private string CheckExistingStock(EntryDataDetails saleitm, xcuda_Item cAsycudaItm)
        {
            return isDBMem
                ? new CheckExistingStockDB().Execute(cAsycudaItm.ItemNumber, saleitm.Sales.EntryDataDate)
                : new CheckExistingStockMemory(_asycudaItems).Execute(cAsycudaItm.ItemNumber,
                    saleitm.Sales.EntryDataDate);
        }

        private async Task AddExceptionAllocation(EntryDataDetails saleitm,  string error)
        {
            if (saleitm.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == error) == null)
            {
                var ssa = new AsycudaSalesAllocations(true)
                {
                    EntryDataDetailsId = saleitm.EntryDataDetailsId,
                    //EntryDataDetails = saleitm,
                    QtyAllocated = saleitm.Quantity - saleitm.QtyAllocated,
                    Status = error,
                    TrackingState = TrackingState.Added
                };
                //await SaveAllocation(ssa).ConfigureAwait(false);
                saleitm.QtyAllocated += ssa.QtyAllocated;
                saleitm.AsycudaSalesAllocations.Add(ssa);
            }
        }

        private double GetAsycudaItmQtyToAllocate(xcuda_Item cAsycudaItm, EntryDataDetails saleitm, out SubItems subitm)
        {
            double asycudaItmQtyToAllocate;
            if (cAsycudaItm.SalesFactor == 0) cAsycudaItm.SalesFactor = 1;
            if (cAsycudaItm.SubItems.Any())
            {
                subitm = cAsycudaItm.SubItems.FirstOrDefault(x => x.ItemNumber == saleitm.ItemNumber);
                if (subitm != null)
                {
                    // TODO: Add code here to CalculateAsycudaItmQtyToAllocate like non sub items
                    Debugger.Break();
                    // TODO: Add code here to CalculateAsycudaItmQtyToAllocate like non sub items

                    asycudaItmQtyToAllocate = subitm.Quantity - subitm.QtyAllocated;
                    //if (Convert.ToDouble(asycudaItmQtyToAllocate) > (Convert.ToDouble(cAsycudaItm.ItemQuantity) - cAsycudaItm.QtyAllocated))
                    //{
                    //    asycudaItmQtyToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
                    //}
                }
                else
                {
                    asycudaItmQtyToAllocate = 0;
                }
            }
            else
            {
                asycudaItmQtyToAllocate = CalculateAsycudaItmQtyToAllocate(cAsycudaItm, saleitm);
                subitm = null;
            }

            return asycudaItmQtyToAllocate;
        }

        private  double CalculateAsycudaItmQtyToAllocate(xcuda_Item cAsycudaItm,
            EntryDataDetails saleItem)
        {
            try
            {

          

                var totalAvailableToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
                //var TotalPiQty = (double)cAsycudaItm.EntryPreviousItems
                //	.Select(x => x.xcuda_PreviousItem)
                //	.Sum(x => x.Suplementary_Quantity);
                var nonDFPQty = cAsycudaItm.EntryPreviousItems.Any() ? (double)cAsycudaItm.EntryPreviousItems
                    .Select(x => x.PreviousItemsEx)
                    .Where(x => x.DutyFreePaid != saleItem.DutyFreePaid || (x.EntryDataType ?? "Sales") != saleItem.Sales.EntryDataType)
                    .Sum(x => x.Suplementary_Quantity) : (saleItem.DutyFreePaid == "Duty Free" ? cAsycudaItm.DPQtyAllocated : cAsycudaItm.DFQtyAllocated);



                //var previousItems = cAsycudaItm.EntryPreviousItems
                //	.Select(x => x.xcuda_PreviousItem)
                //	.Where(x => x.DutyFreePaid == saleItem.DutyFreePaid).ToList();

                var totalDfPQtyAllocated = saleItem.DutyFreePaid == "Duty Free" ? cAsycudaItm.DFQtyAllocated : cAsycudaItm.DPQtyAllocated;

                //var TotalDFPtoAllocate = previousItems.Any() ? (double)previousItems
                //	.Sum(x => x.Suplementary_Quantity) : totalDfPQtyAllocated;
                //var TotalDFPAllocatedQty = previousItems.Any() ? previousItems
                //	.Sum(x => x.QtyAllocated) : totalDfPQtyAllocated;
                //var remainingDFPAllocation = TotalDFPtoAllocate - TotalDFPAllocatedQty;
                //var freeToAllocate = cAsycudaItm.ItemQuantity - TotalDFPtoAllocate; //TotalDFPAllocatedQty + nonDFPQty + cAsycudaItm.QtyAllocated;

                //var allocatedQty = cAsycudaItm.AsycudaSalesAllocations.Where(x => x.DutyFreePaid == saleItem.DutyFreePaid).Sum(x => x.QtyAllocated);
                var nonAllocatedQty = cAsycudaItm.AsycudaSalesAllocations.Where(x => x.DutyFreePaid != saleItem.DutyFreePaid).Sum(x => x.QtyAllocated);

                var finalNonDFPQty = nonDFPQty > nonAllocatedQty ? nonDFPQty : nonAllocatedQty;

                var TakeOut = (finalNonDFPQty + totalDfPQtyAllocated) >= cAsycudaItm.ItemQuantity 
                    ? finalNonDFPQty >= cAsycudaItm.ItemQuantity ? cAsycudaItm.ItemQuantity : cAsycudaItm.QtyAllocated 
                    : (finalNonDFPQty + totalDfPQtyAllocated);


                var res = cAsycudaItm.ItemQuantity - TakeOut;
                if (totalAvailableToAllocate == 0) res = 0;
                return res * cAsycudaItm.SalesFactor;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private xcuda_Item GetAsycudaEntriesWithItemNumber(IList<xcuda_Item> asycudaEntries, int CurrentAsycudaItemIndex)
        {
            _asycudaItems.TryGetValue(asycudaEntries.ElementAtOrDefault(CurrentAsycudaItemIndex).Item_Id, out var cAsycudaItm);
            ///////////////////// took this out because returns cross thread with duty free and duty paid -- 'CRC/06037' 'GB00050065'
            //if (cAsycudaItm.QtyAllocated == 0 && (cAsycudaItm.DFQtyAllocated != 0 || cAsycudaItm.DPQtyAllocated != 0))
            //{

            //    cAsycudaItm.DFQtyAllocated = 0;
            //    cAsycudaItm.DPQtyAllocated = 0;
            //}

            return cAsycudaItm;
        }

        private EntryDataDetails GetSaleEntries(IList<EntryDataDetails> SaleEntries, int CurrentSaleItemIndex)
        {
            return SaleEntries.ElementAtOrDefault(CurrentSaleItemIndex);
        }

        private async Task<double> AllocateSaleItem(xcuda_Item cAsycudaItm, EntryDataDetails saleitm,
            double saleitmQtyToallocate, SubItems subitm)
        {
            //cAsycudaItm.StartTracking();
            //saleitm.StartTracking();
            if (cAsycudaItm.SalesFactor == 0) cAsycudaItm.SalesFactor = 1;

            var dfp = saleitm.DutyFreePaid;
            // allocate Sale item
            var ssa = new AsycudaSalesAllocations
            {
                EntryDataDetailsId = saleitm.EntryDataDetailsId,
                PreviousItem_Id = cAsycudaItm.Item_Id,
                QtyAllocated = 0,
                TrackingState = TrackingState.Added
            };

            if (!string.IsNullOrEmpty(cAsycudaItm.WarehouseError))
            {
                ssa.Status = cAsycudaItm.WarehouseError;
            }




            if (saleitmQtyToallocate != 0) //&& removed because of previous return//cAsycudaItm.QtyAllocated >= 0 && 
                // cAsycudaItm.QtyAllocated <= Convert.ToDouble(cAsycudaItm.ItemQuantity)
            {


                if (saleitmQtyToallocate > 0)
                {

                    if (subitm != null)
                    {
                        subitm.StartTracking();
                        subitm.QtyAllocated += saleitmQtyToallocate;
                    }

                    if (dfp == "Duty Free")
                    {
                        cAsycudaItm.DFQtyAllocated += saleitmQtyToallocate / cAsycudaItm.SalesFactor;
                    }
                    else
                    {
                        cAsycudaItm.DPQtyAllocated += saleitmQtyToallocate / cAsycudaItm.SalesFactor;
                    }

                    if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                    {
                        SetPreviousItemXbond(ssa, cAsycudaItm, dfp, saleitmQtyToallocate / cAsycudaItm.SalesFactor);
                    }



                    saleitm.QtyAllocated += saleitmQtyToallocate;

                    ssa.QtyAllocated += saleitmQtyToallocate;

                    saleitmQtyToallocate = 0;
                }
                else
                {
                    double mqty = saleitmQtyToallocate * -1;


                    if (subitm != null)
                    {
                        subitm.StartTracking();
                        subitm.QtyAllocated -= mqty;
                    }

                    if (dfp == "Duty Free")
                    {
                        //if (cAsycudaItm.DFQtyAllocated !=/*> change to != 0 to match below to mark return more than sold like below*/ 0 && cAsycudaItm.DFQtyAllocated < mqty) mqty = cAsycudaItm.DFQtyAllocated;
                        cAsycudaItm.DFQtyAllocated -= mqty / cAsycudaItm.SalesFactor;
                    }
                    else
                    {
                        //if (cAsycudaItm.DPQtyAllocated != 0 && cAsycudaItm.DPQtyAllocated < mqty) mqty = cAsycudaItm.DPQtyAllocated;
                        cAsycudaItm.DPQtyAllocated -= mqty / cAsycudaItm.SalesFactor;
                    }



                    if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                    {
                        SetPreviousItemXbond(ssa, cAsycudaItm, dfp, -mqty / cAsycudaItm.SalesFactor);
                    }

                    saleitmQtyToallocate += mqty;

                    saleitm.QtyAllocated -= mqty;


                    ssa.QtyAllocated -= mqty;

                    //}
                }
            }

            if (ssa.QtyAllocated == 0) return saleitmQtyToallocate;
            //SaveAllocation(cAsycudaItm, saleitm, subitm, ssa);

            saleitm.AsycudaSalesAllocations.Add(ssa);
            ssa.EntryDataDetails = saleitm;
            ssa.PreviousDocumentItem = cAsycudaItm;
            cAsycudaItm.AsycudaSalesAllocations.Add(ssa);
            _asycudaItems.AddOrUpdate(cAsycudaItm.Item_Id, cAsycudaItm, (key, oldValue) => cAsycudaItm);


            return saleitmQtyToallocate;
        }

        private void SetPreviousItemXbond(AsycudaSalesAllocations ssa, xcuda_Item cAsycudaItm, string dfp, double amt)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate != "Visible") return;


            var alst = cAsycudaItm.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                .Where(x => x.DutyFreePaid == dfp && x.QtyAllocated <= (double)x.Suplementary_Quantity)
                .Where(x => x.xcuda_Item != null && x.xcuda_Item.AsycudaDocument != null && x.xcuda_Item.AsycudaDocument.Cancelled != true)
                .OrderBy(
                    x =>
                        x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate ?? Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate)).ToList();
            foreach (var pitm in alst)
            {

                var atot = (double)pitm.Suplementary_Quantity - Convert.ToDouble(pitm.QtyAllocated);
                if (atot == 0) continue;
                if (amt <= atot)
                {
                    pitm.QtyAllocated += amt;
                    var xbond = new xBondAllocations(true)
                    {
                        AllocationId = ssa.AllocationId,
                        xEntryItem_Id = pitm.xcuda_Item.Item_Id,
                        TrackingState = TrackingState.Added
                    };

                    ssa.xBondAllocations.Add(xbond);
                    pitm.xcuda_Item.xBondAllocations.Add(xbond);
                    break;
                }
                else
                {
                    pitm.QtyAllocated += atot;
                    var xbond = new xBondAllocations(true)
                    {
                        AllocationId = ssa.AllocationId,
                        xEntryItem_Id = pitm.xcuda_Item.Item_Id,
                        TrackingState = TrackingState.Added
                    };
                    ssa.xBondAllocations.Add(xbond);
                    pitm.xcuda_Item.xBondAllocations.Add(xbond);
                    amt -= atot;
                }

            }
        }

    }
}