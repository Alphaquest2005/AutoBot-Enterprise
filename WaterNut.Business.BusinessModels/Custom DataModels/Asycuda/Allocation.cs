using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


using System.Windows;
using AllocationDS.Business.Entities;
using InventoryDS.Business.Entities;
using Core.Common.UI;
using AllocationDS.Business.Services;
using SimpleMvvmToolkit;
using TrackableEntities;
using SubItems = AllocationDS.Business.Entities.SubItems;
using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;


namespace WaterNut.DataSpace
{
    public partial class AllocationsBaseModel
    {

        private static readonly AllocationsBaseModel instance;
        static AllocationsBaseModel()
        {
            instance = new AllocationsBaseModel();
        }

        public static AllocationsBaseModel Instance
        {
            get { return instance; }
        }
        internal class ItemSales
        {
            public string ItemNumber { get; set; }
            public List<EntryDataDetails> SalesList { get; set; }
        }

        internal class InventoryItemSales
        {
            public InventoryItem InventoryItem { get; set; }
            public List<EntryDataDetails> SalesList { get; set; }
        }

        internal class ItemEntries
        {
            public string ItemNumber { get; set; }
            public List<xcuda_Item> EntriesList { get; set; }
        }

        internal class ItemSet
        {
            public string ItemNumber { get; set; }
            public List<EntryDataDetails> SalesList { get; set; }
            public List<xcuda_Item> EntriesList { get; set; }
        }


        public async Task AllocateSales(bool itemDescriptionContainsAsycudaAttribute)
        {
            StatusModel.Timer("Allocating Sales");

            if (itemDescriptionContainsAsycudaAttribute == true)
            {
                await AllocateSalesWhereItemDescriptionContainsAsycudaAttribute().ConfigureAwait(false);
            }
            else
            {
                await AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber().ConfigureAwait(false);
            }
            StatusModel.StopStatusUpdate();

        }

        private async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber()
        {
            var itemSets = await MatchSalestoAsycudaEntriesOnItemNumber().ConfigureAwait(false);
          //  var itemSets = lst as ConcurrentDictionary<,ItemSet> ?? lst.Where(x => x.SalesList.Count > 0).ToList();
            StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
            var t = 0;
           Parallel.ForEach(itemSets.Values, new ParallelOptions(){MaxDegreeOfParallelism= Environment.ProcessorCount * 2},itm => //.Where(x => x.ItemNumber == "AT18547")
            {
            //foreach (var itm in itemSets.Values.Where(x => x.SalesList.Any(y => y.EntryDataId == "GA-0013713-1")))
            //{
                StatusModel.StatusUpdate();
                AllocateSalestoAsycudaByItemNumber(itm.SalesList,//.SalesList.Where(x => x.DoNotAllocate != true).ToList()
                    itm.EntriesList).Wait();
            //};
                

            });
        }

