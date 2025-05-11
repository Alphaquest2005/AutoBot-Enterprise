using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.EF6;
using EntryPreviousItems = CoreEntities.Business.Entities.EntryPreviousItems;
using xBondAllocations = AllocationDS.Business.Entities.xBondAllocations;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_PreviousItem = AllocationDS.Business.Entities.xcuda_PreviousItem;

namespace WaterNut.DataSpace
{
    public class BuildSalesReportClass
    {
        private readonly ConcurrentDictionary<int, (xcuda_PreviousItem Pi, double SalesFactor)> piBag =
            new ConcurrentDictionary<int, (xcuda_PreviousItem Pi, double SalesFactor)>();

        static BuildSalesReportClass()
        {
            Instance = new BuildSalesReportClass();
            Initialization = InitializationAsync();
        }

        public static BuildSalesReportClass Instance { get; }

        public static Task Initialization { get; }

        private static Task InitializationAsync()
        {
            return Task.CompletedTask;
        }


        public void BuildSalesReport() //string dfp
        {
            StatusModel.Timer("Processing Allocations"); //, alst.Count()
            var exceptions = new ConcurrentQueue<Exception>();

            var plst = new AllocationDSContext().xcuda_PreviousItem
                .Include(x => x.xcuda_Item.AsycudaDocument)
                .OrderBy(x => x.xcuda_Item.AsycudaDocument.AssessmentDate)
                .OrderBy(x => x.xcuda_Item.AsycudaDocument.CNumber)
                .OrderBy(x => x.xcuda_Item.LineNumber)

                //   .Where(x => x.Prev_decl_HS_spec == "INT/YBA473GL")
                .ToList();

            var alst = new AllocationDSContext().AsycudaSalesAllocations
                .Include(x => x.EntryDataDetails.Sales)
                //.OrderBy(x => x.EntryDataDetails.Sales.EntryDataDate)
                //.OrderBy(x => x.EntryDataDetails.Sales.EntryDataDate)
                .OrderBy(x => x.AllocationId)
                .Where(x => x.EntryDataDetails.Sales != null)
                //.Where(x => x.EntryDataDetails.ItemNumber == "INT/YBA473GL")
                .ToList()
                .GroupBy(x => x.EntryDataDetails.ItemNumber);

            //TODO: Implement itemAliases
            var ps = alst.GroupJoin(plst, allocations => allocations.Key,
                previousItems => previousItems.Prev_decl_HS_spec,
                (allocations, items) => new {allocations = allocations.ToList(), PList = items.ToList()});


            //Parallel.ForEach(alst, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, g =>//

            // var ps = alst.SelectMany(x => x.Pi).DistinctBy(x => x.Pi.PreviousItem_Id).OrderBy(x => x.pAssessmentDate).ThenBy(x => x.pRegDate);
            //var ps =
            //	alst.OrderBy(x => x.SalesDate).GroupBy(x => new {  x.DutyFreePaid, x.SalesDate })// ,x.Item_Id,
            //		.Select(
            //			x =>
            //				new
            //				{
            //					//x.Key.Item_Id,
            //					Allocations = x.Select(z =>  z.Allocation).OrderBy(z => z.AllocationId),
            //					PList = x.SelectMany(z => z.Pi)
            //                                    //.Where(z => z.pAssessmentDate.Month == x.Key.SalesDate.Month && z.pAssessmentDate.Year == x.Key.SalesDate.Year)
            //					    .Where(z => z.xAssessmentDate <= x.Key.SalesDate)
            //                                .DistinctBy(z => z.Pi.PreviousItem_Id)
            //                                .OrderBy(z => z.xAssessmentDate).ThenBy(z => z.xRegDate),//
            //					x.Key.DutyFreePaid

            //				}).ToList();


            //	foreach (var g in ps)
            Parallel.ForEach(ps, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount * 1}, g =>
            {
                try
                {
                    //var lst = alst.Where(x => x.Pi.Any(z => z.Pi.PreviousItem_Id == g.Pi.PreviousItem_Id)).Select(x => x.Allocation);   

                    SetXBondLst(g.allocations.OrderBy(x => x.AllocationId).ToList(),
                        g.PList.OrderBy(z => z.xcuda_Item.AsycudaDocument.AssessmentDate)
                            .ThenBy(z => z.xcuda_Item.AsycudaDocument.RegistrationDate)
                            .ThenBy(z => z.xcuda_Item.LineNumber).Select(x => (Pi: x, x.xcuda_Item.SalesFactor))
                            .ToList());

                    StatusModel.StatusUpdate();
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
                //	}
            });

            MarkOverAllocations();


            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        private void ClearXbondAllocations()
        {
            using (var ctx = new AllocationDSContext {StartTracking = true})
            {
                ctx.Database.ExecuteSqlCommand($@"DELETE FROM xBondAllocations
			                                        FROM    xBondAllocations INNER JOIN
			                                        xcuda_Item ON xBondAllocations.xEntryItem_Id = xcuda_Item.Item_Id INNER JOIN
			                                        xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
			                                        AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
			                                        WHERE(xcuda_Item.IsAssessed = 1) 
                                                    AND(AsycudaDocumentSet.ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId})");

                ctx.Database.ExecuteSqlCommand($@"UPDATE xcuda_PreviousItem
                                                    SET         QtyAllocated = 0
                                                    FROM    AsycudaDocument INNER JOIN
                                                                     xcuda_PreviousItem ON AsycudaDocument.ASYCUDA_Id = xcuda_PreviousItem.ASYCUDA_Id
                                                    WHERE (AsycudaDocument.AsycudaDocumentSetId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId})");
            }
        }

        private void SetXBondLst(List<AsycudaSalesAllocations> slst,
            List<(xcuda_PreviousItem Pi, double SalesFactor)> plst)
        {
            //TODO: replace this with ex9 allocation code same concept and problems and checks too

            if (plst == null || !plst.Any()) return;
            var pn = 0;
            var pitm = plst[pn];

            plst.ForEach(x =>
            {
                if (piBag.ContainsKey(x.Pi.PreviousItem_Id)) return;
                var pi = piBag.GetOrAdd(x.Pi.PreviousItem_Id, x);
                pi.Pi.QtyAllocated = 0;
            });

            for (var i = 0; i < slst.Count(); i++)
            {
                var ssa = slst.ElementAt(i);

                var piQty = (double) pitm.Pi.Suplementary_Quantity * pitm.SalesFactor;

                var remainingSalesQty = slst.Skip(i).Sum(x => x.QtyAllocated);
                if (ssa.QtyAllocated < 0 && pitm.Pi.QtyAllocated * pitm.SalesFactor <= piQty)
                {
                    pn -= 1;
                    if (pn < 0) pn = 0;
                    pitm = piBag.GetOrAdd(plst[pn].Pi.PreviousItem_Id, plst[pn]);
                }

                if (pitm.Pi.QtyAllocated * pitm.SalesFactor >= piQty && remainingSalesQty > 0)
                {
                    var p1 = pn + 1 <= plst.Count - 1 ? plst[pn + 1] : plst[pn];
                    var pdate = p1.Pi.xcuda_Item.AsycudaDocument.AssessmentDate;
                    if (pn + 1 <= plst.Count - 1 && pdate <= ssa.EntryDataDetails.Sales.EntryDataDate &&
                        ssa.QtyAllocated > 0)
                    {
                        pn += 1;
                        pitm = piBag.GetOrAdd(plst[pn].Pi.PreviousItem_Id, plst[pn]);
                        i -= 1; // set back to keep same sale
                        continue;
                    }

                    //return;
                    //if (remainingSalesQty > 0 && pn == plst.Count - 1)
                    //{
                    // allow over allocation
                    SetXBond(ssa, pitm.Pi).Wait();
                    SaveXbond(ssa, pitm.Pi).Wait();
                    continue;

                    //}
                    //else
                    //{

                    //}

                    // return;
                }

                if (piQty - pitm.Pi.QtyAllocated * pitm.SalesFactor >= ssa.QtyAllocated)
                {
                    //allocate sale and continue to next sale
                    SetXBond(ssa, pitm.Pi).Wait();
                    SaveXbond(ssa, pitm.Pi).Wait();
                    // pitm.Pi.QtyAllocated += ssa.QtyAllocated;
                    //SavePitm(pitm.Pi).Wait();
                    continue;
                }

                //clean out pi item and go next pitm
                SetXBond(ssa, pitm.Pi).Wait();
                SaveXbond(ssa, pitm.Pi).Wait();
                // pitm.Pi.QtyAllocated += (piQty - pitm.Pi.QtyAllocated * pitm.SalesFactor);
                //SavePitm(pitm.Pi).Wait();
                i -= 1; // to keep the same sale
                continue;

                if (Math.Abs(remainingSalesQty) < 0 &&
                    Math.Abs(piQty - pitm.Pi.QtyAllocated * pitm.SalesFactor) < .0001) //
                {
                    slst.Skip(i).ForEach(async x => await SaveXbond(x, pitm.Pi).ConfigureAwait(false));
                    return;
                }
            }
        }


        private Task SetXBond(AsycudaSalesAllocations ssa, xcuda_PreviousItem pitm)
        {
            var amt = ssa.QtyAllocated;
            var atot = (double) pitm.Suplementary_Quantity - pitm.QtyAllocated;


            if (amt <= atot)
            {
                pitm.QtyAllocated += amt;

                //await SaveXbond(ssa, pitm).ConfigureAwait(false);
                //await SavePitm(pitm).ConfigureAwait(false);
            }
            else
            {
                if (atot <= 0 && amt > 0)
                {
                    pitm.QtyAllocated += amt;
                    ssa.QtyAllocated = atot;
                    ssa.Status = "OverAllocated"; //Todo: might neeed to save status
                    return Task.CompletedTask;
                }

                if (atot <= 0 && amt < 0)
                {
                    pitm.QtyAllocated += amt;
                    ssa.QtyAllocated += amt * -1;
                    //ssa.Status = "OverAllocated";//Todo: might neeed to save status
                    return Task.CompletedTask;
                }

                pitm.QtyAllocated += atot;


                ssa.QtyAllocated = atot;
            }

            return Task.CompletedTask;
        }

        private async Task SaveXbond(AsycudaSalesAllocations ssa, xcuda_PreviousItem pitm)
        {
            var xbond = new xBondAllocations(true)
            {
                AllocationId = ssa.AllocationId,
                xEntryItem_Id = pitm.PreviousItem_Id,
                Status = ssa.Status,
                TrackingState = TrackingState.Added
            };
            await AllocationDS.DataModels.BaseDataModel.Instance.SavexBondAllocations(xbond)
                .ConfigureAwait(false);
        }

        private Task SavePitm(xcuda_PreviousItem pitm)
        {
            //	var res = pitm.ChangeTracker.GetChanges().FirstOrDefault();
            if (pitm == null) return Task.CompletedTask;
            //var sql = $@"INSERT INTO xcuda_PreviousItem
            //                   (Packages_number, Previous_Packages_number, Hs_code, Commodity_code, Previous_item_number, Goods_origin, Net_weight, Prev_net_weight, Prev_reg_ser, Prev_reg_nbr, Prev_reg_dat, Prev_reg_cuo, 
            //                   Suplementary_Quantity, Preveious_suplementary_quantity, Current_value, Previous_value, Current_item_number, ASYCUDA_Id, QtyAllocated)
            //                  VALUES        ({res.Packages_number},{res.Previous_Packages_number},{res.Hs_code},{res.Commodity_code},{res.Previous_item_number},{res.Goods_origin},{res.Net_weight},{res.Prev_net_weight},{res.Prev_reg_ser},{res.Prev_reg_nbr},{res.Prev_reg_dat},{res.Prev_reg_cuo},
            //                                  {res.Suplementary_Quantity},{res.Preveious_suplementary_quantity},{res.Current_value},{res.Previous_value},{res.Current_item_number},{res.ASYCUDA_Id},{res.QtyAllocated})";
            //await AllocationDS.DataModels.BaseDataModel.Instance.Savexcuda_PreviousItem(res)
            //	.ConfigureAwait(false);
            using (var ctx = new AllocationDSContext())
            {
                ctx.xcuda_PreviousItem.Attach(pitm);
                ctx.ApplyChanges(pitm);
                ctx.SaveChanges();
            }

            return Task.CompletedTask;
        }

        private async Task SaveAllocation(AsycudaSalesAllocations pitm)
        {
            var res = pitm.ChangeTracker.GetChanges().FirstOrDefault();

            await AllocationDS.DataModels.BaseDataModel.Instance.SaveAsycudaSalesAllocations(res)
                .ConfigureAwait(false);
        }

        public async Task ReBuildSalesReports(string id)
        {
            using (var ctx = new AllocationDSContext())
            {
                var doc = ctx.AsycudaDocument.Include(x => x.Customs_Procedure).FirstOrDefault(x => x.id == id);
                await ReBuildSalesReports(doc).ConfigureAwait(false);
            }
        }

        public async Task ReBuildSalesReports(AsycudaDocument doc)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowSalesToPI != "Visible") return;
            var plst = await GetPreviousItems(doc).ConfigureAwait(false);
            await ReLinkPi2Item(plst).ConfigureAwait(false);
            string dfp;
            if (doc.Customs_Procedure.IsPaid == true)
                dfp = "Duty Paid";
            else
                dfp = "Duty Free";
            var alst = await GetSalesData(dfp, doc.ASYCUDA_Id).ConfigureAwait(false);
            //await BuildSalesReport(alst, dfp).ConfigureAwait(false);//.Where(x => x.pCNumber == "29635" && x.PreviousItemEx.LineNumber == 166).ToList()
        }

