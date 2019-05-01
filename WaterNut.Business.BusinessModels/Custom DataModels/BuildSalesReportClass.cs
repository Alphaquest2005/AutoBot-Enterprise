using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using TrackableEntities;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using xcuda_PreviousItem = AllocationDS.Business.Entities.xcuda_PreviousItem;
using xcuda_PreviousItemService = AllocationDS.Business.Services.xcuda_PreviousItemService;


namespace WaterNut.DataSpace
{
    public class BuildSalesReportClass
    {
        private static readonly BuildSalesReportClass instance;
        static BuildSalesReportClass()
        {
            instance = new BuildSalesReportClass();
            Initialization = InitializationAsync();
        }

        public static BuildSalesReportClass Instance
        {
            get { return instance; }
        }

        public static Task Initialization { get; private set; }

        private static async Task InitializationAsync()
        {
     

          
        }

      
        public async Task BuildSalesReport(List<AsycudaSalesAllocations> alst)
        {
            StatusModel.StartStatusUpdate("Processing Allocations", alst.Count());


            //foreach (var ssa in alst.OrderBy(x => x.EntryDataDetails.Sales.EntryDataDate))
            for (var i = 0; i < alst.Count(); i++)
            {
                var ssa = alst.ElementAt(i);
                var returnsNExt = false;
                if ((i + 1) < alst.Count())
                {
                    var t = alst.Where(x => x.PreviousDocumentItem == ssa.PreviousDocumentItem && x.EntryDataDetailsId > ssa.EntryDataDetailsId).FirstOrDefault();
                    returnsNExt = (t == null ? false : t.QtyAllocated < 0);
                }
                var mlst = ssa.PreviousDocumentItem.xcuda_PreviousItems

                    .Where(x => BaseDataModel.Instance.GetDocument(x.xcuda_Item.ASYCUDA_Id).Result.xcuda_Identification.xcuda_Registration.Number != null
                                && Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate).Month == ssa.EntryDataDetails.Sales.EntryDataDate.Month
                                && Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate).Year == ssa.EntryDataDetails.Sales.EntryDataDate.Year
                                && x.DutyFreePaid == (ssa.EntryDataDetails.Sales as Sales).DutyFreePaid
                                && x.QtyAllocated <= x.Suplementary_Quantity)
                    .OrderBy(
                        x =>
                            x.xcuda_Item.AsycudaDocument.RegistrationDate).ToList();

                SetXBond(ssa, mlst, returnsNExt);