        private async Task AllocateSalesWhereItemDescriptionContainsAsycudaAttribute()
        {
            StatusModel.Timer("Loading Sales Data...");
            var sales = await GetSales().ConfigureAwait(false);

           StatusModel.Timer("Loading Asycuda Data...");
           var IMAsycudaEntries = await GetAllAsycudaEntries().ConfigureAwait(false);
            
            StatusModel.StartStatusUpdate("Allocating Sales", sales.Count());
            var t = 0;
           // Parallel.ForEach(sales, salesDetails => //.Where(x => x.EntryDataId == "7500")
            foreach (var salesDetails in sales)
          
            {
                var salesDescrip = salesDetails.ItemDescription.ToUpper();

                string attrib = salesDescrip.Split('|').Length > 2
                    ? salesDescrip.Split('|')[2].ToUpper().Replace(" ", "")
                    : null;


                StatusModel.StatusUpdate();
                var alst = GetAsycudaEntries(IMAsycudaEntries, attrib, salesDescrip, salesDetails.ItemNumber);//
                //GetSubItems(alst, salesDetails);
               // if(!alst.Any()) Debugger.Break();

                var slst = new List<EntryDataDetails>() {salesDetails};

                if (slst.Any())
                    await AllocateSalestoAsycudaByItemNumber(slst, alst).ConfigureAwait(false);
                //AllocateSalestoAsycudaByItemNumber(slst, alst.Distinct()
                //    .OrderBy(
                //        x =>
                //            x.AsycudaDocument.EffectiveRegistrationDate ??
                //            Convert.ToDateTime(x.AsycudaDocument.RegistrationDate)).ToList());
            }
              //  );
        }

     
        private List<xcuda_Item> GetAsycudaEntries(List<xcuda_Item> IMAsycudaEntries, string attrib, string salesDescrip, string itemNumber)
        {
            var alst = new List<xcuda_Item>();

            if (attrib != null)
            {
                alst.AddRange(IMAsycudaEntries.Where(x =>   x.SubItems.Any() == false  && 
                                                           (x.AttributeOnlyAllocation != null && x.AttributeOnlyAllocation == true)
                                                            && x.ItemNumber == attrib));
            }
            alst.AddRange(IMAsycudaEntries.Where(x => x.SubItems.Any() == false && 
                                                     (x.AttributeOnlyAllocation == null || x.AttributeOnlyAllocation != true)
                                                    && salesDescrip.Replace(" ", "").Contains(x.ItemNumber.ToUpper().Replace(" ", ""))));

            alst.AddRange(IMAsycudaEntries.Where(x => x.SubItems.Any() == true 
                                                    && x.SubItems.Any( z => z.ItemNumber == itemNumber)));

            return alst.Distinct().OrderBy(x => x.AsycudaDocument.RegistrationDate).ToList();
        }

        private async Task<List<xcuda_Item>> GetAllAsycudaEntries()
        {
            var alst = new List<xcuda_Item>();
            using (var ctx = new xcuda_ItemService())
            {
                alst.AddRange(ctx.Getxcuda_ItemByExpressionLst(
                    new List<string>()
                    {
                        "DoNotAllocate == null || DoNotAllocate != true",
                        "AsycudaDocument.DocumentType == \"IM7\"",
                       //"SubItems.Count > 0",
                       // "AttributeOnlyAllocation == true"
                        //string.Format("EX.Precision_4.ToUpper() == \"{0}\"", attrib)
                    },
                    new List<string>() {"SubItems", "AsycudaDocument", "EX"}).Result.Distinct());

            }

            //using (var ctx = new SubItemsService())
            //{
            //    alst.AddRange(
            //        ctx.GetSubItemsByExpressionLst(
            //            new List<string>()
            //            {
            //                "All"
            //                //string.Format("ItemNumber == \"{0}\"", salesDetails.ItemNumber)
            //            }
            //            , new List<string>() { "xcuda_Item", "xcuda_Item.EX", "xcuda_Item.AsycudaDocument" }).Result
            //        //.Where(y => y.ItemNumber == salesDetails.ItemNumber)
            //            .Select(x => x.xcuda_Item).ToList());
            //}
            return alst;
        }

        private static async Task<List<EntryDataDetails>> GetSales()
        {
            List<EntryDataDetails> sales = null;
            using (var ctx = new EntryDataDetailsService())
            {
                sales = (await ctx.GetEntryDataDetailsByExpressionLst(new List<string>()
                {
                  // "EntryDataDetailsId == 85371",
                    "Sales.INVNumber != null",
                    "QtyAllocated != Quantity",
                    "DoNotAllocate != true"
                },
                    new List<string>() { "Sales", "AsycudaSalesAllocations" }).ConfigureAwait(false))
                    //.Where(x => Convert.ToDouble(x.QtyAllocated) != Convert.ToDouble(x.Quantity) && x.DoNotAllocate != true)
                    .OrderBy(x => x.Sales.EntryDataDate).ToList();//.Take(100)
            }
            return sales;
        }

