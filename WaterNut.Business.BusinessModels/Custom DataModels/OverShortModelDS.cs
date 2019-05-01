using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using OversShortQS.Business.Entities;
using OversShortQS.Business.Services;
using SimpleMvvmToolkit;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using AsycudaDocumentItem = OversShortQS.Business.Entities.AsycudaDocumentItem;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;

namespace WaterNut.DataSpace
{
    public class OverShortModelDS
    {
        private static readonly OverShortModelDS instance;

        static OverShortModelDS()
        {
            instance = new OverShortModelDS();
        }

        public static OverShortModelDS Instance
        {
            get { return instance; }
        }

        public async Task Import(string fileName, string fileType, AsycudaDocumentSet docSet, bool overWriteExisting)
        {
            await SaveCSVModel.Instance.ProcessDroppedFile(fileName, fileType, docSet, overWriteExisting).ConfigureAwait(false);
           
        }

        internal async Task SaveReferenceNumber(IEnumerable<OversShortEX> slst, string refTxt)
        {
           
            if (slst.Any() || string.IsNullOrEmpty(refTxt)) return;
            using (var ctx = new OverShortSuggestedDocumentService())
            {
                foreach (var os in slst)
                {
                    var s = new OverShortSuggestedDocument()
                    {
                        OversShortsId = os.OversShortsId,
                        ReferenceNumber = refTxt,
                        TrackingState = TrackingState.Added
                    };
                    await ctx.UpdateOverShortSuggestedDocument(s).ConfigureAwait(false);
                }
            }


        }

        internal async Task SaveCNumber(IEnumerable<OversShortEX> slst, string cntxt)
        {
            
            if (slst.Any() || string.IsNullOrEmpty(cntxt)) return;
            using (var ctx = new OverShortSuggestedDocumentService())
            {
                foreach (var os in slst)
                {
                    var s = new OverShortSuggestedDocument()
                    {
                        OversShortsId = os.OversShortsId,
                        CNumber = cntxt,
                        TrackingState = TrackingState.Added
                    };
                    await ctx.UpdateOverShortSuggestedDocument(s).ConfigureAwait(false);
                }
            }

         
        }



        internal async Task MatchEntries(IEnumerable<OversShortEX> olst)
        {
            IEnumerable<IGrouping<OversShortEX, OverShortDetailsEX>> lst;
            
            if (!olst.Any())
            {
                using (var ctx = new OverShortDetailsEXService())
                {
                    lst = (await ctx.GetOverShortDetailsByExpression("ReceivedQty < InvoiceQty", new List<string>()
                    {
                        "OverShort"
                    }).ConfigureAwait(false)).GroupBy(x => x.OversShortEX);
                }
            }
            else
            {
                lst =
                    olst.SelectMany(x => x.OverShortDetailsEXes)
                        .Where(x => x.ReceivedQty < x.InvoiceQty)
                        .GroupBy(x => x.OversShortEX);
            }

            await MatchAscyudaEntriesToShorts(lst).ConfigureAwait(false);
        }

        internal async Task DoMatch(List<AsycudaDocumentItem> alst, OverShortDetailsEX osd)
        {
            var remainingShortQty = osd.ShortQuantity;
            if (!alst.Any())
            {
                osd.Status = "No Asycuda Entry Found";
                await UpdateOverShortDetail(osd).ConfigureAwait(false);
                return;
            }

            foreach (var ai in alst)
            {
                var asycudaQty = Convert.ToDouble(ai.ItemQuantity - ai.PiQuantity);
                var osa = new OverShortDetailAllocation()
                {
                    Item_Id = ai.Item_Id,
                    OverShortDetailId = osd.OverShortDetailId,
                    TrackingState = TrackingState.Added
                };
                osd.OverShortDetailAllocations.Add(osa);
                if (asycudaQty < remainingShortQty)
                {
                    osa.Status = "Not Enough Qty";
                    osa.QtyAllocated = asycudaQty;
                    remainingShortQty -= asycudaQty;
                }
                else
                {
                    osa.QtyAllocated = remainingShortQty;
                    remainingShortQty = 0;
                    await CreateOverShortDetailAllocation(osa).ConfigureAwait(false);
                    break;
                }
                await CreateOverShortDetailAllocation(osa).ConfigureAwait(false);
            }
            if (remainingShortQty != 0)
            {
                osd.Status = String.Format("Insufficent Qty - Remaining Qty:{0}", remainingShortQty);
                await UpdateOverShortDetail(osd).ConfigureAwait(false);
            }

        }