                if (ssa.xbondEntry.Any() == false)
                {
                    var plst = ssa.PreviousDocumentItem.xcuda_PreviousItems
                        .Where(x =>
                            x.xcuda_Item.AsycudaDocument.CNumber != null
                                //     x.PreviousDocumentItem.xcuda_ASYCUDA.AssessmentDate.Month != ssa.EntryDataDetails.Sales.EntryDataDate.Month
                                //&& x.PreviousDocumentItem.xcuda_ASYCUDA.AssessmentDate.Year != ssa.EntryDataDetails.Sales.EntryDataDate.Year
                                // &&  
                            && x.DutyFreePaid == (ssa.EntryDataDetails.Sales as Sales).DutyFreePaid
                            && x.QtyAllocated <= x.Suplementary_Quantity)
                        .OrderBy(
                            x =>
                                x.xcuda_Item.AsycudaDocument.RegistrationDate).ToList();

                    SetXBond(ssa, plst, returnsNExt);
                }
                StatusModel.StatusUpdate();
            }
        }

        private  void SetXBond(AsycudaSalesAllocations ssa, List<global::AllocationDS.Business.Entities.xcuda_PreviousItem> plst, bool returnsNExt = false)
        {
            var amt = ssa.QtyAllocated;
            foreach (var pitm in plst)
            {
                if (pitm.QtyAllocated == null) pitm.QtyAllocated = 0;
                var atot = pitm.Suplementary_Quantity - pitm.QtyAllocated;
                if (atot == 0 && amt > 0 && returnsNExt == false) continue;

                if (returnsNExt == true && atot == 0 && amt > 0 && pitm != plst.Last()) continue;

                if (ssa.EntryDataDetails.Sales.EntryDataDate < pitm.xcuda_Item.AsycudaDocument.AssessmentDate) continue;



                if (amt <= atot)
                {
                    pitm.QtyAllocated += amt;
                    ssa.xbondEntry.Clear();
                    ssa.xbondEntry.Add(pitm.xcuda_Item);
                    pitm.xcuda_Item.xSalesAllocations.Add(ssa);
                    break;
                }
                else
                {
                    pitm.QtyAllocated += atot;
                    ssa.xbondEntry.Clear();
                    ssa.xbondEntry.Add(pitm.xcuda_Item);
                    pitm.xcuda_Item.xSalesAllocations.Add(ssa);
                    amt -= atot;
                }

            }
        }

        public async Task ReBuildSalesReports()
        {
            
            //try
            //{
            //    if (BaseDataModel.Instance.CurrentApplicationSettings.AllowSalesToPI != "Visible") return;

            await ReLinkPi2Item().ConfigureAwait(false);

            //    var alst = await GetSalesData("Duty Paid").ConfigureAwait(false);

            //    await BuildSalesReportClass(alst).ConfigureAwait(false);//.Where(x => x.CNumber == "29635" && x.PreviousItemEx.LineNumber == 166).ToList()

            //    alst = await GetSalesData("Duty Free").ConfigureAwait(false);

            //    await BuildSalesReportClass(alst).ConfigureAwait(false);//.Where(x => x.CNumber == "29635" && x.PreviousItemEx.LineNumber == 166).ToList()



            //    StatusModel.Timer("ReLoading Data..");


            //    StatusModel.StopStatusUpdate();

            //    MessageBus.Default.BeginNotify(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged, null,
            //                                new NotificationEventArgs(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged));
            //    MessageBox.Show("Complete");
            //}
            //catch (Exception Ex)
            //{
            //    throw;
            //}
        }

        public async Task ReLinkPi2Item()
        {
            StatusModel.Timer("Getting Previous Items");
            List<xcuda_PreviousItem> piLst = null;
            using (var ctx = new xcuda_PreviousItemService())
            {
                piLst = (await ctx.Getxcuda_PreviousItem(new List<string>()
                {
                    "xcuda_Item.AsycudaDocument",
                    "xcuda_Item.EX"
                }).ConfigureAwait(false)).ToList();
            }
            StatusModel.StartStatusUpdate("Re Linking PI to Items", piLst.Count());

         
           // foreach (var pi in piLst
            piLst.AsParallel(new ParallelLinqOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2})
                .ForAll(pi =>
                //  .Where(x => x.Prev_reg_nbr == "39560" && x.Previous_item_number == "25"

            {
                var bl = String.Format("{0} {1} C {2} art. {3}", pi.Prev_reg_cuo,
                    pi.Prev_reg_dat,
                    pi.Prev_reg_nbr, pi.Previous_item_number);
                // find original row
                if (pi.PreviousItem_Id != 0)
                {
                    var pLineNo = Convert.ToInt32(pi.Previous_item_number);
                    // get document

                    xcuda_ASYCUDA pdoc = null;

                    pdoc = BaseDataModel._documentCache.Data.Where(
                        x => x.xcuda_Identification.xcuda_Registration.Number == pi.Prev_reg_nbr)
                        .AsEnumerable()
                        .FirstOrDefault(
                            x =>
                                DateTime.Parse(x.xcuda_Identification.xcuda_Registration.Date).Year.ToString() ==
                                pi.Prev_reg_dat);

                    if (pdoc == null)
                    {
                        //MessageBox.Show(
                        //    string.Format("You need to import Previous Document '{0}' before importing this Ex9 '{1}'",
                        //        pi.Prev_reg_nbr, pi.xcuda_Item.AsycudaDocument.CNumber));
                        return; // continue;
                    }
                    AsycudaDocumentItem Originalitm = null;

                    Originalitm = BaseDataModel._documentItemCache.Data.FirstOrDefault(
                        x =>
                            pi.xcuda_Item != null && pi.xcuda_Item.ItemCost != null && pi.xcuda_Item.ItemNumber != null &&
                            (x.ItemNumber != null && x.ItemNumber.Contains(
                                pi.xcuda_Item.ItemNumber)
                             && x.LineNumber == pLineNo
                             && x.AsycudaDocumentId == pdoc.ASYCUDA_Id));



                    if (Originalitm != null)
                    {
                        if (pi.xcuda_Items.Any(x => x.Item_Id == Originalitm.Item_Id) == false &&
                            Originalitm.PreviousItems.Any(x => x.PreviousItem_Id == pi.PreviousItem_Id) == false)
                        {
                            var epi = new EntryPreviousItems()
                            {
                                PreviousItem_Id = pi.PreviousItem_Id,
                                Item_Id = Originalitm.Item_Id,
                                TrackingState = TrackingState.Added
                            };
                            // await DocumentItemDS.ViewModels.BaseDataModel.Instance.SaveEntryPreviousItems(epi).ConfigureAwait(false);
                            DocumentItemDS.DataModels.BaseDataModel.Instance.SaveEntryPreviousItems(epi).Wait();
                        }


                    }
                    else
                    {
                        //MessageBox.Show(
                        //    string.Format("Item Not found {0} line: {1} PrevCNumber: {2} CNumber: {3}",
                        //        pi.xcuda_Item.EX.Precision_4, pLineNo, pdoc.xcuda_Identification.xcuda_Registration.Number,
                        //        pi.xcuda_Item.AsycudaDocument.CNumber));
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Item Not found {0}, LineNo:-{1}",
                        bl, pi.Current_item_number));
                }
                StatusModel.StatusUpdate();
            }
                );


        }


    }
}