        private static void GetSubItems(List<xcuda_Item> alst, EntryDataDetails salesDetails)
        {
            using (var ctx = new SubItemsService())
            {
                alst.AddRange(
                    ctx.GetSubItemsByExpressionLst(
                        new List<string>() {string.Format("ItemNumber == \"{0}\"", salesDetails.ItemNumber)}
                        , new List<string>() {"xcuda_Item", "xcuda_Item.EX", "xcuda_Item.AsycudaDocument"}).Result
                        //.Where(y => y.ItemNumber == salesDetails.ItemNumber)
                        .Select(x => x.xcuda_Item).ToList());
            }
        }

        private static List<xcuda_Item> GetAsycudaEntries(string attrib, string salesDescrip)
        {
            var alst = new List<xcuda_Item>();


            // alst.AddRange(db.xcuda_Item.Where(x => x.PreviousDocumentItem.SubItems.Any(y => y.ItemNumber == salesDetails.InventoryItems.ItemNumber)));
            //match by attribute
            //alst = db.xcuda_Item.AsEnumerable().Where(x => salesDetails.ItemDescription.ToUpper().Split('|')[2] == x.ItemNumber.ToUpper()).ToList();
            using (var ctx = new xcuda_ItemService())
            {
                if (attrib != null)
                {
                    alst =
                        (ctx.Getxcuda_ItemByExpressionLst(
                            new List<string>()
                            {
                                "DoNotAllocate == null || DoNotAllocate != true",
                                "AsycudaDocument.DocumentType == \"IM7\"",
                                "SubItems.Count == 0",
                                "AttributeOnlyAllocation == true",
                                string.Format("EX.Precision_4.ToUpper() == \"{0}\"", attrib)
                            },
                            new List<string>() {"SubItems", "AsycudaDocument", "EX"}).Result)
                            .ToList();
                    //.Where(x => x.SubItems.Any() == false && (x.AttributeOnlyAllocation == true) && string.IsNullOrEmpty(salesDetails.ItemDescription.ToUpper().Split('|')[2]) == false && salesDetails.ItemDescription.ToUpper().Split('|')[2].ToUpper().Replace(" ", "") == x.ItemNumber.ToUpper().Replace(" ", "")).ToList();
                }
                alst.AddRange((ctx.Getxcuda_ItemByExpressionLst(new List<string>()
                {
                    "DoNotAllocate == null || DoNotAllocate != true",
                    "AsycudaDocument.DocumentType == \"IM7\"",
                   "SubItems.Count == 0",
                    "AttributeOnlyAllocation == null || AttributeOnlyAllocation != true",
                    string.Format("\"{0}\".Contains(EX.Precision_4.ToUpper())", salesDescrip)
                },
                    new List<string>() {"SubItems", "AsycudaDocument", "EX"})).Result);
                //.Where(x => x.SubItems.Any() == false 
                //            && (x.AttributeOnlyAllocation == null || x.AttributeOnlyAllocation != true) 
                //            && salesDetails.ItemDescription.ToUpper().Replace(" ", "").Contains(x.ItemNumber.ToUpper().Replace(" ", ""))));
            }
            return alst;
        }

        private async Task SaveItemLst(ItemSet itm)
        {
                foreach (var item in itm.SalesList)
                {
                    await SaveEntryDataDetails(item).ConfigureAwait(false);
                }
           
           
                foreach (var item in itm.EntriesList)
                {
                   await SaveXcuda_Item(item).ConfigureAwait(false);
                }
        }

        private  async Task<ConcurrentDictionary<string,ItemSet>> MatchSalestoAsycudaEntriesOnItemNumber()
        {
            var asycudaEntries = await GetAsycudaEntries().ConfigureAwait(false);

            var saleslst = await GetSaleslst().ConfigureAwait(false);

            var itmLst = CreateItemSets(saleslst, asycudaEntries);

            return itmLst; //.Where(x => x.ItemNumber == "OC1719907");
        }