        public void ReBuildSalesReports()
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowSalesToPI != "Visible") return;

            /////////////////////////////////////////////////////////////////////////////////////////////

            //	var piLst = await GetPreviousItems().ConfigureAwait(false);

            //	await ReLinkPi2Item(piLst.Where(x => x.Prev_decl_HS_spec == "A002416").ToList()).ConfigureAwait(false);


            //////////////////////////////////////////////////////////////////////////////////////////////

            //var alst = await GetSalesData("Duty Paid").ConfigureAwait(false);

            //await BuildSalesReport(alst, "Duty Paid").ConfigureAwait(false);//.Where(x => x.pCNumber == "29635" && x.PreviousItemEx.LineNumber == 166).ToList()

            // var alst = (await GetSalesData("Duty Free").ConfigureAwait(false));

            ClearXbondAllocations();
            piBag.Clear();

            BuildSalesReport();


            //await BuildSalesReport("Duty Free").ConfigureAwait(false);
        }

        private void MarkOverAllocations()
        {
            if (piBag.Any())
            {
                var res = piBag.Where(x =>
                    x.Value.Pi.QtyAllocated > Convert.ToDouble(x.Value.Pi.Suplementary_Quantity));
                Parallel.ForEach(res
                    ,
                    new ParallelOptions {MaxDegreeOfParallelism = 1}, i => //Environment.ProcessorCount*
                    {
                        using (var ctx = new AllocationDSContext {StartTracking = false})
                        {
                            var sql = "";

                            if (ctx.xBondAllocations == null) return;

                            var lst =
                                ctx.xBondAllocations
                                    .Include(x => x.AsycudaSalesAllocations)
                                    .Where(x => x != null && x.xEntryItem_Id == i.Key)
                                    .OrderByDescending(x => x.AllocationId)
                                    .DistinctBy(x => x.AllocationId)
                                    .ToList();

                            foreach (var allo in lst)
                            {
                                var tot = i.Value.Pi.QtyAllocated - (double) i.Value.Pi.Suplementary_Quantity;
                                var r = tot > allo.AsycudaSalesAllocations.QtyAllocated / i.Value.SalesFactor
                                    ? allo.AsycudaSalesAllocations.QtyAllocated / i.Value.SalesFactor
                                    : tot;
                                if (i.Value.Pi.QtyAllocated > (double) i.Value.Pi.Suplementary_Quantity)
                                {
                                    allo.AsycudaSalesAllocations.QtyAllocated -= r;
                                    i.Value.Pi.QtyAllocated -= r;
                                    /////// is the same thing
                                    sql += $@" UPDATE       xcuda_PreviousItem
															SET                QtyAllocated = (QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	PreviousItem_Id = {i.Key}";


                                    if (allo.AsycudaSalesAllocations.QtyAllocated == 0.0)
                                    {
                                        //                              allo.AsycudaSalesAllocations.QtyAllocated =
                                        //                                  r; //add back so wont disturb calculations
                                        //                              allo.Status = $"Over Allocated by {r}";

                                        //                              sql += $@"  Update xBondAllocations
                                        //Set Status = '{allo.Status}'
                                        //Where xBondAllocationId = {allo.xBondAllocationId}";
                                        sql += $@"  Delete xBondAllocations
														Where xBondAllocationId = {allo.xBondAllocationId}";
                                    }
                                    else
                                    {
                                        sql += $@" INSERT INTO xBondAllocations
														 (xEntryItem_Id, AllocationId, Status)
														VALUES        ({allo.xEntryItem_Id},{allo.AllocationId},'Over Allocated by {r}')";

                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }


                            if (!string.IsNullOrEmpty(sql))
                                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
                        }
                    });
            }
        }

        private async Task<List<AsycudaSalesAllocations>> GetSalesData(string dfp, int Asycuda_Id)
        {
            var expLst = new List<string>
            {
                "EntryDataDetails.Sales != null",
                $"EntryDataDetails.TaxAmount {(dfp == "Duty Free" ? "== 0" : "!= 0")}",
                $"EntryDataDetails.Sales.EntryDataDate >= \"{"5/22/2015"}\" && EntryDataDetails.Sales.EntryDataDate <= \"{"5/31/2015 11:00 pm"}\"",
                "PreviousDocumentItem != null",
                "PreviousDocumentItem.EntryPreviousItems.Any(xcuda_PreviousItem.ASYCUDA_Id == \"" + Asycuda_Id + "\")"
                // "PreviousDocumentItem.AsycudaDocument.pCNumber == \"22699\""
                //, "EntryDataDetails.ItemNumber = \"ASA/2247010\""
            };

            return await GetSalesData(expLst).ConfigureAwait(false);
        }

        private Task<List<AsycudaSalesAllocations>> GetSalesData(string dfp)
        {
            //var expLst = new List<string>()
            //{
            //    "EntryDataDetails.Sales != null",
            //    (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? string.Format("EntryDataDetails.Sales.EntryDataDate >= \"{0}\"", BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate) : "EntryDataDetails.Sales.EntryDataDate >= \"1/1/2010\""),
            //    string.Format("EntryDataDetails.Sales.EntryDataDate >= \"{0}\" && EntryDataDetails.Sales.EntryDataDate <= \"{1}\"","5/22/2015", "6/30/2015 11:00 pm"),
            //    string.Format("EntryDataDetails.Sales.TaxAmount {0}", dfp == "Duty Free" ? "== 0" : "!= 0"),
            //    "PreviousDocumentItem != null",
            //    "PreviousDocumentItem.AsycudaDocument != null",
            //   // "PreviousDocumentItem.AsycudaDocument.pCNumber == \"22699\""
            //   //, "EntryDataDetails.ItemNumber = \"ASA/2247010\""
            //};


            // return await GetSalesData(expLst).ConfigureAwait(false);
            return Task.FromResult<List<AsycudaSalesAllocations>>(null);
        }

        private async Task<List<AsycudaSalesAllocations>> GetSalesData(List<string> expLst)
        {
            StatusModel.Timer("Loading Data...");
            using (var ctx = new AsycudaSalesAllocationsService())
            {
                var tot = await ctx.CountByExpressionLst(expLst).ConfigureAwait(false);
                var alst = await ctx
                    .GetAsycudaSalesAllocationsByBatchExpressionLst(expLst, tot, new List<string>(), false)
                    .ConfigureAwait(false);

                var res = alst.OrderBy(x => x.EntryDataDetails.Sales.EntryDataDate)
                    .ThenBy(x => x.EntryDataDetails.EntryDataDetailsId)
                    .ToList();

                StatusModel.Timer("Cleaning Data...");
                res.ForEach(
                    x =>
                        x.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                            .ToList()
                            .ForEach(z => z.QtyAllocated = 0));
                res.ForEach(x => x.xBondAllocations.Clear());
                return res;
            }
        }

        public Task ReLinkPi2Item(List<xcuda_PreviousItem> piLst)
        {
            StatusModel.Timer("Getting Previous Items");


            StatusModel.StartStatusUpdate("Re Linking PI to Items", piLst.Count());


            var ilst = piLst.GroupBy(x => new
                {x.Prev_reg_nbr, x.Prev_reg_year, x.Prev_reg_cuo, x.Previous_item_number, x.xcuda_Item});

            // foreach (var g in ilst)
            var exceptions = new ConcurrentQueue<Exception>();

            ilst.AsParallel(new ParallelLinqOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}) //
                .ForAll(g =>
                {
                    using (var dtx = new DocumentDSContext())
                    {
                        using (var ctx = new DocumentItemDSContext())
                        {
                            //foreach (var g in ilst)
                            //{
                            try
                            {
                                var bl =
                                    $"{g.Key.Prev_reg_cuo} {g.Key.Prev_reg_year} C {g.Key.Prev_reg_nbr} art. {g.Key.Previous_item_number}";

                                var pLineNo = Convert.ToInt32(g.Key.Previous_item_number);
                                // get document

                                xcuda_ASYCUDA pdoc = null;

                                pdoc = dtx.xcuda_ASYCUDA.FirstOrDefault(
                                    x =>
                                        x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.xcuda_Identification.xcuda_Registration.Date != null &&
                                        x.xcuda_ASYCUDA_ExtendedProperties.Cancelled != true &&
                                        ((DateTime)x.xcuda_Identification.xcuda_Registration.Date).Year == g.Key.Prev_reg_year
                                        && x.xcuda_Identification.xcuda_Registration.Number == g.Key.Prev_reg_nbr &&
                                        x.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code ==
                                        g.Key.Prev_reg_cuo);

                                if (pdoc == null)
                                    throw new ApplicationException(
                                        $"You need to import Previous Document '{g.Key.Prev_reg_nbr}' before importing this Ex9 '{g.Key.xcuda_Item.AsycudaDocument.CNumber}'");
                                xcuda_Item Originalitm = null;
                                if (g.Key.xcuda_Item != null)
                                {
                                    var itmNumber = g.Key.xcuda_Item.ItemNumber;
                                    if (itmNumber != null) //&& Math.Abs(g.Key.xcuda_Item.ItemCost) > 0
                                        Originalitm = ctx.xcuda_Item.Include(x => x.xcuda_PreviousItems).FirstOrDefault(
                                            x =>
                                                x.xcuda_Tarification.xcuda_HScode.Precision_4 != null &&
                                                itmNumber.Contains(x.xcuda_Tarification.xcuda_HScode.Precision_4)
                                                && x.LineNumber == pLineNo
                                                && x.ASYCUDA_Id == pdoc.ASYCUDA_Id);
                                }


                                if (Originalitm != null)
                                {
                                    var epilst = new List<EntryPreviousItems>();
                                    foreach (var pi in g)
                                        if ( //pi.xcuda_Items.Any(x => x.Item_Id == Originalitm.Item_Id) == false &&
                                            Originalitm.xcuda_PreviousItems.Any(x =>
                                                x.PreviousItem_Id == pi.PreviousItem_Id) ==
                                            false)
                                        {
                                            var epi = new EntryPreviousItems(true)
                                            {
                                                PreviousItem_Id = pi.PreviousItem_Id,
                                                Item_Id = Originalitm.Item_Id,
                                                TrackingState = TrackingState.Added
                                            };
                                            // await DocumentItemDS.ViewModels.BaseDataModel.Instance.SaveEntryPreviousItems(epi).ConfigureAwait(false);
                                            epilst.Add(epi);
                                            //Originalitm.PreviousItems.Add(epi);
                                        }

                                    BaseDataModel.Instance.SaveEntryPreviousItems(epilst).Wait();
                                }
                            }


                            catch
                                (Exception ex)
                            {
                                exceptions.Enqueue(ex);
                            }
                            //}
                        }
                    }
                });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return Task.CompletedTask;
        }

        private async Task<List<xcuda_PreviousItem>> GetPreviousItems(AsycudaDocument doc)
        {
            List<xcuda_PreviousItem> piLst = null;
            using (var ctx = new xcuda_PreviousItemService())
            {
                piLst =
                    (await
                        ctx.Getxcuda_PreviousItemByExpression(
                            "xcuda_Item.AsycudaDocument.pCNumber == \"" + doc.CNumber + "\"",
                            new List<string> // && PreviousItem_Id == 294486
                            {
                                "xcuda_Item.AsycudaDocument",
                                "xcuda_Item.xcuda_Tarification.xcuda_HScode"
                            }).ConfigureAwait(false)).ToList();
            }

            return piLst;
        }

        private async Task<List<xcuda_PreviousItem>> GetPreviousItems()
        {
            List<xcuda_PreviousItem> piLst = null;
            using (var ctx = new xcuda_PreviousItemService())
            {
                piLst =
                    (await
                        ctx.Getxcuda_PreviousItemByExpression(
                            $"(xcuda_Item.AsycudaDocument.ApplicationSettingsId == {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId} ) && " +
                            "(xcuda_Item.AsycudaDocument.Cancelled != true) && " +
                            "(xcuda_Item.AsycudaDocument.pCNumber != null " +
                            "|| xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) " +
                            "&& " +
                            $"xcuda_Item.AsycudaDocument.AssessmentDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"",
                            new List<string> // && PreviousItem_Id == 294486
                            {
                                "xcuda_Item.AsycudaDocument",
                                "xcuda_Item.xcuda_Tarification.xcuda_HScode"
                            }).ConfigureAwait(false)).ToList();
            }

            return piLst;
        }


        //   public IEnumerable<AllocationPi> GetAllocations()
        //{


        //	using (var ctx = new AllocationDSContext() {StartTracking = true})
        //	{
        //		try
        //		{

        //			ctx.Configuration.LazyLoadingEnabled = false;
        //			ctx.Configuration.AutoDetectChangesEnabled = false;


        //			var res = ctx.AsycudaSalesAllocations
        //       //               .Include(x => x.EntryDataDetails.Sales)
        //			    //.Include("PreviousDocumentItem.EntryPreviousItems.xcuda_Item.AsycudaDocument")
        //			    //.Include("PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
        //                      .Where(x => x.EntryDataDetails.Sales != null 

        //                              //   && "17997453".Contains(x.EntryDataDetails.ItemNumber) && x.EntryDataDetails.Sales.EntryDataDate.Month == 11 && x.EntryDataDetails.Sales.EntryDataDate.Year == 2016
        //		                && x.EntryDataDetails.Sales.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId

        //                              && x.EntryDataDetails.ItemNumber == "A002416"


        //                                  && x.PreviousDocumentItem != null).Where(x => x.PreviousDocumentItem.EntryPreviousItems.Any(
        //								z => z.xcuda_Item.AsycudaDocument != null
        //                                         // && x.EntryDataDetails.Sales.EntryDataDate >= z.xcuda_Item.AsycudaDocument.AssessmentDate
        //                                         // && x.EntryDataDetails.Sales.EntryDataDate >= z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate

        //                                          && z.xcuda_Item.AsycudaDocument.Cancelled != true
        //									&& z.xcuda_Item.AsycudaDocument.DoNotAllocate != true

        //									&& (z.xcuda_Item.AsycudaDocument.CNumber != null || z.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
        //								    && z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Cancelled != true
        //								    && z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.DoNotAllocate != true
        //                                          && z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Exwarehouse 
        //                                          && z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure.IsPaid == ((x.EntryDataDetails.TaxAmount??0) > 0)) 
        //							&& x.PreviousDocumentItem.AsycudaDocument != null
        //							&& x.PreviousDocumentItem.AsycudaDocument.Cancelled != true)
        //				.OrderBy(x => x.EntryDataDetails.Sales.EntryDataDate)
        //				.ThenBy(x => x.EntryDataDetails.EntryDataDetailsId)
        //				    .Select(x => new AllocationPi
        //				{
        //					Allocation = x,
        //					SalesDate = x.EntryDataDetails.Sales.EntryDataDate,
        //					Item_Id = x.PreviousDocumentItem.Item_Id,
        //					DutyFreePaid = (x.EntryDataDetails.TaxAmount > 0 ? "Duty Paid" : "Duty Free"),
        //                          Pi = x.PreviousDocumentItem.EntryPreviousItems // already filtered out
        //                   //       .Where(p => p.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Cancelled != true
        //																		 //&&   x.EntryDataDetails.Sales.EntryDataDate >= p.xcuda_Item.AsycudaDocument.AssessmentDate
        //                   //                                                            && p.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Extended_customs_procedure == (x.EntryDataDetails.Sales.TaxAmount == 0?"9070": "4070"))

        //					.Select(p => new DatedPi
        //					{
        //						Pi = p.xcuda_PreviousItem,

        //                              SalesFactor = p.xcuda_PreviousItem.xcuda_Item.SalesFactor,
        //                              pCNumber = p.xcuda_Item.AsycudaDocument.CNumber,
        //						pAssessmentDate = p.xcuda_Item.AsycudaDocument.AssessmentDate ?? DateTime.MinValue,
        //						pRegDate = (p.xcuda_Item.AsycudaDocument.RegistrationDate ?? DateTime.MinValue),
        //					    xCNumber = p.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.CNumber,
        //                              xAssessmentDate = p.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate??DateTime.MinValue,
        //					    xRegDate = (p.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.RegistrationDate?? DateTime.MinValue),
        //                          }).OrderBy(q => q.xAssessmentDate).ThenBy(q => q.xRegDate),//TODO: check exwarehouseing sorting for updated sorting


        //                      })
        //				.Where(x => x.Pi.Any())
        //                      .ToList()
        //				.OrderBy(x => x.Pi.First().xAssessmentDate)
        //				.ThenBy(x => x.Pi.First().xRegDate);
        //			return res;


        //		}
        //		catch (Exception)
        //		{

        //			throw;
        //		}
        //	}

        //}


        public class DatedPi
        {
            public xcuda_PreviousItem Pi { get; set; }
            public DateTime pAssessmentDate { get; set; }
            public DateTime? pRegDate { get; set; }
            public string pCNumber { get; set; }
            public double SalesFactor { get; set; }
            public DateTime xRegDate { get; set; }
            public DateTime xAssessmentDate { get; set; }
            public string xCNumber { get; set; }
        }

        public class AllocationPi
        {
            public AsycudaSalesAllocations Allocation { get; set; }
            public IEnumerable<DatedPi> Pi { get; set; }
            public int Item_Id { get; set; }
            public string DutyFreePaid { get; internal set; }
            public DateTime SalesDate { get; internal set; }
        }

        public class PiAllocations
        {
            public List<AsycudaSalesAllocations> Allocations { get; set; }
            public DatedPi Pi { get; set; }
        }
    }
}