        private  async Task CreateOverShortDetailAllocation(OverShortDetailAllocation osa)
        {
            using (var ctx = new OverShortDetailAllocationService())
            {
                await ctx.CreateOverShortDetailAllocation(osa).ConfigureAwait(false);
            }
        }

        private  async Task UpdateOverShortDetail(OverShortDetail osd)
        {
            using (var ctx = new OverShortDetailService())
            {
                await ctx.UpdateOverShortDetail(osd).ConfigureAwait(false);
            }
        }

        private async Task MatchAscyudaEntriesToShorts(IEnumerable<IGrouping<OversShortEX, OverShortDetailsEX>> ilst)
        {
            try
            {

                var lst = ilst.ToList();
                StatusModel.StartStatusUpdate("Matching O/S To Asycuda Entries", lst.Count());
                foreach (var osg in lst)
                {
                    StatusModel.StatusUpdate("Matching O/S To Asycuda Entries");
                    IGrouping<OversShortEX, OverShortDetailsEX> os = osg;
                    var alst = await GetAsycudaEntriesByCNumberThenReferenceNumber(os).ConfigureAwait(false);
                    await SetMatch(os, alst).ConfigureAwait(false);
                }

                StatusModel.StopStatusUpdate();
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IEnumerable<AsycudaDocumentItem>> GetAsycudaEntriesByCNumberThenReferenceNumber(
            IGrouping<OversShortEX, OverShortDetailsEX> os)
        {
            IEnumerable<AsycudaDocumentItem> alst = null;

            using (var ctx = new AsycudaDocumentItemService())
            {
                alst = await ctx.GetAsycudaDocumentItemsByExpressionNav("All", new Dictionary<string, string>()
                {
                    {
                        "AsycudaDocument",
                        string.Format("ReferenceNumber == \"{0}\" || CNumber == \"{1}\"",
                            os.Key.OverShortSuggestedDocuments.ReferenceNumber, os.Key.OverShortSuggestedDocuments.CNumber)
                    }
                }, new List<string>()
                {
                    "AsycudaDocument"
                }).ConfigureAwait(false);

            }
            return alst;
        }

        private async Task SetMatch(IGrouping<OversShort, OverShortDetailsEX> os, IEnumerable<AsycudaDocumentItem> alst)
        {
            foreach (var osd in os)
            {
                osd.Status = null;
                var ai = alst.Where(x => x.ItemNumber == osd.ItemNumber).ToList();
                await DoMatch(ai, osd).ConfigureAwait(false);
            }
        }

        internal async Task RemoveSelectedOverShorts(IEnumerable<OversShortEX> lst)
        {
         
            StatusModel.StartStatusUpdate("Removing OversShort", lst.Count());
            using (var ctx = new OversShortService())
            {
                foreach (var item in lst.ToList())
                {
                    await ctx.DeleteOversShort(item.OversShortsId.ToString()).ConfigureAwait(false);
                    StatusModel.StatusUpdate();
                }
            }
            StatusModel.StopStatusUpdate();

          

        }



        internal async Task AutoMatch(IEnumerable<OversShortEX> slst)
        {
            IEnumerable<IGrouping<OversShortEX, OverShortDetailsEX>> lst;
            
            if (!slst.Any())
            {
                using (var ctx = new OverShortDetailsEXService())
                {
                    var alst = await ctx.GetOverShortDetailsByExpressionNav("ReceivedQty < InvoiceQty",
                        new Dictionary<string, string>()
                        {
                            {
                                "OversShortEX",
                                "OverShortSuggestedDocuments.ReferenceNumber != null || OverShortSuggestedDocuments.CNumber != null"
                            }
                        },
                        new List<string>() {"OversShort"}).ConfigureAwait(false);
                    lst = alst.GroupBy(x => x.OversShortEX);
                }
            }
            else
            {
                lst =
                    slst.SelectMany(x => x.OverShortDetailsEXes)
                        .Where(
                            x =>
                                x.ReceivedQty < x.InvoiceQty &&
                                string.IsNullOrEmpty(x.OversShortEX.OverShortSuggestedDocuments.ReferenceNumber) &&
                                string.IsNullOrEmpty(x.OversShortEX.OverShortSuggestedDocuments.CNumber))
                        .GroupBy(x => x.OversShortEX);
            }

            await AutoMatchAscyudaEntriesToShorts(lst).ConfigureAwait(false);
        }

        internal async Task MatchToCurrentItem(AsycudaDocumentItem currentDocumentItem, OverShortDetailsEX osd)
        {

            var ci = await GetCurrentOSAsycudaDocumentItem(currentDocumentItem).ConfigureAwait(false);
            osd.Status = "";
            await DoMatch(new List<AsycudaDocumentItem>() { { ci } }, osd).ConfigureAwait(false);

          
        }

        private async Task<AsycudaDocumentItem> GetCurrentOSAsycudaDocumentItem(AsycudaDocumentItem currentDocumentItem)
        {
            using (var ctx = new AsycudaDocumentItemService())
            {
                return
                    await
                        ctx.GetAsycudaDocumentItemByKey(currentDocumentItem.Item_Id
                                .ToString()).ConfigureAwait(false);
            }
        }


        internal async Task RemoveOverShortDetail(OverShortDetail osd)
        {
          await DeleteOverShortDetail(osd).ConfigureAwait(false);
        }


        private async Task DeleteOverShortDetail(OverShortDetail osd)
        {
            using (var ctx = new OverShortDetailService())
            {
                await ctx.DeleteOverShortDetail(osd.OverShortDetailId.ToString()).ConfigureAwait(false);
            }
        }


        internal async Task RemoveOsa(OverShortDetailAllocation osa)
        {
            await DeleteOverShortAllocation(osa).ConfigureAwait(false);

            
        }

        private async Task DeleteOverShortAllocation(OverShortDetailAllocation osa)
        {
            using (var ctx = new OverShortDetailAllocationService())
            {
                await ctx.DeleteOverShortDetailAllocation(osa.OverShortAllocationId.ToString()).ConfigureAwait(false);
            }
        }

        private async Task AutoMatchAscyudaEntriesToShorts(IEnumerable<IGrouping<OversShortEX, OverShortDetailsEX>> ilst)
        {
            try
            {

                var lst = ilst.ToList();
                StatusModel.StartStatusUpdate("Matching O/S To Asycuda Entries", lst.Count());
                foreach (var osg in lst)
                {
                    StatusModel.StatusUpdate("Matching O/S To Asycuda Entries");
                    IGrouping<OversShortEX, OverShortDetailsEX> os = osg;

                    await DetailsSetMatch(os).ConfigureAwait(false);
                }

                StatusModel.StopStatusUpdate();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task DetailsSetMatch(IGrouping<OversShortEX, OverShortDetailsEX> os)
        {
            foreach (var osd in os)
            {
                osd.Status = null;
                var alst = await GetSuggestedItems(osd).ConfigureAwait(false);
                await DoMatch(alst.ToList(), osd).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<AsycudaDocumentItem>> GetSuggestedItems(OverShortDetailsEX osd)
        {
            using (var ctx = new AsycudaDocumentItemService())
            {
                var lst = await ctx.GetAsycudaDocumentItemsByExpression(string.Format("ItemNumber == \"{0}\" &&" +
                                                                                      "DocumentType == \"IM7\"",
                    osd.ItemNumber), new List<string>()
                    {
                        "AsycudaDocument"
                    })
                    .ConfigureAwait(false);
                return
                    lst.OrderBy(x => x.AsycudaDocument.RegistrationDate - osd.OversShortEX.InvoiceDate);
            }
        }

    


        

        public async Task CreateOversOps(IEnumerable<OversShortEX> selOS, AsycudaDocumentSet docSet)
        {

            if (docSet == null)
            {
                throw new ApplicationException("Please Select a Asycuda Document Set before proceding");
             
            }

           
            if (selOS.Any() == false)
            {
                throw new ApplicationException("Please Select Overs/Shorts before proceding");
              
            }

            StatusModel.Timer("Getting Data...");

            var slstSource = selOS.SelectMany(x => x.OverShortDetailsEXes.Where(y => y.Type == "Over")).ToList();

            if (!slstSource.Any())
            {
                StatusModel.StopStatusUpdate();
                return;
            }
            var slst = (from s in slstSource
                group s by new
                {
                    s.ItemNumber,
                    s.ItemDescription,
                    s.InventoryItem.TariffCode,
                    s.Cost,
                    s.InventoryItem
                }
                into g
                select new
                {
                    EntlnData = new BaseDataModel.EntryLineData()
                    {
                        ItemNumber = g.Key.ItemNumber,
                        ItemDescription = g.Key.ItemDescription,
                        TariffCode = g.Key.TariffCode,
                        Cost = Convert.ToDouble(g.Key.Cost),
                        InventoryItem = g.Key.InventoryItem,
                        Quantity = Convert.ToDouble(g.Sum(x => x.OversQuantity)),
                        EntryDataDetails = new List<IEntryDataDetail>()
                    }

                }).ToList();

            if (!slst.Any())
            {
                StatusModel.StopStatusUpdate();
                throw new ApplicationException(
                    "No OPS Allocations found! If you just deleted Entries, Please Allocate Sales then continue Else Contact your Network Administrator");
            }


            DocumentCT cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);



            int itmcount = 0;

            Document_Type dt = BaseDataModel.Instance.Document_Types.FirstOrDefault(x => x.DisplayName == "IM7");

            if (dt == null)
            {
                StatusModel.StopStatusUpdate();
                throw new ApplicationException(string.Format("Null Document Type for '{0}' Contact your Network Administrator", "IM7"));
               
            }

            dt.DefaultCustoms_Procedure =
                BaseDataModel.Instance.Customs_Procedures
                    .FirstOrDefault(x => x.DisplayName.Contains("OPP") && x.Document_TypeId == dt.Document_TypeId);

            if (dt.DefaultCustoms_Procedure == null)
            {
                StatusModel.StopStatusUpdate();
                throw new ApplicationException(string.Format("Null Customs Procedure for '{0}' Contact your Network Administrator",
                    "OPP"));
               
            }
           
            CreateOPSClass.Instance.OPSIntializeCdoc(cdoc, dt, docSet);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = dt.DefaultCustoms_Procedure;


            StatusModel.StartStatusUpdate("Creating Opening Stock Entries", slst.Count());


            foreach (var pod in slst)
            {

                StatusModel.StatusUpdate();

                xcuda_Item itm = BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod.EntlnData, cdoc);
                if (itm != null)
                {
                    itmcount += 1;
                }

                if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);

                    cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                    CreateOPSClass.Instance.OPSIntializeCdoc(cdoc, dt, docSet);
                    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = dt.DefaultCustoms_Procedure;
                }

            }
            if (cdoc.DocumentItems.Count == 0) await BaseDataModel.Instance.DeleteAsycudaDocument(cdoc.Document).ConfigureAwait(false);

        }


        public async Task CreateShortsEx9(IEnumerable<OversShortEX> selos, AsycudaDocumentSet docSet, bool BreakOnMonthYear, bool ApplyEX9Bucket)
        {

            if (docSet == null)
            {
                throw new ApplicationException("Please Select a Asycuda Document Set before proceding");
               
            }

           

            if (selos.Any() == false)
            {
                throw new ApplicationException("Please Select Overs/Shorts before proceding");
               
            }

            StatusModel.Timer("Getting Data...");

            var slstSource =
                selos.SelectMany(x => x.OverShortDetailsEXes.Where(y => y.Type == "Short")).ToList();
            if (!slstSource.Any())
            {
                return;
            }
            IEnumerable<EXWDataBlock> slst;
            if (BreakOnMonthYear)
            {
                slst = (from s in slstSource
                    group s by new {MonthYear = s.OversShortEX.InvoiceDate.ToString("MMM-yy")}
                    into g
                    select new EXWDataBlock()
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = "DutyFree",
                        OverShortDetailsEX = g.ToList(),

                    }).ToList();
            }
            else
            {
                slst = (from s in slstSource
                    group s by new {MonthYear = "NoMTY"}
                    into g
                    select new EXWDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = "DutyFree",
                        OverShortDetailsEX = g.ToList(),

                    }).ToList();

            }

            if (!slst.Any())
            {
                StatusModel.StopStatusUpdate();
                throw new ApplicationException(
                    "No OPS Allocations found! If you just deleted Entries, Please Allocate Sales then continue Else Contact your Network Administrator");
                
            }

            var dfp = "Duty Free";


            Document_Type dt = CreateDocumentType();
            if (dt == null) return;

            DocumentCT cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
            CreateEx9Class.Instance.Ex9InitializeCdoc(dt, dfp, cdoc,docSet);

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = dt.DefaultCustoms_Procedure;

            int itmcount = 0;

            StatusModel.StartStatusUpdate("Creating Shorts EXW Entries", slst.Count());

            foreach (EXWDataBlock monthyear in slst)
            {
                var elst = PrepareShortsEXWData(monthyear);

                foreach (var pod in elst)
                {

                    StatusModel.StatusUpdate();

                    if (CreateEx9Class.Instance.CreateEx9EntryAsync(pod, cdoc, itmcount, dfp, ApplyEX9Bucket))
                    {

                        StatusModel.StatusUpdate();

                        itmcount += 1;
                    }

                    if (itmcount%BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)

                        await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                        //dup new file
                        cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);

                        CreateEx9Class.Instance.Ex9InitializeCdoc(dt, dfp, cdoc, docSet);
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = dt.DefaultCustoms_Procedure;
                        if (cdoc.Document != null)
                        {
                            cdoc.Document.xcuda_Valuation = new xcuda_Valuation();
                            cdoc.Document.xcuda_Valuation.xcuda_Weight = new xcuda_Weight();
                            cdoc.Document.xcuda_Valuation.xcuda_Weight.Gross_weight =
                                cdoc.Document.xcuda_PreviousItem.Sum(x => x.Net_weight);
                        }
                    }

                }
            if (cdoc.DocumentItems.Count == 0) await BaseDataModel.Instance.DeleteAsycudaDocument(cdoc.Document).ConfigureAwait(false);




        }