        private static ConcurrentDictionary<string,ItemSet> CreateItemSets(IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
        {
            var itmLst = from s in saleslst
                join a in asycudaEntries on s.ItemNumber equals a.ItemNumber into j
                from a in j.DefaultIfEmpty()
                select new ItemSet
                {
                    ItemNumber = s.ItemNumber,
                    SalesList = s.SalesList,
                    EntriesList = a != null ? a.EntriesList : null
                };

            var res = new ConcurrentDictionary<string, ItemSet>();
            foreach (var itm in itmLst)
            {
                res.AddOrUpdate(itm.ItemNumber, itm,(key,value) => itm);
            }
            return res;
        }

        private static async Task<IEnumerable<ItemEntries>> GetAsycudaEntries()
        {
            StatusModel.Timer("Getting Data - Asycuda Entries...");
            //string itmnumber = "MMM/08657";
            IEnumerable<ItemEntries> asycudaEntries = null;
            using (var ctx = new xcuda_ItemService())
            {
                var lst = await ctx.Getxcuda_ItemByExpressionNav(
                    "All",
                    //"EX.Precision_4 == \"PE10833\"",
                    new Dictionary<string, string>() {{"AsycudaDocument", "CNumber != null && DocumentType == \"IM7\""}}
                    , new List<string>() {"AsycudaDocument", "EX"}).ConfigureAwait(false);


                asycudaEntries = from s in lst
                    //.Where(x => x.ItemNumber == itmnumber)
                    //       .Where(x => x.AsycudaDocument.CNumber != null).AsEnumerable()
                    group s by s.ItemNumber
                    into g
                    select
                        new ItemEntries
                        {
                            ItemNumber = g.Key,
                            EntriesList =
                                g.AsEnumerable()
                                    .OrderBy(
                                        x =>
                                            x.AsycudaDocument.EffectiveRegistrationDate == null
                                                ? Convert.ToDateTime(x.AsycudaDocument.RegistrationDate)
                                                : x.AsycudaDocument.EffectiveRegistrationDate)
                                    .ToList()
                        };
            }
            return asycudaEntries;
        }

        private static async Task<IEnumerable<ItemSales>> GetSaleslst()
        {
            StatusModel.Timer("Getting Data - Sales Entries...");
            // SwitchToMyDB(mydb, BaseDataModel.Instance.CurrentAsycudaDocumentSet);

            IEnumerable<ItemSales> saleslst = null;
            using (var ctx = new EntryDataDetailsService())
            {
                var salesData =

                await
                        ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"PE10833\" &&" +
                                                               "QtyAllocated != Quantity " +
                                                               "&& DoNotAllocate != true", new Dictionary<string, string>() { { "Sales", "INVNumber != null" } }, new List<string>() { "Sales" })
                            .ConfigureAwait(false);
                saleslst = from d in salesData
                    //.Where(x => x.EntryData == "GB-0009053")                                       
                    //.SelectMany(x => x.EntryDataDetails.Select(ed => ed))
                    //.Where(x => x.QtyAllocated == null || x.QtyAllocated != ((Double) x.Quantity))
                    //.Where(x => x.ItemNumber == itmnumber)
                    // .AsEnumerable()
                    group d by d.ItemNumber
                    into g
                    select
                        new ItemSales
                        {
                            ItemNumber = g.Key,
                            SalesList = g.OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId).ToList()
                        };
            }
            return saleslst;
        }


