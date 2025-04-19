using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Entities;
using SimpleMvvmToolkit;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;

//using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;
//using xcuda_PreviousItem = AllocationDS.Business.Entities.xcuda_PreviousItem;

namespace WaterNut.DataSpace
{
    public class CreateEx9Class
    {
        private static readonly CreateEx9Class _instance;
        static CreateEx9Class()
        {
            _instance = new CreateEx9Class();
        }

        public static CreateEx9Class Instance
        {
            get { return _instance; }
        }

        public bool BreakOnMonthYear { get; set; }
        

        public bool PerIM7 { get; set; }
       

        public bool ApplyEx9Bucket { get; set; }
       

        public async Task CreateEx9(string filterExpression, bool perIM7, bool applyEx9Bucket, bool breakOnMonthYear, AsycudaDocumentSet docSet)
        {

            try
            {
                PerIM7 = perIM7;
                ApplyEx9Bucket = applyEx9Bucket;
                BreakOnMonthYear = breakOnMonthYear;
                
                var slst = (await CreateAllocationDataBlocks(filterExpression).ConfigureAwait(false)).Where(x => x.Allocations.Count > 0);

                var dutylst = CreateDutyList(slst);
                foreach (var dfp in dutylst)
                {
                  await CreateDutyFreePaidDocument(dfp, slst.Where(x => x.DutyFreePaid == dfp), docSet).ConfigureAwait(false);
                }
                
                StatusModel.StopStatusUpdate();
               
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task CreateDutyFreePaidDocument(string dfp, IEnumerable<AllocationsModel.AllocationDataBlock> slst, AsycudaDocumentSet docSet)
        {
            try
            {


                var itmcount = 0;


                Document_Type dt;
                dt = await GetDocumentType(dfp).ConfigureAwait(false);
                if (dt == null)
                {
                    throw new ApplicationException(string.Format("Null Document Type for '{0}' Contact your Network Administrator", dfp));
                }


                StatusModel.StatusUpdate(string.Format("Creating xBond Entries - {0}", dfp));

                var cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);

                foreach (var monthyear in slst) //.Where(x => x.DutyFreePaid == dfp)
                {

                    var prevEntryId = "";
                    var prevIM7 = "";
                    var elst = PrepareAllocationsData(monthyear);

                    foreach (var mypod in elst)
                    {
                        //itmcount = await InitializeDocumentCT(itmcount, prevEntryId, mypod, cdoc, prevIM7, monthyear, dt, dfp).ConfigureAwait(true);

                        if (MaxLineCount(itmcount)
                            || InvoicePerEntry(prevEntryId, mypod)
                            || (cdoc.Document == null || itmcount == 0)
                            || IsPerIM7(prevIM7, monthyear))
                        {
                            if (itmcount != 0)
                            {
                                if (cdoc.Document != null)
                                {
                                    await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                    //}
                                    //else
                                    //{
                                    cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                                }
                            }
                            Ex9InitializeCdoc(dt, dfp, cdoc, docSet);
                            if (PerIM7)
                                cdoc.Document.xcuda_Declarant.Number =
                                    cdoc.Document.xcuda_Declarant.Number.Replace(
                                        docSet.Declarant_Reference_Number,
                                        docSet.Declarant_Reference_Number +
                                        "-" +
                                        monthyear.CNumber);
                            InsertEntryIdintoRefNum(cdoc, mypod.EntlnData.EntryDataDetails[0].EntryDataId);

                            itmcount = 0;
                        }

                        if (CreateEx9EntryAsync(mypod, cdoc, itmcount, dfp, this.ApplyEx9Bucket))
                        {
                            itmcount += 1;
                        }
                        
                        prevEntryId = mypod.EntlnData.EntryDataDetails.Count > 0
                            ? mypod.EntlnData.EntryDataDetails[0].EntryDataId
                            : "";
                        prevIM7 = PerIM7 == true ? monthyear.CNumber : "";
                        StatusModel.StatusUpdate();
                    }

                }
                await SaveDocumentCT(cdoc).ConfigureAwait(false);
                // await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }



        private async Task<int> InitializeDocumentCT(int itmcount, string prevEntryId, AllocationsModel.MyPodData mypod,
            DocumentCT cdoc, AsycudaDocumentSet docSet, string prevIM7, AllocationsModel.AllocationDataBlock monthyear, Document_Type dt,
            string dfp)
        {
            try
            {
                if (MaxLineCount(itmcount) 
                    || InvoicePerEntry(prevEntryId, mypod) 
                    || (cdoc.Document == null || itmcount == 0 ) 
                    || IsPerIM7(prevIM7, monthyear))
                {
                    if (itmcount != 0)
                    {
                        if (cdoc.Document != null)
                        {
                            await SaveDocumentCT(cdoc).ConfigureAwait(false);
                        //}
                        //else
                        //{
                            cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(true);
                        }
                    }
                    Ex9InitializeCdoc(dt, dfp, cdoc, docSet);
                    if (PerIM7 == true)
                        cdoc.Document.xcuda_Declarant.Number =
                            cdoc.Document.xcuda_Declarant.Number.Replace(
                                docSet.Declarant_Reference_Number,
                                docSet.Declarant_Reference_Number + "-" +
                                monthyear.CNumber);
                    InsertEntryIdintoRefNum(cdoc, mypod.EntlnData.EntryDataDetails[0].EntryDataId);

                    itmcount = 0;
                }
                return itmcount;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsPerIM7(string prevIM7, AllocationsModel.AllocationDataBlock monthyear)
        {
            return (PerIM7 == true && (string.IsNullOrEmpty(prevIM7) ||(!string.IsNullOrEmpty(prevIM7) && prevIM7 != monthyear.CNumber)));
        }

        private static bool InvoicePerEntry(string prevEntryId, AllocationsModel.MyPodData mypod)
        {
            return (BaseDataModel.Instance.CurrentApplicationSettings
                .InvoicePerEntry == true &&
                    //prevEntryId != "" &&
                    prevEntryId != mypod.EntlnData.EntryDataDetails[0].EntryDataId);
        }

        private static bool MaxLineCount(int itmcount)
        {
            return (itmcount != 0 &&
                    itmcount%
                    BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines ==
                    0);
        }

        private static async Task SaveDocumentCT(DocumentCT cdoc)
        {
            try
            {

                if (cdoc != null && cdoc.DocumentItems.Any() == true)
                {
                    if(cdoc.Document.xcuda_Valuation == null)
                    cdoc.Document.xcuda_Valuation = new xcuda_Valuation() {ASYCUDA_Id = cdoc.Document.ASYCUDA_Id, TrackingState = TrackingState.Added };
                    if (cdoc.Document.xcuda_Valuation.xcuda_Weight == null)
                        cdoc.Document.xcuda_Valuation.xcuda_Weight = new xcuda_Weight() { Valuation_Id = cdoc.Document.xcuda_Valuation.ASYCUDA_Id, TrackingState = TrackingState.Added };
                    cdoc.Document.xcuda_Valuation.xcuda_Weight.Gross_weight =
                        cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Sum(x => x.Net_weight);

                    

                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static async Task<Document_Type> GetDocumentType(string dfp)
        {
            try
            {

                Document_Type dt;
                using (var ctx = new Document_TypeService())
                {
                    if (dfp == "Duty Free")
                    {
                        dt =
                            (await
                                ctx.GetDocument_TypeByExpression(
                                    "Type_of_declaration + Declaration_gen_procedure_code == \"EX9\"")
                                    .ConfigureAwait(false)).FirstOrDefault();
                    }
                    else
                    {
                        dt =
                            (await
                                ctx.GetDocument_TypeByExpression(
                                    "Type_of_declaration + Declaration_gen_procedure_code == \"IM4\"")
                                    .ConfigureAwait(false)).FirstOrDefault();
                    }
                }
                return dt;

            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        private static IEnumerable<string> CreateDutyList(IEnumerable<AllocationsModel.AllocationDataBlock> slst)
        {
            try
            {
                var dutylst = slst.Where(x => x.Allocations.Count > 0).Select(x => x.DutyFreePaid).Distinct();
                return dutylst;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<IEnumerable<AllocationsModel.AllocationDataBlock>> CreateAllocationDataBlocks(string filterExpression)
        {
            try
            {
                StatusModel.Timer("Getting ExBond Data");
                var slstSource = await GetEX9Data(filterExpression).ConfigureAwait(false);
                StatusModel.StartStatusUpdate("Creating xBond Entries", slstSource.Count());
                IEnumerable<AllocationsModel.AllocationDataBlock> slst;
                if (BreakOnMonthYear == true)
                {
                    slst = CreateBreakOnMonthYearAllocationDataBlocks(slstSource);
                }
                else
                {
                    slst = CreateWholeAllocationDataBlocks(slstSource);
                }
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static async Task<List<AsycudaSalesAllocations>> GetEX9Data(string FilterExpression)
        {
            //res.Append("PreviousItem_Id.HasValue == true" +
            //       "&& (xBond_Item_Id == 0)" +
            //       "&& (pIsAssessed == true)" +
            //       "&& (QtyAllocated != null && EntryDataDetailsId != null && Cost > 0)" +
            //       "&& (pRegistrationDate != DateTime.MinValue)" +
            //       "&& (pCNumber != null)");
            var lst = await AllocationsModel.Instance.GetAsycudaSalesAllocations(FilterExpression).ConfigureAwait(false);
            return lst.Where(x => x.PreviousDocumentItem != null 
                                    && x.PreviousDocumentItem.AsycudaDocument.CNumber != null
                                    && x.xBondEntry == null
                                    && x.PreviousDocumentItem.IsAssessed == true 
                                    && (x.QtyAllocated != 0 && x.EntryDataDetails != null && x.EntryDataDetails.Cost > 0)
                                    && x.PreviousDocumentItem.ItemQuantity > 0).ToList();
        }

        private IEnumerable<AllocationsModel.AllocationDataBlock> CreateWholeAllocationDataBlocks(List<AsycudaSalesAllocations> slstSource)
        {
            IEnumerable<AllocationsModel.AllocationDataBlock> slst;
            if (PerIM7 == true)
            {
                slst = CreateWholeIM7AllocationDataBlocks(slstSource);
            }
            else
            {
                slst = CreateWholeNonIM7AllocationDataBlocks(slstSource);
            }
            return slst;
        }

        private static IEnumerable<AllocationsModel.AllocationDataBlock> CreateWholeNonIM7AllocationDataBlocks(
            List<AsycudaSalesAllocations> slstSource)
        {
            try
            {

                IEnumerable<AllocationsModel.AllocationDataBlock> slst;
                var source = slstSource.Where(x => x.EntryDataDetails.Sales != null).ToList();

                slst = from s in source
                    group s by new {s.EntryDataDetails.Sales.DutyFreePaid, MonthYear = "NoMTY"}
                    into g
                    select new AllocationsModel.AllocationDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static IEnumerable<AllocationsModel.AllocationDataBlock> CreateWholeIM7AllocationDataBlocks(
            List<AsycudaSalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationsModel.AllocationDataBlock> slst;
                slst = from s in slstSource
                    group s by
                        new
                        {
                            s.EntryDataDetails.Sales.DutyFreePaid,
                            MonthYear = "NoMTY",
                            s.PreviousDocumentItem.AsycudaDocument.CNumber
                        }
                    into g
                    select new AllocationsModel.AllocationDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                        CNumber = g.Key.CNumber
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationsModel.AllocationDataBlock> CreateBreakOnMonthYearAllocationDataBlocks(
            List<AsycudaSalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationsModel.AllocationDataBlock> slst;
                if (PerIM7 == true)
                {
                    slst = CreatePerIM7AllocationDataBlocks(slstSource);
                }
                else
                {
                    slst = CreateAllocationDataBlocks(slstSource);
                }
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static IEnumerable<AllocationsModel.AllocationDataBlock> CreateAllocationDataBlocks(
            List<AsycudaSalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationsModel.AllocationDataBlock> slst;
                slst = from s in slstSource
                    group s by
                        new
                        {
                            s.EntryDataDetails.Sales.DutyFreePaid,
                            MonthYear =
                                Convert.ToDateTime(
                                    s.EntryDataDetails.Sales.EntryDataDate)
                                    .ToString("MMM-yy")
                        }
                    into g
                    select new AllocationsModel.AllocationDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static IEnumerable<AllocationsModel.AllocationDataBlock> CreatePerIM7AllocationDataBlocks(List<AsycudaSalesAllocations> slstSource)
        {
            try
            {

            IEnumerable<AllocationsModel.AllocationDataBlock> slst;
            slst = from s in slstSource
                group s by
                    new
                    {
                        s.EntryDataDetails.Sales.DutyFreePaid,
                        MonthYear =
                            Convert.ToDateTime(
                                s.EntryDataDetails.Sales.EntryDataDate)
                                .ToString("MMM-yy"),
                        s.PreviousDocumentItem.AsycudaDocument.CNumber
                    }
                into g
                select new AllocationsModel.AllocationDataBlock
                {
                    MonthYear = g.Key.MonthYear,
                    DutyFreePaid = g.Key.DutyFreePaid,
                    Allocations = g.ToList(),
                    CNumber = g.Key.CNumber
                };
            return slst;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<AllocationsModel.MyPodData> PrepareAllocationsData(AllocationsModel.AllocationDataBlock monthyear)
        {
            try
            {
                List<AllocationsModel.MyPodData> elst;
                if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true)
                {
                    elst = GroupAllocationsByEx9(monthyear);
                }
                else
                {
                    elst = GroupAllocations(monthyear);
                }

                return elst.Where(x => (x.EntlnData.PreviousDocumentItem as xcuda_Item).DoNotEX != true).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static List<AllocationsModel.MyPodData> GroupAllocations(AllocationsModel.AllocationDataBlock monthyear)
        {
            try
            {


                List<AllocationsModel.MyPodData> elst;
                elst =
                    (from s in
                        Enumerable.OrderBy<AsycudaSalesAllocations, string>(monthyear.Allocations, p => p.TariffCode)
                            .GroupBy(x => x.PreviousDocumentItem)
                            .Where(z => z.ToList().Any(q => q.EntryDataDetails.Quantity < 0) == false)
                            .SelectMany(x => x.ToList())
                        select new AllocationsModel.MyPodData
                        {
                            Allocations = new List<AsycudaSalesAllocations>() {s},
                            EntlnData = new AllocationsModel.AlloEntryLineData
                            {
                                ItemNumber = s.EntryDataDetails.ItemNumber,
                                InventoryItem = s.EntryDataDetails.InventoryItem,
                                ItemDescription = s.EntryDataDetails.ItemDescription,
                                TariffCode = s.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.Commodity_code,
                                Cost = Convert.ToDouble(s.PreviousDocumentItem.ItemCost),
                                Quantity = s.QtyAllocated,
                                EntryDataDetails = new List<IEntryDataDetail>() {s.EntryDataDetails},
                                PreviousDocumentItemId = s.PreviousDocumentItem.Item_Id.ToString()
                            }
                        }).ToList();
                // group the returns
                var returnlst =
                    (from s in
                        Enumerable.OrderBy<AsycudaSalesAllocations, string>(monthyear.Allocations, p => p.TariffCode)
                            .GroupBy(x => x.PreviousDocumentItem)
                            .Where(z => z.ToList().Where(q => q.EntryDataDetails.Quantity < 0).Any() == true)
                        select new AllocationsModel.MyPodData
                        {
                            Allocations = s.ToList(),
                            EntlnData = new AllocationsModel.AlloEntryLineData
                            {
                                ItemNumber = s.LastOrDefault().EntryDataDetails.ItemNumber,
                                ItemDescription = s.LastOrDefault().EntryDataDetails.ItemDescription,
                                TariffCode = s.Key.TariffCode,
                                Cost = s.Key.ItemCost,
                                Quantity = s.Sum(x => x.QtyAllocated),
                                EntryDataDetails = s.Select(x => x.EntryDataDetails as IEntryDataDetail).ToList(),
                                PreviousDocumentItemId = s.Key.Item_Id.ToString()
                            }
                        }).ToList();
                elst.AddRange(returnlst);
                return elst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static List<AllocationsModel.MyPodData> GroupAllocationsByEx9(
            AllocationsModel.AllocationDataBlock monthyear)
        {
            try
            {


                List<AllocationsModel.MyPodData> elst;
                elst =
                    (from s in
                        Enumerable.OrderBy<AsycudaSalesAllocations, string>(monthyear.Allocations, p => p.TariffCode)
                                .Where(x => x.PreviousDocumentItem.ItemCost != null)
                        //  where s.EntryDataDetails.ItemNumber == "SPG20331"
                        group s by new
                        {
                            // s.EntryDataDetails.ItemNumber,
                            // s.EntryDataDetails.ItemDescription,
                            // s.EntryDataDetails.TariffCode,
                            //Cost = Math.Round(s.EntryDataDetails.Cost, 2),
                            // s.EntryDataDetails.InventoryItems,
                            s.PreviousDocumentItem
                        }
                        into g
                        select new AllocationsModel.MyPodData
                        {
                            Allocations = g.ToList(),
                            EntlnData = new AllocationsModel.AlloEntryLineData
                            {
                                //ItemNumber = g.Key.ItemNumber,
                                ItemNumber = g.LastOrDefault().EntryDataDetails.ItemNumber,
                                InventoryItem = g.LastOrDefault().EntryDataDetails.InventoryItem,
                                //ItemDescription = g.Key.ItemDescription,
                                ItemDescription = g.LastOrDefault().EntryDataDetails.ItemDescription,
                                //TariffCode = g.Key.TariffCode,
                                TariffCode = g.Key.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.Commodity_code,
                                Cost = Convert.ToDouble(g.Key.PreviousDocumentItem.ItemCost),
                                // InventoryItem = g.Key.InventoryItems,
                                Quantity = g.Sum(x => x.QtyAllocated),
                                EntryDataDetails = g.Select(x => x.EntryDataDetails as IEntryDataDetail).ToList(),
                                PreviousDocumentItemId = g.Key.PreviousDocumentItem.Item_Id.ToString(),
                                PreviousDocumentItem = g.Key.PreviousDocumentItem
                            }
                        }).ToList();
                return elst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InsertEntryIdintoRefNum(DocumentCT cdoc, string entryDataId)
        {
            try
            {
                if (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry == true)
                {
                    cdoc.Document.xcuda_Declarant.Number = entryDataId;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool CreateEx9EntryAsync(dynamic mypod, DocumentCT cdoc, int itmcount, string dfp,bool applyEX9Bucket)
        {
            try
            {

                if (applyEX9Bucket == true)
                {
                    Ex9Bucket(mypod, dfp, cdoc);
                }



                mypod.EntlnData.Quantity = Math.Round(mypod.EntlnData.Quantity, 2);
                if (mypod.EntlnData.Quantity <= 0) return false;

                global::DocumentItemDS.Business.Entities.xcuda_PreviousItem pitm = CreatePreviousItem(mypod.EntlnData, itmcount, dfp);
                if (pitm.Net_weight < 0.01)
                {
                    foreach (AsycudaSalesAllocations item in mypod.Allocations)
                    {
                        item.Status = "Net Weight < 0.01";
                    }
                    return false;
                }

               
                //cdoc.Document.xcuda_PreviousItem.Add(pitm);
                pitm.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                global::DocumentItemDS.Business.Entities.xcuda_Item itm = BaseDataModel.Instance.CreateItemFromEntryDataDetail(mypod.EntlnData, cdoc);
                
                itm.xcuda_ASYCUDA = cdoc.Document;

                //TODO:Refactor this dup code
                if (mypod.Allocations != null)
                {
                    var itmcnt = 0;
                    foreach (
                        var allo in (mypod.Allocations as List<AsycudaSalesAllocations>))//.Distinct()
                    {
                        itm.xBondAllocations.Add(new xBondAllocations(){ AllocationId = allo.AllocationId, xcuda_Item = itm, TrackingState = TrackingState.Added});

                        itmcnt = AddFreeText(itmcnt, itm, allo.EntryDataDetails.EntryDataId);
                    }
                }
                //return true;



                itm.xcuda_PreviousItem = pitm;
                pitm.xcuda_Item = itm;

               // cdoc.Document.xcuda_PreviousItem.Add(pitm);
                //pitm.xcuda_ASYCUDA = cdoc;
                // pitm.PreviousDocumentItem = itm;

                itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pitm.Hs_code;
                itm.xcuda_Goods_description.Country_of_origin_code = pitm.Goods_origin;


                itm.xcuda_Previous_doc.Summary_declaration = String.Format("{0} {1} C {2} art. {3}", pitm.Prev_reg_cuo,
                    pitm.Prev_reg_dat,
                    pitm.Prev_reg_nbr, pitm.Previous_item_number);
                itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm() { TrackingState = TrackingState.Added };
                itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = pitm.Net_weight;
                itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = pitm.Net_weight;
                // adjusting because not using real statistical value when calculating
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency =
                    Convert.ToDouble(Math.Round((pitm.Current_value * pitm.Suplementary_Quantity), 2));
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency =
                    Convert.ToDouble(Math.Round(pitm.Current_value * pitm.Suplementary_Quantity, 2));
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_code = "XCD";
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_rate = 1;


                if (cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Count() == 1 || itmcount == 0)
                {
                    pitm.Packages_number = "1"; //(i.Packages.Number_of_packages).ToString();
                    pitm.Previous_Packages_number = pitm.Previous_item_number == "1" ? "1" : "0";
                }
                else
                {
                    pitm.Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                    pitm.Previous_Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                }

               

                return true;
            }
            catch (Exception Ex)
            {
                throw;
            }


        }

        private static int AddFreeText(int itmcnt, global::DocumentItemDS.Business.Entities.xcuda_Item itm, string entryDataId)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 != true)
            {
                if (itmcnt < 5)
                {
                    if (itm.Free_text_1 == null) itm.Free_text_1 = ""; //"Inv.#"
                    itm.Free_text_1 = itm.Free_text_1 + "|" +
                                      string.Format("{0}", entryDataId);
                    //CleanText(allo.EntryDataDetails.EntryDataId));
                }
                else
                {
                    if (itm.Free_text_2 == null) itm.Free_text_2 = ""; //"Inv.#"
                    itm.Free_text_2 = itm.Free_text_2 + "|" +
                                      string.Format("{0}", entryDataId);
                    // CleanText(allo.EntryDataDetails.EntryDataId)); 
                }

                itmcnt += 1;
            }
            if (itm.Free_text_1 != null && itm.Free_text_1.Length > 1)
            {
                if (itm.Free_text_1.Length < 31)
                {
                    itm.Free_text_1 = itm.Free_text_1.Substring(1);
                }
                else
                {
                    itm.Free_text_1 = itm.Free_text_1.Substring(1, 30);
                }
            }


            if (itm.Free_text_2 != null && itm.Free_text_2.Length > 1)
            {
                if (itm.Free_text_2.Length < 21)
                {
                    itm.Free_text_2 = itm.Free_text_2.Substring(1);
                }
                else
                {
                    itm.Free_text_2 = itm.Free_text_2.Substring(1, 20);
                }
            }
            return itmcnt;
        }

        private  void Ex9Bucket(AllocationsModel.MyPodData mypod, string dfp, DocumentCT cdoc)
        {
            // prevent over draw down of pqty == quantity allocated
            try
            {
                
                var eld = mypod.EntlnData;
                var previousItem = mypod.EntlnData.PreviousDocumentItem as xcuda_Item;
                if (previousItem == null) return;
                var PdfpAllocated = (dfp == "Duty Free" ? previousItem.DFQtyAllocated : previousItem.DPQtyAllocated);
                var pAllocated = previousItem.DFQtyAllocated + previousItem.DPQtyAllocated;


                if (previousItem.QtyAllocated == 0)
                {
                    if (dfp == "Duty Free")
                    {
                        previousItem.DFQtyAllocated = 0;
                    }
                    else
                    {
                        previousItem.DPQtyAllocated = 0;
                    }
                }
                if (previousItem.xcuda_PreviousItems.Any() == false) return;

                var pqty = previousItem.xcuda_PreviousItems.Where(x => x.xcuda_Item.AsycudaDocument.CNumber != null && x.DutyFreePaid == dfp).Sum(xx => xx.Suplementary_Quantity);
                var apqty = previousItem.xcuda_PreviousItems.Where(x => x.xcuda_Item.AsycudaDocument.CNumber != null).Sum(xx => xx.Suplementary_Quantity);

                if (previousItem.QtyAllocated == 0) return;

                if (pqty != 0 && previousItem.xcuda_PreviousItems.Sum(x => x.Suplementary_Quantity) == previousItem.ItemQuantity) pqty = Convert.ToDouble(previousItem.xcuda_PreviousItems.Sum(x => x.Suplementary_Quantity));


                if (pqty == 0 && (eld.Quantity > (previousItem.ItemQuantity - previousItem.xcuda_PreviousItems.Sum(x => x.Suplementary_Quantity)))) pqty = Convert.ToDouble(previousItem.xcuda_PreviousItems.Sum(x => x.Suplementary_Quantity));


                if (previousItem.ItemQuantity == apqty)
                {
                    mypod.Allocations.Clear();
                    eld.EntryDataDetails.Clear();
                    eld.Quantity = 0;
                    return;
                }
                if (PdfpAllocated - (apqty + Convert.ToDouble(eld.Quantity)) < 0)
                {

                    var qty = PdfpAllocated - (apqty);

                    var lst = eld.EntryDataDetails.OrderBy(x => (x as EntryDataDetails).Sales.EntryDataDate).ToList();
                    var aqty = PdfpAllocated;
                    var lqty = lst.Sum(x => x.QtyAllocated);
                    while (qty < lqty)
                    {
                        if (lst.Any() == false) break;
                        lqty -= lst.ElementAt(0).QtyAllocated;

                        //if (aqty < qty) 
                        //{ 
                        //}
                        var entlst = mypod.Allocations.Where(x => x.EntryDataDetails.EntryDataDetailsId == lst.ElementAt(0).EntryDataDetailsId).ToList();
                        foreach (var item in entlst)
                        {
                            mypod.Allocations.Remove(item);
                        }


                        eld.EntryDataDetails.Remove(lst.ElementAt(0));
                        lst.RemoveAt(0);
                    }


                    eld.Quantity = qty;
                }


            }
            catch (Exception Ex)
            {
                throw;
            }

        }

        private global::DocumentItemDS.Business.Entities.xcuda_PreviousItem CreatePreviousItem(AllocationsModel.AlloEntryLineData pod, int itmcount, string dfp)
        {

            try
            {
                var previousItem = (pod.PreviousDocumentItem as xcuda_Item);
                
                var pitm = new global::DocumentItemDS.Business.Entities.xcuda_PreviousItem(){TrackingState = TrackingState.Added };
                if (previousItem == null) return pitm;

                pitm.Hs_code = previousItem.xcuda_Tarification.xcuda_HScode.Commodity_code;
                pitm.Commodity_code = "00";
                pitm.Current_item_number = (itmcount + 1).ToString(); // piggy back the previous item count
                pitm.Previous_item_number = previousItem.LineNumber.ToString();


                SetWeights(pod, pitm, dfp);


                pitm.Previous_Packages_number = "0";


                pitm.Suplementary_Quantity = Convert.ToDouble(pod.Quantity);
                pitm.Preveious_suplementary_quantity = Convert.ToDouble(previousItem.ItemQuantity);


                pitm.Goods_origin = previousItem.xcuda_Goods_description.Country_of_origin_code;
                double pval = previousItem.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency;
                pitm.Previous_value = Convert.ToDouble((pval / previousItem.ItemQuantity));
                pitm.Current_value = Convert.ToDouble((pval) / Convert.ToDouble(previousItem.ItemQuantity));
                pitm.Prev_reg_ser = "C";
                pitm.Prev_reg_nbr = previousItem.AsycudaDocument.CNumber;
                pitm.Prev_reg_dat = previousItem.AsycudaDocument.RegistrationDate.GetValueOrDefault().Year.ToString();
                pitm.Prev_reg_cuo = previousItem.AsycudaDocument.Customs_clearance_office_code;

                return pitm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetWeights(AllocationsModel.AlloEntryLineData pod, global::DocumentItemDS.Business.Entities.xcuda_PreviousItem pitm, string dfp)
        {
            try
            {
                var previousItem = pod.PreviousDocumentItem as xcuda_Item;
                if (previousItem == null) return;
                var pw = Convert.ToDouble(previousItem.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm);
                //Double iw = System.Convert.ToDouble(Math.Round((pod.PreviousDocumentItem.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm
                //                    / pod.PreviousDocumentItem.ItemQuantity) * Convert.ToDouble(pod.Quantity), 2));
                var iw = Convert.ToDouble((previousItem.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm
                                           / previousItem.ItemQuantity) * Convert.ToDouble(pod.Quantity));

                var ppdfpqty =
                    previousItem.xcuda_PreviousItems.Where(x => x.DutyFreePaid != dfp)
                        .Sum(x => x.Suplementary_Quantity);
                var pi = previousItem.ItemQuantity -
                         previousItem.xcuda_PreviousItems.Where(x => x.DutyFreePaid == dfp)
                             .Sum(x => x.Suplementary_Quantity);
                var pdfpqty = (dfp == "Duty Free"
                    ? previousItem.DPQtyAllocated
                    : previousItem.DFQtyAllocated);
                var rw = previousItem.xcuda_PreviousItems.ToList().Sum(x => x.Net_weight);
                if ((pdfpqty == ppdfpqty) &&
                    (pi == pod.Quantity) &&
                    (previousItem.ItemQuantity - previousItem.QtyAllocated == 0))
                    //                //(pod.PreviousDocumentItem.AsycudaSalesAllocations.Max(x => x.EntryDataDetailsId) == pod.EntryDataDetails.Max(x => x.EntryDataDetailsId) && (pod.PreviousDocumentItem.ItemQuantity - pod.PreviousDocumentItem.QtyAllocated == 0))
                {

                    pitm.Net_weight = Convert.ToDouble(pw - rw);
                }
                else
                {
                    pitm.Net_weight = Convert.ToDouble(Math.Round(iw, 2));
                }
                //pitm.Prev_net_weight = pw;
                pitm.Prev_net_weight = Convert.ToDouble(pw - rw);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Ex9InitializeCdoc(Document_Type dt, string dfp, DocumentCT cdoc, AsycudaDocumentSet ads)
        {
            try
            {

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                BaseDataModel.Instance.IntCdoc(cdoc, dt, ads);

                switch (dfp)
                {
                    case "Duty Free":
                        var Exp = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(y => y.Description.ToUpper() == "EX9".ToUpper());
                        if (Exp.Customs_Procedure == null || string.IsNullOrEmpty(Exp.Customs_Procedure))
                        {
                            throw new ApplicationException("Export Template default Customs Procedures not Configured!");
                        }
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Duty Free Entries";
                        var df  =
                            BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                                .FirstOrDefault(
                                    x =>
                                        x.DisplayName ==
                                        ((Exp == null || string.IsNullOrEmpty(Exp.Customs_Procedure))
                                            ? "9070-000"
                                            : Exp.Customs_Procedure));
                       
                            AttachCustomProcedure(cdoc, df);
                        
                        break;
                    case "Duty Paid":
                        var Exp1 = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(y => y.Description == "IM4");
                        if (Exp1.Customs_Procedure == null || string.IsNullOrEmpty(Exp1.Customs_Procedure))
                        {
                            throw new ApplicationException("Export Template default Customs Procedures not Configured!");
                        }
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Duty Paid Entries";
                        var dp =
                            BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                                .FirstOrDefault(
                                    x =>
                                        x.DisplayName ==
                                        ((Exp1 == null || string.IsNullOrEmpty(Exp1.Customs_Procedure))
                                            ? "4070-000"
                                            : Exp1.Customs_Procedure));
                        AttachCustomProcedure(cdoc, dp);
                        break;
                    default:
                        break;
                }

                AllocationsModel.Instance.AddDutyFreePaidtoRef(cdoc, dfp, ads);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void AttachCustomProcedure(DocumentCT cdoc, Customs_Procedure cp)
        {
            if (cp == null)
            {
                throw new ApplicationException("Default Export Template not configured properly!");
            }
            else
            {
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = cp.Document_TypeId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type = cp.Document_Type;
            }
        }
    }
}