        public StringBuilder BuildOSLst(List<OversShortEX> lst)
        {
            var slst = new StringBuilder("&& (OverShortsId in (");
            foreach (var os in lst)
            {
                slst.Append(string.Format("{0},", os.OversShortsId));
            }
            RemoveComma(slst);
            slst.Append(")");
            return slst;
        }


        private void RemoveComma(StringBuilder slst)
        {
            slst.Remove(slst.Length - 1, 1);
        }

        private Document_Type CreateDocumentType()
        {
            Document_Type dt = BaseDataModel.Instance.Document_Types.FirstOrDefault(x => x.DisplayName == "EX9");

            if (dt == null)
            {
                throw new ApplicationException(string.Format("Null Document Type for '{0}' Contact your Network Administrator", "EX9"));
              
            }

            dt.DefaultCustoms_Procedure =
                BaseDataModel.Instance.Customs_Procedures
                    .FirstOrDefault(x => x.DisplayName.Contains("EXW") && x.Document_TypeId == dt.Document_TypeId);

            if (dt.DefaultCustoms_Procedure == null)
            {
                throw new ApplicationException(string.Format("Null Customs Procedure for '{0}' Contact your Network Administrator",
                    "EXW"));
               
            }
         
            return dt;
        }

        private List<AllocationsModel.MyPodData> PrepareShortsEXWData(EXWDataBlock monthyear)
        {
            var slst = from s in monthyear.OverShortDetailsEX.SelectMany(x => x.OverShortAllocationsEXes)
                group s by new
                {
                    s.OverShortDetailsEX.ItemNumber,
                    s.OverShortDetailsEX.ItemDescription,
                    s.OverShortDetailsEX.InventoryItem.TariffCode,
                    s.OverShortDetailsEX.Cost,
                    s.OverShortDetailsEX.InventoryItem,
                    s.AsycudaDocumentItem
                }
                into g
                select new AllocationsModel.MyPodData
                {
                    Allocations = new List<AsycudaSalesAllocations>(),
                    EntlnData = new AllocationsModel.AlloEntryLineData()
                    {
                        ItemNumber = g.Key.ItemNumber,
                        ItemDescription = g.Key.ItemDescription,
                        TariffCode = g.Key.TariffCode,
                        Cost = Convert.ToDouble(g.Key.Cost),
                        InventoryItem = g.Key.InventoryItem,
                        Quantity = Convert.ToDouble(g.Sum(x => x.QtyAllocated)),
                        EntryDataDetails = new List<IEntryDataDetail>(),
                        //PreviousEntry = g.Key.AsycudaDocumentItem
                    }

                };
            return slst.ToList();
        }

 


    }

    public class EXWDataBlock
    {
        public string MonthYear { get; set; }
        public string DutyFreePaid { get; set; }
        public List<OverShortDetailsEX> OverShortDetailsEX { get; set; }
    }



}