        private  async Task AllocateSalestoAsycudaByItemNumber(List<EntryDataDetails> saleslst, List<xcuda_Item> asycudaEntries)
        {
            try
            {


                if (asycudaEntries == null || !asycudaEntries.Any())
                {
                    foreach (var item in saleslst)
                    {
                        if (item.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == "No Asycuda Entries Found") == null)
                        {
                            await AddExceptionAllocation(item, "No Asycuda Entries Found").ConfigureAwait(false);
                            
                        }

                    }
                   
                    // continue;
                    return;
                }

                var CurrentAsycudaItemIndex = 0;
                var CurrentSalesItemIndex = 0;
                var cAsycudaItm = GetAsycudaEntries(asycudaEntries, CurrentAsycudaItemIndex);

                while (cAsycudaItm.QtyAllocated == Convert.ToDouble(cAsycudaItm.ItemQuantity))
                {
                    if (CurrentAsycudaItemIndex + 1 < asycudaEntries.Count())
                    {
                        CurrentAsycudaItemIndex += 1;
                        cAsycudaItm = GetAsycudaEntries(asycudaEntries, CurrentAsycudaItemIndex);
                    }
                    else
                    {
                        break;
                    }
                }

                var saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);
                // AllocatePreviousItems(cAsycudaItm);

                // foreach (var saleitm in saleslst)
                for (var s = CurrentSalesItemIndex; s < saleslst.Count(); s++)
                {
                    StatusModel.Refresh();

                    //if (saleitm.QtyAllocated == null) saleitm.QtyAllocated = 0;
                    if (CurrentSalesItemIndex != s)
                    {
                        CurrentSalesItemIndex = s;
                        saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);
                    }



                    foreach (var allo in saleitm.AsycudaSalesAllocations.ToList())
                    {
                        await AllocationsModel.Instance.ClearAllocation(allo).ConfigureAwait(false);
                        saleitm.AsycudaSalesAllocations.Remove(allo);
                    }

                    //saleitm.AsycudaSalesAllocations.Clear();
                    var saleitmQtyToallocate = saleitm.Quantity - saleitm.QtyAllocated;
                    SubItems subitm = null;

                    for (var i = CurrentAsycudaItemIndex; i < asycudaEntries.Count(); i++)
                    {
                        // reset in event earlier dat



                        if (CurrentAsycudaItemIndex != i)
                        {
                            CurrentAsycudaItemIndex = i;
                            cAsycudaItm = GetAsycudaEntries(asycudaEntries, CurrentAsycudaItemIndex);
                        }

                        ReturnsStepBack(asycudaEntries, ref i, ref CurrentAsycudaItemIndex, ref cAsycudaItm, saleitm);

                        if ((cAsycudaItm.AsycudaDocument.EffectiveRegistrationDate == null ? cAsycudaItm.AsycudaDocument.RegistrationDate : cAsycudaItm.AsycudaDocument.EffectiveRegistrationDate) > saleitm.Sales.EntryDataDate)
                        {
                            await AddExceptionAllocation(saleitm, "Early Sales").ConfigureAwait(false);
               
                            break;
                        }


                        var asycudaItmQtyToAllocate = GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm);


                        if (asycudaItmQtyToAllocate == 0 && saleitmQtyToallocate > 0)
                        {
                            continue;
                        }

                        if ((asycudaItmQtyToAllocate) >= (saleitmQtyToallocate))
                        {
                            var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, saleitmQtyToallocate, subitm).ConfigureAwait(false);
                            saleitmQtyToallocate = ramt;
                            if (ramt == 0)
                            {
                                break;
                            }

                            if (i == 0 && saleitmQtyToallocate < 0) break;
                            ReturnsStepBack(asycudaEntries, ref i, ref CurrentAsycudaItemIndex, ref cAsycudaItm, saleitm);
                        }
                        else
                        {
                            if (asycudaItmQtyToAllocate != 0)
                            {
                                var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, asycudaItmQtyToAllocate, subitm).ConfigureAwait(false);
                                saleitmQtyToallocate -= asycudaItmQtyToAllocate;
                                if (saleitmQtyToallocate < 0)
                                {
                                    throw new ApplicationException("saleitmQtyToallocate < 0 check this out");
                                }
                            }
                            else
                            {
                                await AddExceptionAllocation(saleitm, "Out of Stock").ConfigureAwait(false);
                            }

                        }
                    }

                    if (saleitmQtyToallocate > 0 && saleitm.AsycudaSalesAllocations.Any() == false)//
                    {
                        await AddExceptionAllocation(saleitm, "Insufficent Quantities").ConfigureAwait(false);
                        
                    }

                }

            }


            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task AddExceptionAllocation(EntryDataDetails saleitm, string error)
        {
            var ssa = new AsycudaSalesAllocations()
            {
                EntryDataDetailsId = saleitm.EntryDataDetailsId,
                //EntryDataDetails = saleitm,
                Status = error,
                TrackingState = TrackingState.Added
            };
            await SaveAllocation(ssa).ConfigureAwait(false);
            saleitm.AsycudaSalesAllocations.Add(ssa);
        }

        private static async Task SaveEntryDataDetails(EntryDataDetails item)
        {
            if (item == null) return;
            using (var ctx = new EntryDataDetailsService())
            {
                await ctx.UpdateEntryDataDetails(item).ConfigureAwait(false);
            }
        }

        private  void ReturnsStepBack(List<xcuda_Item> asycudaEntries, ref int i, ref int CurrentAsycudaItemIndex, ref xcuda_Item cAsycudaItm, EntryDataDetails saleitm)
        {
            if (saleitm.Quantity < 0) // if is a return go to previous cAsycudaItem
            {
                while (cAsycudaItm.QtyAllocated == 0 && CurrentAsycudaItemIndex > 0)
                {
                    CurrentAsycudaItemIndex -= 1;
                    cAsycudaItm = GetAsycudaEntries(asycudaEntries, CurrentAsycudaItemIndex);
                    i = CurrentAsycudaItemIndex;
                }

            }
        }

        private  void AllocatePreviousItems(xcuda_Item cAsycudaItm)
        {

            foreach (var pitm in cAsycudaItm.xcuda_PreviousItems.OrderBy(x => x.xcuda_Item.AsycudaDocument.RegistrationDate))
            {
                if (pitm.QtyAllocated == null) pitm.QtyAllocated = 0;
                var atot = pitm.Suplementary_Quantity - Convert.ToSingle(pitm.QtyAllocated);
                var ctot = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
                if (ctot >= atot)
                {
                    if (pitm.DutyFreePaid == "Duty Free")
                    {
                        cAsycudaItm.DFQtyAllocated += atot;
                    }
                    else
                    {
                        cAsycudaItm.DPQtyAllocated += atot;
                    }

                    pitm.QtyAllocated += atot;
                }
                else
                {
                    if (pitm.DutyFreePaid == "Duty Free")
                    {
                        cAsycudaItm.DFQtyAllocated += ctot;
                    }
                    else
                    {
                        cAsycudaItm.DPQtyAllocated += ctot;
                    }

                    pitm.QtyAllocated += ctot;

                }
            }

        }

        private double GetAsycudaItmQtyToAllocate(xcuda_Item cAsycudaItm, EntryDataDetails saleitm, out SubItems subitm)
        {
            double asycudaItmQtyToAllocate;
            if (cAsycudaItm.SubItems.Any())
            {
                subitm = cAsycudaItm.SubItems.FirstOrDefault(x => x.ItemNumber == saleitm.ItemNumber);
                if (subitm != null)
                {
                    asycudaItmQtyToAllocate = subitm.Quantity - subitm.QtyAllocated;
                    if (Convert.ToDouble(asycudaItmQtyToAllocate) > (Convert.ToDouble(cAsycudaItm.ItemQuantity) - cAsycudaItm.QtyAllocated))
                    {
                        asycudaItmQtyToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
                    }
                }
                else
                {
                    asycudaItmQtyToAllocate = 0;
                }
            }
            else
            {
                asycudaItmQtyToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
                subitm = null;
            }

            return asycudaItmQtyToAllocate;
        }

        private  xcuda_Item GetAsycudaEntries(IList<xcuda_Item> asycudaEntries, int CurrentAsycudaItemIndex)
        {
            var cAsycudaItm = asycudaEntries.ElementAtOrDefault<xcuda_Item>(CurrentAsycudaItemIndex);
            if (cAsycudaItm.QtyAllocated == 0)
            {
                cAsycudaItm.DFQtyAllocated = 0;
                cAsycudaItm.DPQtyAllocated = 0;
            }

            return cAsycudaItm;
        }

        private  EntryDataDetails GetSaleEntries(IList<EntryDataDetails> SaleEntries, int CurrentSaleItemIndex)
        {
            var SaleItm = SaleEntries.ElementAtOrDefault<EntryDataDetails>(CurrentSaleItemIndex);
            if (SaleItm == null) return null;
            if (SaleItm.QtyAllocated == null) SaleItm.QtyAllocated = 0;
            // AllocatePreviousItems(cAsycudaItm); /// have to take this out cuz i can' block sales also need to find a way to link asycuda to sales
            return SaleItm;
        }

        private  async Task<double> AllocateSaleItem(xcuda_Item cAsycudaItm, EntryDataDetails saleitm,
                                             double saleitmQtyToallocate, SubItems subitm)
        {
            try
            {

                var dfp = ((Sales) saleitm.Sales).DutyFreePaid;
                // allocate Sale item
                var ssa = new AsycudaSalesAllocations
                {
                    EntryDataDetailsId = saleitm.EntryDataDetailsId,
                    PreviousItem_Id = cAsycudaItm.Item_Id,
                    QtyAllocated = 0,
                    TrackingState = TrackingState.Added
                };




                if (cAsycudaItm.QtyAllocated >= 0 && saleitmQtyToallocate != 0 &&
                    cAsycudaItm.QtyAllocated <= Convert.ToDouble(cAsycudaItm.ItemQuantity))
                {


                    if (saleitmQtyToallocate > 0)
                    {

                        if (subitm != null)
                        {
                            subitm.QtyAllocated = subitm.QtyAllocated + saleitmQtyToallocate;
                        }

                        if (dfp == "Duty Free")
                        {
                            cAsycudaItm.DFQtyAllocated += saleitmQtyToallocate;
                        }
                        else
                        {
                            cAsycudaItm.DPQtyAllocated += saleitmQtyToallocate;
                        }

                        if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                        {
                            SetPreviousItemXbond(ssa, cAsycudaItm, dfp, saleitmQtyToallocate);
                        }

                        saleitm.QtyAllocated += saleitmQtyToallocate;

                        ssa.QtyAllocated += saleitmQtyToallocate;

                        saleitmQtyToallocate -= saleitmQtyToallocate;
                    }
                    else
                    {
                        // returns
                        double mqty = 0;

                        if ((Convert.ToDouble(cAsycudaItm.QtyAllocated) == 0))
                        {
                            if (dfp == "Duty Free")
                            {
                                cAsycudaItm.DFQtyAllocated = cAsycudaItm.ItemQuantity;
                            }
                            else
                            {
                                cAsycudaItm.DPQtyAllocated = cAsycudaItm.ItemQuantity;
                            }
                            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                            {
                                SetPreviousItemXbond(ssa, cAsycudaItm, dfp, saleitmQtyToallocate);
                            }
                        }

                        if ((cAsycudaItm.QtyAllocated > Convert.ToDouble(saleitmQtyToallocate*-1)))
                        {
                            mqty = saleitmQtyToallocate*-1;
                        }
                        else
                        {
                            if (cAsycudaItm.QtyAllocated == 0)
                            {
                                mqty = cAsycudaItm.ItemQuantity;
                            }
                            else
                            {
                                mqty = cAsycudaItm.QtyAllocated;
                            }
                            //mqty = Convert.ToDouble(saleitmQtyToallocate * -1);
                        }

                        if (cAsycudaItm.QtyAllocated != 0)
                        {
                            if (subitm != null)
                            {
                                subitm.QtyAllocated = subitm.QtyAllocated - mqty;
                            }
                            if (dfp == "Duty Free")
                            {
                                cAsycudaItm.DFQtyAllocated -= mqty;
                            }
                            else
                            {
                                cAsycudaItm.DPQtyAllocated -= mqty;
                            }

                            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                            {
                                SetPreviousItemXbond(ssa, cAsycudaItm, dfp, -mqty);
                            }
                            saleitmQtyToallocate += mqty;

                            saleitm.QtyAllocated -= mqty;
                                ///saleitm.QtyAllocated + System.Convert.ToDouble(saleitmQtyToallocate);

                            ssa.QtyAllocated -= mqty; //Convert.ToDouble(saleitmQtyToallocate);

                        }
                    }
                }
                //saleitm.AsycudaSalesAllocations = new ObservableCollection<AsycudaSalesAllocations>(saleitm.AsycudaSalesAllocations){ssa};
                await SaveAllocation(ssa).ConfigureAwait(false);
               if(subitm != null) await SaveSubItem(subitm).ConfigureAwait(false);
                await SaveXcuda_Item(cAsycudaItm).ConfigureAwait(false);
                await SaveEntryDataDetails(saleitm).ConfigureAwait(false);

               // saleitm.AsycudaSalesAllocations.Add(ssa);
                return saleitmQtyToallocate;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveAllocation(AsycudaSalesAllocations ssa)
        {
            using (var ctx = new AsycudaSalesAllocationsService())
            {
                await ctx.UpdateAsycudaSalesAllocations(ssa).ConfigureAwait(false);
            }
        }

        private static async Task SaveXcuda_Item(xcuda_Item cAsycudaItm)
        {
            using (var ctx = new xcuda_ItemService())
            {
                await ctx.Updatexcuda_Item(cAsycudaItm).ConfigureAwait(false);
            }
        }

        private static async Task SaveSubItem(SubItems subitm)
        {
            if (subitm == null) return;
            using (var ctx = new SubItemsService())
            {
                await ctx.UpdateSubItems(subitm).ConfigureAwait(false);
            }
        }

        private void SetPreviousItemXbond(AsycudaSalesAllocations ssa, xcuda_Item cAsycudaItm, string dfp, double amt)
        {
            try
            {
                if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate != "Visible") return;


                var alst = cAsycudaItm.xcuda_PreviousItems
                            .Where(x => x.DutyFreePaid == dfp && x.QtyAllocated <= x.Suplementary_Quantity)
                            .Where(x => x.xcuda_Item != null && x.xcuda_Item.AsycudaDocument != null)
                            .OrderBy(
                                    x =>
                                    x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate == null
                                        ? Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate)
                                        : x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate).ToList();
                foreach (var pitm in alst)
                {
                    if (pitm.QtyAllocated == null) pitm.QtyAllocated = 0;
                    var atot = pitm.Suplementary_Quantity - Convert.ToSingle(pitm.QtyAllocated);
                    if (atot == 0) continue;
                    if (amt <= atot)
                    {
                        pitm.QtyAllocated += amt;
                        ssa.xbondEntry.Add(pitm.xcuda_Item);
                        pitm.xcuda_Item.xSalesAllocations.Add(ssa);
                        break;
                    }
                    else
                    {
                        pitm.QtyAllocated += atot;
                        ssa.xbondEntry.Add(pitm.xcuda_Item);
                        pitm.xcuda_Item.xSalesAllocations.Add(ssa);
                        amt -= atot;
                    }

                }

            }
            catch (Exception Ex)
            {
                throw;
            }
        }


      
    }
}
