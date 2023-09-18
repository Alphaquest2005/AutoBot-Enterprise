using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.Interfaces;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using InventoryItem = InventoryDS.Business.Entities.InventoryItem;
using ItemSalesPiSummary = WaterNut.DataSpace.ItemSalesPiSummary;
using xBondAllocations = DocumentItemDS.Business.Entities.xBondAllocations;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    public class CreateDutyFreePaidDocument
    {
        
        private static readonly ConcurrentBag<PiSummary> DocSetPi = new ConcurrentBag<PiSummary>();
        private Dictionary<(int PreviousItemId, string CNumber), xcuda_Exporter> _exporters = null;

        static CreateDutyFreePaidDocument()
        {
        }

      

        private static List<InventoryItem> InventoryDataCache { get;  set; }

       

     
        public async Task<List<DocumentCT>> Execute(string dfp,
            IEnumerable<AllocationDataBlock> slst,
            AsycudaDocumentSet docSet, string documentType, bool isGrouped,
            List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths,
            bool applyEx9Bucket, string ex9BucketType, bool applyHistoricChecks, bool applyCurrentChecks,
            bool autoAssess, bool perInvoice, bool overPIcheck, bool universalPIcheck, bool itemPIcheck,
            ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems, bool PerIM7, string prefix = null)
        {
            try
            {
               // ClearDocSetPi(); // move this out for now so it spans mulitple periods

                var sql = "";
                // applyHistoricChecks = false;
                // applyEx9Bucket = false;
                //applyCurrentChecks = false;
                if(InventoryDataCache == null) InventoryDataCache =
                    InventoryItemUtils.GetInventoryItems(DataSpace.BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId, false);

                var itmcount = 0;
                var docList = new List<DocumentCT>();

             

                 if (checkForMultipleMonths)
                    if (slst.ToList().SelectMany(x => x.Allocations).Select(x => x.InvoiceDate.Month).Distinct()
                            .Count() > 1)
                    {
                        throw new ApplicationException(
                            string.Format("Data Contains Multiple Months", dfp));
                    }



                StatusModel.StatusUpdate($"Creating xBond Entries - {dfp}");

                var cdoc = await DataSpace.BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                
                var effectiveAssessmentDate =
                    slst.SelectMany(x =>x.Allocations).Select(x =>
                        x.EffectiveDate == DateTime.MinValue || x.EffectiveDate == null
                            ? x.InvoiceDate
                            : x.EffectiveDate).Min();
                Ex9InitializeCdoc(dfp, cdoc, docSet, documentType, effectiveAssessmentDate, docList, prefix);
               // var docTasks = new List<Task>();
                foreach (var monthyear in slst) //.Where(x => x.DutyFreePaid == dfp)
                {

                    var prevEntryId = "";
                    var prevIM7 = "";
                    var elst = PrepareAllocationsData(monthyear, isGrouped, sql);

                    if (perInvoice) elst = elst.OrderBy(x => x.EntlnData.EntryDataDetails.First().EntryDataId);
                   
                    foreach (var mypod in elst)
                    {
                        //itmcount = await InitializeDocumentCT(itmcount, prevEntryId, mypod, cdoc, prevIM7, monthyear, dt, dfp).ConfigureAwait(true);
                        if (!cdoc.EmailIds.Contains(mypod.EntlnData.EmailId))
                            cdoc.EmailIds.Add(mypod.EntlnData.EmailId);


                        if (!(mypod.EntlnData.Quantity > 0)) continue;
                        if (DataSpace.BaseDataModel.Instance.MaxLineCount(itmcount)
                            || DataSpace.BaseDataModel.Instance.InvoicePerEntry(perInvoice, prevEntryId, mypod.EntlnData.EntryDataDetails[0].EntryDataId)
                            || (cdoc.Document == null || itmcount == 0)
                            || DataSpace.BaseDataModel.Instance.IsPerIM7(PerIM7,prevIM7, monthyear.CNumber))
                        {
                            if (itmcount != 0)
                            {
                                if (cdoc.Document != null)
                                {

                                    DataSpace.BaseDataModel.SaveAttachments(docSet, cdoc);
                                    AttachSupplier(cdoc);
                                   // await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                    //SaveSql(sql);
                                    //sql = "";
                                    //docTasks.Add(Task.Run(async () =>
                                    //{
                                        await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                        SaveSql(sql);
                                    //}));

                                    docList.Add(cdoc);
                                    //}
                                    //else
                                    //{
                                    cdoc = await DataSpace.BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);

                                    effectiveAssessmentDate =
                                        monthyear.Allocations.Select(x =>
                                            x.EffectiveDate == DateTime.MinValue || x.EffectiveDate == null
                                                ? x.InvoiceDate
                                                : x.EffectiveDate).Min();
                                }
                            }

                            Ex9InitializeCdoc(dfp, cdoc, docSet, documentType, effectiveAssessmentDate, docList, prefix);
                            if (PerIM7)
                                cdoc.Document.xcuda_Declarant.Number =
                                    cdoc.Document.xcuda_Declarant.Number.Replace(
                                        docSet.Declarant_Reference_Number,
                                        docSet.Declarant_Reference_Number +
                                        "-" +
                                        monthyear.CNumber);
                            InsertEntryIdintoRefNum(cdoc, mypod.EntlnData.EntryDataDetails.First().EntryDataId);

                            //  if (cdoc.Document.xcuda_General_information == null) cdoc.Document.xcuda_General_information = new xcuda_General_information(true) {TrackingState = TrackingState.Added};
                            cdoc.Document.xcuda_General_information.Comments_free_text =
                                $"EffectiveAssessmentDate:{effectiveAssessmentDate.GetValueOrDefault():MMM-dd-yyyy}";
                            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                                effectiveAssessmentDate;
                            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                            var curLst = mypod.EntlnData.EntryDataDetails.Select(x => x.Currency).Where(x => x != null)
                                .Distinct().ToList();
                            if (curLst.Any())
                            {
                                if (curLst.Count() > 1)
                                    throw new ApplicationException("EntryDataDetails Contains More than 1 Currencies");

                                if (cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code != curLst.First())
                                {
                                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = curLst.First();
                                }
                            }

                            itmcount = 0;
                        }

                        var t = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Customs_ProcedureId;
                        var newItms = await
                            CreateEx9EntryAsync(mypod, cdoc, itmcount, dfp, documentType,
                                    itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks, overPIcheck, universalPIcheck, itemPIcheck, docPreviousItems)
                                .ConfigureAwait(false);

                        itmcount += newItms.itmCount;
                        sql += newItms.sql;

                        prevEntryId = mypod.EntlnData.EntryDataDetails.Count() > 0
                            ? mypod.EntlnData.EntryDataDetails[0].EntryDataId
                            : "";
                        prevIM7 = PerIM7 == true ? monthyear.CNumber : "";
                        StatusModel.StatusUpdate();
                    }

                }
                DataSpace.BaseDataModel.SaveAttachments(docSet, cdoc);
                AttachSupplier(cdoc);
                //await SaveDocumentCT(cdoc).ConfigureAwait(false);
                if (!cdoc.DocumentItems.Any())
                {
                    //clean up
                    docSet.xcuda_ASYCUDA_ExtendedProperties.Remove(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties);
                    cdoc.Document = null;
                }

                //await SaveDocumentCT(cdoc).ConfigureAwait(false);
                //SaveSql(sql);

                //docTasks.Add(Task.Run(async () =>
                //{
                    await SaveDocumentCT(cdoc).ConfigureAwait(false);
                    SaveSql(sql);
                //}));

                if (cdoc.Document != null) docList.Add(cdoc);

               // Task.WaitAll(docTasks.ToArray());

                return docList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void ClearDocSetPi()
        {
            DocSetPi.Clear();
        }

        private static void SaveSql(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            using (var ctx = new AllocationDSContext())
            {
                ctx.Database.CommandTimeout = 0;
                ctx.Database
                    .ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
        }

        private void AttachSupplier(DocumentCT cdoc)
        {
            var firstItm = cdoc.DocumentItems.FirstOrDefault();
            if (firstItm == null) return;
            
            var exporter = Enumerable.FirstOrDefault<KeyValuePair<(int PreviousItemId, string CNumber), xcuda_Exporter>>(GetExporters(), x => x.Key.CNumber == firstItm.xcuda_PreviousItem.Prev_reg_nbr).Value;
            if(exporter != null)
            {
                cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name = exporter.Exporter_name;
            }
            
        }

        private Dictionary<(int PreviousItemId, string CNumber), xcuda_Exporter> GetExporters()
        {
            if(_exporters != null) return _exporters;
            _exporters = new DocumentDSContext().xcuda_Exporter.Select(x => new { PreviousItemId = x.Traders_Id, CNumber = x.xcuda_Traders.xcuda_ASYCUDA.xcuda_Identification.xcuda_Registration.Number, value = x}).ToDictionary(x => (x.PreviousItemId,x.CNumber), x => x.value);
            return _exporters;
        }

        private async Task SaveDocumentCT(DocumentCT cdoc)
        {
            try
            {

                if (cdoc != null && cdoc.DocumentItems.Any() == true)
                {
                    PreProcesEx9Document(cdoc);


                    await DataSpace.BaseDataModel.Instance.SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void PreProcesEx9Document(DocumentCT cdoc)
        {
            if (cdoc.Document.xcuda_Valuation == null)
                cdoc.Document.xcuda_Valuation = new xcuda_Valuation(true)
                {
                    ASYCUDA_Id = cdoc.Document.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                };
            if (cdoc.Document.xcuda_Valuation.xcuda_Weight == null)
                cdoc.Document.xcuda_Valuation.xcuda_Weight = new xcuda_Weight(true)
                {
                    Valuation_Id = cdoc.Document.xcuda_Valuation.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                };

            var xcudaPreviousItems = cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Where(x => x != null)
                .ToList();
            if (xcudaPreviousItems.Any())
            {
                cdoc.Document.xcuda_Valuation.xcuda_Weight.Gross_weight =
                    (double)xcudaPreviousItems.Sum(x => x.Net_weight);
            }
        }

        private IEnumerable<DataSpace.BaseDataModel.MyPodData> PrepareAllocationsData(AllocationDataBlock monthyear,
            bool isGrouped, string sql)
        {
            try
            {
                List<DataSpace.BaseDataModel.MyPodData> elst;
                //if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true)
                //{
                elst = isGrouped 
                    ? GroupAllocationsByPreviousItem(monthyear) 
                    : SingleAllocationPerPreviousItem(monthyear, sql);
                //}
                //else
                //{
                //elst = GroupAllocations(monthyear);
                //}

                return elst.ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<DataSpace.BaseDataModel.MyPodData> GroupAllocationsByPreviousItem(AllocationDataBlock monthyear)
        {
            try
            {
                var elst = monthyear.Allocations
                    .OrderBy(x => x.pTariffCode)
                    .GroupBy(x =>  x.PreviousItem_Id)
                    .Select(s => CreateMyPodData(s.ToList(), monthyear.Filter)).ToList();
               


                return elst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataSpace.BaseDataModel.MyPodData CreateMyPodData(List<EX9Allocations> s, (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter)
        {
            try
            {

                return new DataSpace.BaseDataModel.MyPodData
                {
                    Filter = filter,
                    Allocations = s.OrderByDescending(x => x.AllocationId).Select(x =>
                        new AsycudaSalesAllocations()
                        {
                            AllocationId = x.AllocationId,
                            PreviousItem_Id = x.PreviousItem_Id,
                            EntryDataDetailsId = x.EntryDataDetailsId,
                            Status = x.Status,
                            QtyAllocated = x.QtyAllocated,
                            PIData = x.PIData,
                            EntryDataDetails = x.EntryDataDetails,


                        }).ToList(),
                    AllNames = s.SelectMany(x =>
                        {
                            var alias = Enumerable.Where<InventoryItem>(InventoryDataCache, c => c.Id == x.InventoryItemId)
                                .SelectMany(i => i.InventoryItemAlias)
                                .ToList();
                            
                            var aliasNames = alias.Select(a => a.AliasItem.ItemNumber).ToList();

                            var aliasAlias = Enumerable.Where<InventoryItem>(InventoryDataCache, z => z.InventoryItemAlias.Any(a => aliasNames.Contains(a.AliasItem.ItemNumber)) ).SelectMany(z => z.InventoryItemAlias).ToList();
                            //.Where(c => alias.Select(z => z.AliasItemId).ToList().Any(z => z == c.Id))
                            //.SelectMany(i => i.InventoryItemAlias);
                            var aaNames = aliasAlias.Select(a => a.AliasItem.ItemNumber).ToList();

                            

                            var firstLst = aliasNames.Union(aaNames).ToList();

                            var secondLst = MoreEnumerable.Append(firstLst, x.ItemNumber);

                            return secondLst;
                        })
                        .Distinct()
                        .ToList(),
                    EntlnData = new AlloEntryLineData
                    {
                        ItemNumber = s.LastOrDefault()?.ItemNumber,
                        ItemDescription = s.LastOrDefault()?.ItemDescription,
                        TariffCode = s.LastOrDefault()?.pTariffCode,
                        Cost = s.LastOrDefault()?.pItemCost.GetValueOrDefault()??0.0,
                        FileTypeId = s.LastOrDefault()?.FileTypeId,
                        EmailId = s.LastOrDefault()?.EmailId,
                        Quantity = s.Sum(x => x.QtyAllocated / (x.SalesFactor == 0 ? 1 : x.SalesFactor)),
                        EntryDataDetails = s.DistinctBy(z => z.EntryDataDetailsId).Select(z =>
                            new EntryDataDetailSummary()
                            {
                                EntryDataDetailsId = z.EntryDataDetailsId.GetValueOrDefault(),
                                EntryDataId = z.InvoiceNo,
                                EntryData_Id = z.EntryData_Id,
                                SourceFile = z.SourceFile,
                                QtyAllocated = z.SalesQuantity,
                                EntryDataDate = z.InvoiceDate,
                                EffectiveDate = z.EffectiveDate.GetValueOrDefault(),
                                Currency = z.Currency,
                                Comment = z.Comment
                            }).ToList(),
                        PreviousDocumentItemId = Convert.ToInt32(s.LastOrDefault()?.PreviousItem_Id ?? 0),
                        InternalFreight = s.LastOrDefault()?.InternalFreight ?? 0.0,
                        Freight = s.LastOrDefault()?.Freight ?? 0.0,
                        Weight = s.LastOrDefault()?.Weight ?? 0.0,
                        pDocumentItem = new pDocumentItem()
                        {
                            DFQtyAllocated = s.LastOrDefault()?.DFQtyAllocated??0,
                            DPQtyAllocated = s.LastOrDefault()?.DPQtyAllocated??0,
                            ItemQuantity = s.LastOrDefault()?.pQuantity.GetValueOrDefault()??0,
                            LineNumber = s.LastOrDefault()?.pLineNumber??0,
                            ItemNumber = s.LastOrDefault()?.pItemNumber,
                            CustomsProcedure = s.LastOrDefault()?.CustomsProcedure,
                            Description = s.LastOrDefault()?.pItemDescription,
                            xcuda_ItemId = s.LastOrDefault()?.PreviousItem_Id.GetValueOrDefault()??0,
                            AssessmentDate = s.LastOrDefault()?.pAssessmentDate??DateTime.MinValue,
                            ExpiryDate = s.LastOrDefault()?.pExpiryDate??DateTime.MinValue,
                            previousItems = s.LastOrDefault().previousItems
                        },
                        EX9Allocation = new EX9Allocation()
                        {
                            SalesFactor = s.LastOrDefault()?.SalesFactor ?? 0.0,
                            Net_weight_itm = s.LastOrDefault()?.Net_weight_itm??0.0m,
                            pQuantity = s.LastOrDefault()?.pQuantity.GetValueOrDefault() ?? 0.0,
                            pCNumber = s.LastOrDefault()?.pCNumber,
                            Customs_clearance_office_code = s.LastOrDefault()?.Customs_clearance_office_code,
                            Country_of_origin_code = s.LastOrDefault()?.Country_of_origin_code,
                            pRegistrationDate = s.LastOrDefault()?.pRegistrationDate??DateTime.MinValue,
                            pQtyAllocated = s.LastOrDefault()?.QtyAllocated ?? 0.0,
                            Total_CIF_itm = s.LastOrDefault()?.Total_CIF_itm ?? 0.0,
                            pTariffCode = s.LastOrDefault()?.pTariffCode,
                            pPrecision1 = s.LastOrDefault()?.pPrecision1
                        },
                        TariffSupUnitLkps = s.LastOrDefault()?.TariffSupUnitLkps == null ? new List<ITariffSupUnitLkp>(): s.LastOrDefault()?.TariffSupUnitLkps.Select(x => (ITariffSupUnitLkp)x)
                            .ToList()

                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<DataSpace.BaseDataModel.MyPodData> SingleAllocationPerPreviousItem(AllocationDataBlock monthyear, string sql)
        {
            try
            {
                var xSalesByItemId = monthyear.Allocations
                    .OrderBy(x => x.AllocationId).Select(x => (Item: (x.ItemNumber, x.InventoryItemId), xSale: x))
                    .GroupBy(x => x.Item)
                    .Select(x => (x.Key, Value: x.Select(i => i.xSale).ToList()))
                    .ToList();

                var groupedlst = DataSpace.Utils.CreateItemSet<EX9Allocations>(xSalesByItemId);

                //var groupedlst = monthyear.Allocations
                //    .OrderBy(x => x.AllocationId)
                //    .GroupBy(x => x.PreviousItem_Id)
                //    .ToList();

                var elst = SplitToLines(sql, groupedlst);

                var res = elst.Select(list => CreateMyPodData(list, monthyear.Filter)).ToList();

              
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<List<EX9Allocations>> SplitToLines(string sql, Dictionary<int, List<((string ItemNumber, int InventoryItemId) Key, List<EX9Allocations> Value)>> groupedlst)
        {
            var elst = new List<List<EX9Allocations>>();

            foreach (var group in groupedlst)
            {
                var lst = group.Value.SelectMany(z => z.Value.Select(v => v).ToList()).ToList();
                var nlst = new List<EX9Allocations>();
                while (lst.Any())
                {
                    // focus on returns ignore weight for now

                    var itm = lst.First();
                    if (itm.QtyAllocated > 0)
                    {
                        if (nlst.Sum(x => x.QtyAllocated) + itm.QtyAllocated > 0)
                        {
                            nlst = new List<EX9Allocations> { itm };
                            lst.RemoveAt(0);
                        }
                        else
                        {
                            nlst.Add(itm);
                            lst.RemoveAt(0);
                        }
                    }
                    else
                    {
                        if (nlst.Sum(x => x.QtyAllocated) + itm.QtyAllocated > 0 && itm.QtyAllocated > 0)
                        {
                            if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) > 0) elst.Add(nlst);
                            nlst = new List<EX9Allocations> { itm };
                            lst.RemoveAt(0);
                        }
                        else
                        {
                            nlst.Add(itm);
                            lst.RemoveAt(0);
                        }
                    }

                    if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) > 0 && (lst.Any() && lst.First().QtyAllocated > 0) || !lst.Any()) elst.Add(nlst);
                }

                if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) < 0)
                    UpdateXStatus(
                        nlst.Select(x => new AsycudaSalesAllocations()
                        {
                            AllocationId = x.AllocationId, EntryDataDetailsId = x.EntryDataDetailsId,
                            PreviousItem_Id = x.PreviousItem_Id
                        }).ToList(), $"Set < 0: {nlst.Sum(x => x.QtyAllocated)}",ref sql);
            }

            return elst;
        }

        private void InsertEntryIdintoRefNum(DocumentCT cdoc, string entryDataId)
        {
            try
            {
                if (DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry == true)
                {
                    cdoc.Document.xcuda_Declarant.Number = entryDataId;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<(int itmCount,string sql)> CreateEx9EntryAsync(DataSpace.BaseDataModel.MyPodData mypod, DocumentCT cdoc,
            int itmcount, string dfp,
            string documentType, List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity,
            bool applyEx9Bucket, string ex9BucketType, bool applyHistoricChecks, bool applyCurrentChecks,
            bool overPIcheck, bool universalPIcheck, bool itemPIcheck,
            ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems)
        {
            try
            {
                string sql = "";
                // clear all xstatus so know what happened
                //sql += UpdateXStatus(mypod.Allocations,null);

                if (mypod.EntlnData.pDocumentItem.ExpiryDate <= DateTime.Now && (DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ExportExpiredEntries??true) == false)
                {
                    if(Math.Abs(mypod.EntlnData.pDocumentItem.ItemQuantity - mypod.EntlnData.pDocumentItem.PiQuantity) > 0)
                     UpdateXStatus(mypod.Allocations,
                        $@"Expired Entry: '{
                            mypod.EntlnData.pDocumentItem.ExpiryDate.ToShortDateString()}'",ref sql);
                    return (0,sql);
                }


                /////////////// QtyAllocated >= piQuantity cap
                ///
                /// 
                /// 
                if (checkQtyAllocatedGreaterThanPiQuantity)
                {
                   
                    //var psum = mypod.EntlnData.pDocumentItem.previousItems
                    //    .DistinctBy(x => x.PreviousItem_Id)
                    //    .Select(x => x.Suplementary_Quantity).DefaultIfEmpty(0).Sum();
                    //if (mypod.EntlnData.pDocumentItem.QtyAllocated <= psum)
                    //{
                    //    updateXStatus(mypod.Allocations,
                    //        $@"Failed QtyAllocated <= PiQuantity:: QtyAllocated: {
                    //                mypod.EntlnData.pDocumentItem.QtyAllocated
                    //            } PiQuantity: {psum}");
                    //    return 0;
                    //}

                   
                }
                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic

                List<ItemSalesPiSummary> salesPiAll = new List<ItemSalesPiSummary>();
                List<ItemSalesPiSummary> universalData = new List<ItemSalesPiSummary>(); ;
                List<ItemSalesPiSummary> salesPiHistoric;
                List<ItemSalesPiSummary> salesPiCurrent;
                List<ItemSalesPiSummary> itemSalesPiHistoric = new List<ItemSalesPiSummary>();
                List<ItemSalesPiSummary> itemSalesPiCurrent = new List<ItemSalesPiSummary>();

                universalData = itemSalesPiSummaryLst.Where(x => (mypod.AllNames.Contains(x.ItemNumber) || x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId)
                                                                 && x.Type == "Universal").ToList();

                salesPiAll = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                              //--------took out the name and line because the id more precise especially with item aliases - rouge - "14479 vs 014479"
                                                              // x.ItemNumber == mypod.EntlnData.ItemNumber &&
                                                              // x.pCNumber == mypod.EntlnData.EX9Allocation
                                                              //    .pCNumber // donot disable this because previous month pi are not included
                                                              //&& x.pLineNumber == mypod.EntlnData.pDocumentItem
                                                              //    .LineNumber
                                                              && x.Type == "All").ToList();
                salesPiHistoric = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                   //x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                   // took this out because of changed allocation can lead to overallocation
                                                                   //eg HS/SAN2, nov 2018 - reallocated to HS/SAN2 but orignally allocated to HS-SAN2

                                                                   //&& x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                   //&& x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                   && x.Type == "Historic").ToList();
                salesPiCurrent = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                  //  x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                  //&& x.pCNumber == mypod.EntlnData.EX9Allocation
                                                                  //    .pCNumber // donot disable this because previous month pi are not included
                                                                  //&& x.pLineNumber == mypod.EntlnData.pDocumentItem
                                                                  //    .LineNumber
                                                                  && x.Type == "Current").ToList();

                itemSalesPiHistoric = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                       //x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                       //&& x.pCNumber == mypod.EntlnData
                                                                       //    .EX9Allocation.pCNumber
                                                                       //&& x.pLineNumber ==
                                                                       //mypod.EntlnData.pDocumentItem.LineNumber
                                                                       && x.Type == "Historic").ToList();
                //itemSalesPiCurrent = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber
                //                                                  && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                //                                                  && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                //                                                  && x.Type == "Current").ToList();


                Debug.WriteLine(
                    $"Create EX9 For {mypod.EntlnData.ItemNumber}:{mypod.EntlnData.EntryDataDetails.First().EntryDataDate:MMM-yy} - {mypod.EntlnData.Quantity} | C#{mypod.EntlnData.EX9Allocation.pCNumber}-{mypod.EntlnData.pDocumentItem.LineNumber}");
                salesPiHistoric.ForEach(x =>
                    Debug.WriteLine(
                        $"Sales vs Pi History: {x.QtyAllocated} of {/*x.pQtyAllocated*/""} - {x.PiQuantity} | C#{x.pCNumber}-{x.pLineNumber}"));
                var entryType = salesPiHistoric.FirstOrDefault()?.EntryDataType ?? documentType;//salesPiAll.FirstOrDefault()?.EntryDataType?? documentType;

                var docSetPiLst = DocSetPi
                    .Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId)
                    .ToList();

                var docPi = docSetPiLst
                    .Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();

                var itemDocPi = docSetPiLst
                    .Where(x => x.Type == entryType)
                    .Where(x => x.DutyFreePaid == dfp)//c#17227 line 40 -Appid = 7
                    .Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();

                // var universalSalesAll = (double)universalData.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault()?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();

                var previousItemUniversalData = universalData.GroupBy(x => x.PreviousItem_Id).ToList();

                //var universalSalesAll = (double) previousItemUniversalData.Select(x => x.FirstOrDefault(q => q.pQtyAllocated > 0)?.SalesQuantity).DefaultIfEmpty(0.0).Sum();
                var universalSalesAll = (double)previousItemUniversalData.SelectMany(x => x.Select(z => z.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                var universalPiAll = (double) previousItemUniversalData.Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();

                // var totalSalesAll = (double)salesPiAll.GroupBy(x => x.PreviousItem_Id).SelectMany(x => x.Select(z => z.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                var totalSalesAll = (double) salesPiAll.GroupBy(x => x.PreviousItem_Id).SelectMany(x => x.Select(q => q.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                var totalPiAll = (double) salesPiAll.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();
                var totalSalesHistoric = (double)salesPiHistoric.GroupBy(x => x.PreviousItem_Id).SelectMany(x => x.Select(z => z.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                //var totalSalesHistoric = (double) salesPiHistoric.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.pQtyAllocated > 0)?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();
                var totalPiHistoric = (double) salesPiHistoric.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();
                var totalSalesCurrent = (double) salesPiCurrent.Select(x => x.QtyAllocated).DefaultIfEmpty(0.0).Sum();
                var totalPiCurrent = (double) salesPiCurrent.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();

                var itemPiHistoric = itemSalesPiHistoric.GroupBy(x => x.PreviousItem_Id)
                    .Select(x => x.First().PiQuantity).DefaultIfEmpty(0).Sum();

                var itemSalesHistoric = (double)itemSalesPiHistoric.Select(x => x.QtyAllocated).DefaultIfEmpty(0.0).Sum();

              
                //if(mypod.Filter.startDate == DateTime.Parse("1/1/2022") && mypod.EntlnData.PreviousDocumentItemId == 362441) Debugger.Break();
                var preEx9Bucket = mypod.EntlnData.Quantity;
                if (applyEx9Bucket)
                    if (ex9BucketType == "Current")
                    {
                        Ex9Bucket(mypod, dfp, docPi, salesPiCurrent.Any() ? salesPiCurrent : salesPiAll, docPreviousItems, ref sql);
                    }
                    else
                    {
                        Ex9Bucket(mypod, mypod.EntlnData.Quantity, totalSalesHistoric, totalPiHistoric, "Historic", ref sql);
                        Ex9Bucket(mypod, mypod.EntlnData.Quantity, totalSalesCurrent, totalPiCurrent, "Current", ref sql);
                    }

                var salesFactor = Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                    ? 1
                    : mypod.EntlnData.EX9Allocation.SalesFactor;


                

                if (overPIcheck)
                    if (Math.Round( totalSalesAll, 2) <
                        Math.Round((totalPiAll  + docPi + mypod.EntlnData.Quantity) * salesFactor, 2))//
                    {
                        var availibleQty = Math.Round(totalSalesAll, 2) - (Math.Round(totalPiAll, 2)+ docPi);
                        if (availibleQty <= 0)
                        {
                            UpdateXStatus(mypod.Allocations,
                                $@"Failed All Sales Check:: Total All Sales:{Math.Round(totalSalesAll, 2)}
                                            Total All PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}", ref sql);
                            return (0, sql);
                        }

                        Ex9Bucket(mypod, availibleQty, totalSalesAll, totalPiAll, "Total",ref sql);
                    }

                if (universalPIcheck)
                    if (Math.Round(universalSalesAll, 2) <
                        Math.Round((universalPiAll + docPi + mypod.EntlnData.Quantity) * salesFactor, 2))//
                    {
                        var availibleQty = Math.Round(universalSalesAll, 2) - (Math.Round(universalPiAll, 2)+ docPi);
                        if (availibleQty <= 0)
                        {
                           UpdateXStatus(mypod.Allocations,
                                $@"Failed universal Sales Check:: Universal Sales:{Math.Round(universalSalesAll, 2)}
                                            Universal PI: {universalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}", ref sql);
                            return (0, sql);
                        }

                        Ex9Bucket(mypod, availibleQty, universalSalesAll, universalPiAll, "Universal",ref sql);

                    }

                mypod.EntlnData.Quantity = Math.Round(mypod.EntlnData.Quantity, 2);
                Debug.WriteLine($"EX9Bucket Quantity {mypod.EntlnData.ItemNumber} - {mypod.EntlnData.Quantity}");
                if (mypod.EntlnData.Quantity <= 0)
                {
                    UpdateXStatus(mypod.Allocations,
                        $@"Failed Ex9Bucket set Qty to Zero:: preQty: {preEx9Bucket}",ref sql);
                    return (0, sql);
                }

                ////////////////////////----------------- Cap to prevent xQuantity > Sales Quantity
                double qtyAllocated = 0;
                foreach (var allocation in mypod.Allocations)
                {
                    qtyAllocated += allocation.QtyAllocated /
                                    (Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                                        ? 1
                                        : mypod.EntlnData.EX9Allocation.SalesFactor);
                    allocation.xStatus = "";
                }


                // todo: ensure allocations are marked for investigation
                double qty = mypod.EntlnData.Quantity;
                if ((qty - Math.Round(qtyAllocated, 2))  > 0.0001)
                {
                    UpdateXStatus(mypod.Allocations,
                        $@"Failed Quantity vs QtyAllocated:: Qty: {qty} QtyAllocated: {qtyAllocated}",ref sql);
                    return (0, sql);
                }
                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic



               


                if (documentType == "Sales" && totalSalesAll == 0 && universalPIcheck )// && mypod.Allocations.FirstOrDefault()?.Status != "Short Shipped"
                {
                    UpdateXStatus(mypod.Allocations,
                        $@"No Sales Found",ref sql);
                    return (0, sql); // no sales found
                }


                


                if (applyHistoricChecks && totalSalesHistoric + totalPiHistoric > 0)// if sales + pi is zero this is first sale so can't do historic checks
                {
                    

                    var docDFPpi = DocSetPi.Where(x => x.DutyFreePaid == dfp && x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId).Sum(x => x.TotalQuantity);
                    if (Math.Round(totalSalesHistoric, 2) <
                        Math.Round((totalPiHistoric + docDFPpi + mypod.EntlnData.Quantity) * salesFactor, 2))// take out docpi because its included in sales
                    {
                        //updateXStatus(mypod.Allocations,
                        //    $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                        //       Total Historic PI: {totalPiHistoric}
                        //       xQuantity:{mypod.EntlnData.Quantity}");
                        //return 0;
                        var availibleQty = totalSalesHistoric - (totalPiHistoric + docDFPpi);
                        if (availibleQty != 0) Ex9Bucket(mypod, availibleQty, totalSalesHistoric, totalPiHistoric, "Historic",ref sql);
                        if (mypod.EntlnData.Quantity == 0 || availibleQty == 0)
                        {
                            UpdateXStatus(mypod.Allocations,
                                $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                                   Total Historic PI: {totalPiHistoric}
                                   xQuantity:{mypod.EntlnData.Quantity}",ref sql);
                            return (0, sql);
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////

                if (applyCurrentChecks)
                {
                    //////////////////////////////////////////////////////////////////////////
                    ///     Sales Cap/ Sales Bucket Current


                    if (totalSalesCurrent == 0)// && mypod.Allocations.FirstOrDefault()?.Status != "Short Shipped"
                    {
                        UpdateXStatus(mypod.Allocations,
                            $@"No Current Sales Found",ref sql);
                        return (0, sql);
                    }

                    // no sales found


                    if (Math.Round(totalSalesCurrent, 2) <
                        Math.Round((totalPiCurrent + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        UpdateXStatus(mypod.Allocations,
                            $@"Failed Current Check:: Total Current Sales:{Math.Round(totalSalesCurrent, 2)}
                               Total Current PI: {totalPiCurrent}
                               xQuantity:{mypod.EntlnData.Quantity}",ref sql);
                        return (0, sql);
                    }
                }

                // mandatory check to prevent overtaking
                if (mypod.EntlnData.pDocumentItem.ItemQuantity <
                    Math.Round((totalPiAll + docPi + mypod.EntlnData.Quantity), 2))
                {

                    var availibleQty = mypod.EntlnData.pDocumentItem.ItemQuantity - (totalPiAll + docPi);
                    if (availibleQty != 0) Ex9Bucket(mypod, availibleQty, mypod.EntlnData.pDocumentItem.ItemQuantity, totalPiAll + docPi, "Historic",ref sql);
                    if (mypod.EntlnData.Quantity == 0 || availibleQty == 0)
                    {
                        UpdateXStatus(mypod.Allocations,
                            $@"Failed ItemQuantity < totalPiAll & xQuantity:{mypod.EntlnData.pDocumentItem.ItemQuantity}
                               totalPiAll PI: {totalPiAll}
                               xQuantity:{mypod.EntlnData.Quantity}",ref sql);
                        return (0, sql);
                    }

                   
                }

                //// item sales vs item pi, prevents early exwarehouse when its just one totalsales vs totalpi
                //if(documentType != "DIS")
                if (itemPIcheck && itemSalesHistoric + itemPiHistoric > 0) // need to investigate this same reason as totalsaleshistory
                {
                    /// re-enabled for EX9ALL because its over taking - Appid = 6, INV: 29916-5803 
                    if (Math.Round(itemSalesHistoric, 2) <
                        Math.Round((itemPiHistoric + itemDocPi + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        //updateXStatus(mypod.Allocations,
                        //    $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                        //       Total Historic PI: {totalPiHistoric}
                        //       xQuantity:{mypod.EntlnData.Quantity}");
                        //return 0;
                        var availibleQty = itemSalesHistoric - (itemPiHistoric + itemDocPi);
                        Ex9Bucket(mypod, availibleQty, itemSalesHistoric, itemPiHistoric, "Historic",ref sql);
                        if (mypod.EntlnData.Quantity == 0)
                        {
                            UpdateXStatus(mypod.Allocations,
                                $@"Failed Item Historical Check:: Item Historic Sales:{Math.Round(itemSalesHistoric, 2)}
                                   Item Historic PI: {itemPiHistoric}
                                   xQuantity:{mypod.EntlnData.Quantity}",ref sql);
                            return (0,sql);
                        }
                    }

                   
                    ////////////////////////////////////////////////////////////////////////
                    //// Cap to prevent over creation of ex9 vs Item Quantity espectially if creating Duty paid and Duty Free at same time

                    if (mypod.EntlnData.pDocumentItem.ItemQuantity <
                        Math.Round(itemPiHistoric + itemDocPi + mypod.EntlnData.Quantity, 2))
                    {

                        var availibleQty = mypod.EntlnData.pDocumentItem.ItemQuantity - (itemPiHistoric + itemDocPi);
                        /// pass item quantity to ex9 bucket
                        //Debugger.Break();
                        Ex9Bucket(mypod, availibleQty, mypod.EntlnData.pDocumentItem.ItemQuantity, itemPiHistoric + itemDocPi , "Historic", ref sql);
                        if (mypod.EntlnData.Quantity == 0)
                        {
                            UpdateXStatus(mypod.Allocations,
                                $@"Failed ItemQuantity < ItemPIHistoric & xQuantity:{mypod.EntlnData.pDocumentItem.ItemQuantity}
                               Item Historic PI: {itemPiHistoric}
                               xQuantity:{mypod.EntlnData.Quantity}",ref sql);
                            return (0,sql);
                        }
                    }


                  

                }
                
                ////////////////////////////////////////////////////////////////////////
                //// Sales dependent check to prevent sales overexwarehouse even when Allocations changed. all other checks are previous document dependent
                var xSalesPi = mypod.Allocations
                    .Select(x => new {e = x.EntryDataDetails.Quantity,pi = x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails.Where(z => z.CustomsOperation == "Exwarehouse").Sum(z => z.Quantity)?? 0}).Sum(x => x.e - x.pi) ;
                ///// the DONT FORGET WE TAKING JUST REMAINING SALES SO IT WILL BE ZERO EVEN IF ITS ALREADY DOUBLE EX-WAREHOUSED
                if (xSalesPi < Math.Round(mypod.EntlnData.Quantity, 2))//take out docpi its throwing it ouff   //itemDocPi + 
                {

                    var availibleQty = xSalesPi - Math.Round( mypod.EntlnData.Quantity, 2);//take out docpi its throwing it ouff   //itemDocPi + 
                    /// pass item quantity to ex9 bucket
                    if (availibleQty >= 0)
                        Ex9Bucket(mypod, availibleQty, itemSalesHistoric, itemPiHistoric, "Historic", ref sql);
                    if (mypod.EntlnData.Quantity <= 0 || availibleQty <= 0)
                    {
                        UpdateXStatus(mypod.Allocations,
                            $@"Failed Existing xSales already taken out..:{mypod.Allocations.SelectMany(x => x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails.Select(n => $"c#{n.CNumber}|{n.LineNumber}").ToList()).DefaultIfEmpty("").Aggregate((o,n) => $"{o}, {n}")}", ref sql);
                        return (0,sql);
                    }
                }

                if (mypod.EntlnData.Quantity <= 0) return (0,sql);
                //////////////////// can't delump allocations because of returns and 1kg weights issue too much items wont be able to exwarehouse
                CreateEx9Item(mypod, cdoc, itmcount, dfp, docPreviousItems,ref sql, entryType, out var itmsCreated); 


                return (itmsCreated,sql);
            }
            catch (Exception Ex)
            {
                throw;
            }


        }

        private bool CreateEx9Item(DataSpace.BaseDataModel.MyPodData mypod, DocumentCT cdoc, int itmcount, string dfp, ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems,
           ref string sql, string entryType, out int itmsCreated)
        {
            var itmsToBeCreated = 1;
            itmsCreated = 0;


            for (int i = 0; i < itmsToBeCreated; i++)
            {
                var lineData = mypod.EntlnData;

                var pitm = CreatePreviousItem(lineData, itmcount + i, docPreviousItems);

                if (IsPiWeightLessthanMinimum(mypod, ref sql, ref itmsCreated, pitm)) return true;

                pitm.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;

                var itm = DataSpace.BaseDataModel.Instance.CreateItemFromEntryDataDetail(lineData, cdoc);

                SwitchCustomsProcedureBasedonIM7(cdoc, lineData, itm);
                   


                ProcessAllocations(mypod, itm);
                SetBasicInfofromPreviousItem(itm, lineData, pitm);

                var previousItems = CreatePreviousItems(dfp, pitm);
                UpdateDocPreviousItems(docPreviousItems, lineData, previousItems);


                CreateEntryPreviousItems(lineData, pitm);


                SetPackageInfo(mypod, cdoc, itmcount, pitm, itm, lineData);

                UpdatePackageInfoFromPreviousItem(pitm, itm);


                


                CreateWeightItm(itm, pitm);
                // adjusting because not using real statistical value when calculating
                SetValuationItm(itm, pitm);

                UpdateDocSetPi(mypod, dfp, entryType);

                DataSpace.BaseDataModel.Instance.ProcessItemTariff(mypod.EntlnData, cdoc.Document, itm);

                itmsCreated += 1;
            }

            return false;
        }

        private static void SwitchCustomsProcedureBasedonIM7(DocumentCT cdoc, AlloEntryLineData lineData, xcuda_Item itm)
        {
            if (!lineData.pDocumentItem.CustomsProcedure.StartsWith(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                    .Customs_Procedure.CustomsProcedure.Substring(2, 2)))
            {
                var altCustomProcedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                    .Customs_Procedure.InCustomsProcedure.FirstOrDefault(x =>
                        x.InCustomsProcedure.CustomsProcedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                            .Customs_Procedure.CustomsProcedure)
                    ?.OutCustomsProcedure;
                if (altCustomProcedure != null)
                {
                    itm.xcuda_Tarification.National_customs_procedure = altCustomProcedure
                        .National_customs_procedure;
                    itm.xcuda_Tarification.Extended_customs_procedure = altCustomProcedure
                        .Extended_customs_procedure;
                }
                else
                {
                    throw new ApplicationException(
                        $"Customs Procedure '{lineData.pDocumentItem.CustomsProcedure}' not found in '{cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsProcedure}'");
                }
            }
        }

        private static void SetBasicInfofromPreviousItem(xcuda_Item itm, AlloEntryLineData lineData, xcuda_PreviousItem pitm)
        {
            itm.EmailId = lineData.EmailId;
            itm.xcuda_Valuation_item.Total_CIF_itm = pitm.Current_value;
            itm.xcuda_Tarification.xcuda_HScode.Precision_4 = lineData.pDocumentItem.ItemNumber;
            itm.xcuda_Tarification.xcuda_HScode.Precision_1 = lineData.EX9Allocation.pPrecision1;
            itm.xcuda_Goods_description.Commercial_Description =
                DataSpace.BaseDataModel.Instance.CleanText(lineData.pDocumentItem.Description);
            itm.IsAssessed = false;
            itm.SalesFactor = lineData.EX9Allocation.SalesFactor;


            itm.xcuda_PreviousItem = pitm;
            pitm.xcuda_Item = itm;

            itm.ItemQuantity =
                (double)pitm
                    .Suplementary_Quantity; // taking the previous item quantity because thats governing thing when exwarehousing. c#36689 51 9/1/2021
            itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pitm.Hs_code;
            itm.xcuda_Goods_description.Country_of_origin_code = pitm.Goods_origin;
        }

        private static void SetValuationItm(xcuda_Item itm, xcuda_PreviousItem pitm)
        {
            itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency =
                Convert.ToDouble(Math.Round((pitm.Current_value * (double)pitm.Suplementary_Quantity), 2));
            itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency =
                Convert.ToDouble(Math.Round(pitm.Current_value * (double)pitm.Suplementary_Quantity, 2));
            itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_code = "XCD";
            itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_rate = 1;
        }

        private static void UpdateDocSetPi(DataSpace.BaseDataModel.MyPodData mypod, string dfp, string entryType)
        {
            DocSetPi.Add(new PiSummary()
            {
                ItemNumber = mypod.EntlnData.ItemNumber,
                PreviousItem_Id = mypod.EntlnData.PreviousDocumentItemId,
                DutyFreePaid = dfp,
                Type = entryType,
                TotalQuantity = mypod.EntlnData.Quantity,
                pCNumber = mypod.EntlnData.EX9Allocation.pCNumber,
                pLineNumber = mypod.EntlnData.pDocumentItem.LineNumber.ToString()
            });
        }

        private static void CreateWeightItm(xcuda_Item itm, xcuda_PreviousItem pitm)
        {
            itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
            {
                TrackingState = TrackingState.Added,
                Gross_weight_itm = pitm.Net_weight,
                Net_weight_itm = pitm.Net_weight
            };
        }

        private static void UpdatePackageInfoFromPreviousItem(xcuda_PreviousItem pitm, xcuda_Item itm)
        {
            if (pitm.Previous_Packages_number != null && pitm.Previous_Packages_number != "0")
            {
                var pkg = itm.xcuda_Packages.FirstOrDefault();
                if (pkg == null)
                {
                    pkg = new xcuda_Packages(true)
                    {
                        Item_Id = itm.Item_Id,
                        Marks1_of_packages = "Marks",
                        TrackingState = TrackingState.Added
                    };
                    itm.xcuda_Packages.Add(pkg);
                }

                pkg.Number_of_packages =
                    Convert.ToDouble(pitm.Previous_Packages_number);
            }
        }

        private static void SetPackageInfo(DataSpace.BaseDataModel.MyPodData mypod, DocumentCT cdoc, int itmcount, xcuda_PreviousItem pitm,
            xcuda_Item itm, AlloEntryLineData lineData)
        {
            if (cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Count() == 1 || itmcount == 0)
            {
                pitm.Packages_number = "1"; //(i.Packages.Number_of_packages).ToString();
                pitm.Previous_Packages_number = pitm.Previous_item_number == 1 ? "1" : "0";


                CreateXcudaAttachements(cdoc, itm, lineData);

                AddSourceFileAttachment(mypod, cdoc, itm, lineData);
            }
            else
            {
                SetOtherPackageInfo(pitm);
            }
        }

        private static void SetOtherPackageInfo(xcuda_PreviousItem pitm)
        {
            if (pitm.Packages_number == null)
            {
                pitm.Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                pitm.Previous_Packages_number = (0).ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void AddSourceFileAttachment(DataSpace.BaseDataModel.MyPodData mypod, DocumentCT cdoc, xcuda_Item itm,
            AlloEntryLineData lineData)
        {
            var sourceFile = new FileInfo(mypod.EntlnData.EntryDataDetails[0].SourceFile);
            var sourceFilePdf = sourceFile.FullName.Replace(sourceFile.Extension, ".pdf");
            sourceFilePdf = sourceFilePdf.Replace("-Fixed", "");
            if (File.Exists(sourceFilePdf))
                itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                {
                    Attached_document_code = "IV05",
                    Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                    Attached_document_reference = cdoc.Document.ReferenceNumber + ".pdf",
                    xcuda_Attachments = new List<xcuda_Attachments>()
                    {
                        new xcuda_Attachments(true)
                        {
                            Attachments = new Attachments(true)
                            {
                                FilePath = sourceFilePdf,
                                TrackingState = TrackingState.Added,
                                DocumentCode = @"IV05",
                                EmailId = lineData.EmailId?.ToString(),
                                Reference = cdoc.Document.ReferenceNumber,
                            },

                            TrackingState = TrackingState.Added
                        }
                    },
                    TrackingState = TrackingState.Added
                });
        }

        private static void CreateXcudaAttachements(DocumentCT cdoc, xcuda_Item itm, AlloEntryLineData lineData)
        {
            itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
            {
                Attached_document_code =
                    DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                        x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                            .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "DFS1",
                Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                Attached_document_reference = cdoc.Document.ReferenceNumber,
                xcuda_Attachments = new List<xcuda_Attachments>()
                {
                    new xcuda_Attachments(true)
                    {
                        Attachments = new Attachments(true)
                        {
                            FilePath = Path.Combine(
                                DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.DataFolder == null
                                    ? cdoc.Document.ReferenceNumber + ".pdf"
                                    : $"{DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.DataFolder}\\{cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number}\\{cdoc.Document.ReferenceNumber}.pdf"),
                            TrackingState = TrackingState.Added,
                            DocumentCode = DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                                x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                    .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "DFS1",
                            EmailId = lineData.EmailId?.ToString(),
                            Reference = cdoc.Document.ReferenceNumber,
                        },

                        TrackingState = TrackingState.Added
                    }
                },
                TrackingState = TrackingState.Added
            });
        }

        private static void CreateEntryPreviousItems(AlloEntryLineData lineData, xcuda_PreviousItem pitm)
        {
            var ep = new EntryPreviousItems(true)
            {
                Item_Id = lineData.PreviousDocumentItemId,
                PreviousItem_Id = pitm.PreviousItem_Id,
                TrackingState = TrackingState.Added
            };
            pitm.xcuda_Items.Add(ep);
        }

        private static void UpdateDocPreviousItems(ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems, AlloEntryLineData lineData,
            PreviousItems previousItems)
        {
            //if (docPreviousItems.ContainsKey(lineData.PreviousDocumentItemId))
            //{
            //    docPreviousItems[lineData.PreviousDocumentItemId].Add(previousItems);
            //}
            //else
            //{
                docPreviousItems.AddOrUpdate(lineData.PreviousDocumentItemId,new List<PreviousItems>(){previousItems}, (k, o) =>
                {
                    o.Add(previousItems);
                    return o ;
                });
            //}
        }

        private static PreviousItems CreatePreviousItems(string dfp, xcuda_PreviousItem pitm)
        {
            var previousItems = new PreviousItems()
            {
                DutyFreePaid = dfp, Net_weight = (double)pitm.Net_weight,
                PreviousItem_Id = pitm.PreviousItem_Id,
                Suplementary_Quantity = (double)pitm.Suplementary_Quantity
            };
            return previousItems;
        }

        private void ProcessAllocations(DataSpace.BaseDataModel.MyPodData mypod, xcuda_Item itm)
        {
            //TODO:Refactor this dup code
            if (mypod.Allocations != null)
            {
                var itmcnt = 1;
                foreach (
                    var allo in (mypod.Allocations as List<AsycudaSalesAllocations>)) //.Distinct()
                {
                    itm.xBondAllocations.Add(new xBondAllocations(true)
                    {
                        AllocationId = allo.AllocationId,
                        xcuda_Item = itm,
                        TrackingState = TrackingState.Added
                    });

                    itmcnt = AddFreeText(itmcnt, itm, allo.EntryDataDetails.EntryDataId,
                        allo.EntryDataDetails.LineNumber.GetValueOrDefault(), allo.EntryDataDetails.Comment,
                        mypod.EntlnData.ItemNumber);
                }
            }
        }

        private bool IsPiWeightLessthanMinimum(DataSpace.BaseDataModel.MyPodData mypod, ref string sql, ref int itmsCreated, xcuda_PreviousItem pitm)
        {
            if (Math.Round(pitm.Net_weight, 2) < (decimal)0.01)
            {
                UpdateXStatus(mypod.Allocations,
                    $@"Failed PiNetWeight < 0.01 :: PiNetWeight:{Math.Round(pitm.Net_weight, 2)}", ref sql);
                {
                    itmsCreated = 0;
                    return true;
                }
            }

            return false;
        }

        private void Ex9Bucket(DataSpace.BaseDataModel.MyPodData mypod, double availibleQty, double totalSalesAll,
            double totalPiAll,
            string type, ref string sql)
        {
            try
            {
                var totalallocations = mypod.Allocations.Count();
                var rejects = new List<AsycudaSalesAllocations>();
                for (int i = totalallocations; i <= totalallocations; i--)
                {
                    var remainingSalesQty = mypod.Allocations.Take(i).Sum(x => x.QtyAllocated - (x.PIData.Sum(z => z.xQuantity) ?? 0));
                    
                    //if (remainingSalesQty > availibleQty && totalallocations > 1)
                    //{
                    if (i <= 0  && remainingSalesQty <= 0)//&& rejects.Any()
                    {
                         UpdateXStatus(rejects,
                            $@"Failed All Sales Check:: {type} Sales:{Math.Round(totalSalesAll, 2)}
                                            {type} PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}", ref sql);
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated); 
                        return;
                    }
                    var ssa = mypod.Allocations.ElementAt(i-1);
                    var piData = ssa.PIData.Sum(x => x.xQuantity) ?? 0;
                    var nr = mypod.Allocations.Take(i).Sum(x => x.QtyAllocated - (x.PIData.Sum(z => z.xQuantity) ?? 0));

                    if (nr < availibleQty && (nr + (ssa.QtyAllocated- piData)) >= availibleQty)
                    {
                        ssa.QtyAllocated = availibleQty - nr;
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
                        break;//put back break because its finished reducing allocations and need to exit --- gonna bug somewhere can't remember
                    }
                    if (nr >= availibleQty)
                    {
                        if(mypod.Allocations.Count == 1) ssa.QtyAllocated = availibleQty; //renable with condition to fix [TestCase("7/1/2020", "7/31/2020", "MRL/JB0057F", 1, 1, 2)]// overexwarehousing --- //this increased the qty because its referenced in next line below.
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated); ;
                        break;
                    }
                    else
                    {
                        mypod.Allocations.RemoveAt(i-1);
                        rejects.Add(ssa);
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
                    }
                    //}
                    //else
                    //{

                    //}
                }

                UpdateXStatus( rejects,
                    $@"Failed All Sales Check:: {type} Sales:{Math.Round(totalSalesAll, 2)}
                                            {type} PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}", ref sql);
                mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private void UpdateXStatus(List<AsycudaSalesAllocations> allocations, string xstatus,ref string sql )
        {
            
            foreach (var a in allocations)
            {
                a.xStatus = xstatus;
                if (a.AllocationId == 0)
                {
                    sql +=
                        $@"Insert into AsycudaSalesAllocations(QtyAllocated, EntryDataDetailsId, PreviousItem_Id, EANumber, SANumber,Status, xStatus)
                                           Values ({a.QtyAllocated},{a.EntryDataDetailsId},{a.PreviousItem_Id},{a.EANumber},{a.SANumber},'{a.Status}','{a.xStatus.Replace("'", "''")}');\r\n";
                }
                else
                {
                    sql += $"Update AsycudaSalesAllocations Set xStatus = \'{xstatus.Replace("'", "''")}\' where allocationId = {a.AllocationId} ;\r\n";
                }
                
            }

        }

        private int AddFreeText(int itmcnt, xcuda_Item itm, string entryDataId, int lineNumber, string comment,
            string itemCode)
        {
            //if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true) return itmcnt;
            if (itm.Free_text_1 == null) itm.Free_text_1 = "";
            itm.Free_text_1 = $"{entryDataId}|{lineNumber}|{itemCode}";
            itm.PreviousInvoiceItemNumber = itemCode;
            itm.PreviousInvoiceLineNumber = lineNumber.ToString();
            itm.PreviousInvoiceNumber = entryDataId;
            if(!string.IsNullOrEmpty(comment)) itm.Free_text_2 = $"{comment}";
            itmcnt += 1;

            DataSpace.BaseDataModel.LimitFreeText(itm);
            return itmcnt;
        }

        private  void Ex9Bucket(DataSpace.BaseDataModel.MyPodData mypod, string dfp,  double docPi, List<ItemSalesPiSummary> itemSalesPiSummaryLst, ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems, ref string sql)
        {
            // prevent over draw down of pqty == quantity allocated
            try
            {
                var salesFactor = Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                    ? 1
                    : mypod.EntlnData.EX9Allocation.SalesFactor;
                var entryLine = mypod.EntlnData;
                var asycudaLine = mypod.EntlnData.pDocumentItem;
                var allocations = mypod.Allocations;
                
                if (asycudaLine == null) throw new ApplicationException("Allocation Previous Document is Null");
                //var itemAllocated = (dfp == "Duty Free" ? asycudaLine.DFQtyAllocated : asycudaLine.DPQtyAllocated);
                //var allocationsAllocated = allocations.Sum(x => x.QtyAllocated);
                // var totalSalesQtyAllocatedHistoric = salesSummary.Select(x => x.TotalQtyAllocated).DefaultIfEmpty(0).Sum() / salesFactor; // down to run levels
                var allocationSales = itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() >
                                      entryLine.EntryDataDetails.Sum(x => x.QtyAllocated)
                    ? itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() 
                    : entryLine.EntryDataDetails.Sum(x => x.QtyAllocated);/// Can't use just sales data because of partial exwarehouse the pi quantity will over ride it better to use total quantity '322035' columbian emeralds // itemSalesPiSummaryLst.First().Type == "Historic" ? itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() :  entryLine.EntryDataDetails.Sum(x => x.QtyAllocated); //itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum();//switch to QtyAllocated - 'CLC/124075' -- rouge 'WAM03600'

                var allocationPi = itemSalesPiSummaryLst.GroupBy(x => x.PreviousItem_Id).Select(x => x.First().PiQuantity).DefaultIfEmpty(0).Sum();
            
                var asycudaTotalQuantity = asycudaLine.ItemQuantity;//PdfpAllocated;//

                var alreadyTakenOutItemsLst = asycudaLine.previousItems.DistinctBy(x => x.PreviousItem_Id).ToList();
                if(docPreviousItems.ContainsKey(asycudaLine.xcuda_ItemId)) alreadyTakenOutItemsLst.AddRange(docPreviousItems[asycudaLine.xcuda_ItemId].Where(x => x.DutyFreePaid == dfp).ToList());
                
         
                var alreadyTakenOutTotalQuantity = alreadyTakenOutItemsLst.Sum(xx => xx.Suplementary_Quantity);

                var alreadyTakenOutDFPQuantity = alreadyTakenOutItemsLst.Where(x => x.DutyFreePaid == dfp).Sum(xx => xx.Suplementary_Quantity);

                var remainingQtyToBeTakenOut = Math.Round((double) (allocationSales - (allocationPi )) , 3);//+ docPi taking out docpi here because sales is not total sales 

                if ((Math.Abs(asycudaTotalQuantity - alreadyTakenOutTotalQuantity) < 0.01) 
                    //|| (Math.Abs(dutyFreePaidAllocated - alreadyTakenOutDFPQty) < 0.01)  //////////////Allow historical adjustment
                    || (Math.Abs(remainingQtyToBeTakenOut) <= 0)
                    || allocationSales < allocationPi)
                {
                    UpdateXStatus(mypod.Allocations,
                        $@"Failed Ex9 Bucket :: PI Quantity: {allocationPi}", ref sql);
                    allocations.Clear();
                    entryLine.EntryDataDetails.Clear();
                    entryLine.Quantity = 0;
                    return ;
                }
                //if (dutyFreePaidAllocated - (alreadyTakenOutDFPQty + entryLine.Quantity) < 0)
                //{
                
                   
                    
                if (remainingQtyToBeTakenOut + alreadyTakenOutTotalQuantity + docPi>= asycudaTotalQuantity) remainingQtyToBeTakenOut = asycudaTotalQuantity - alreadyTakenOutTotalQuantity - docPi;
                var salesLst = entryLine.EntryDataDetails.OrderBy(x => x.EntryDataDate).ThenBy(x => x.EntryDataDetailsId).ToList();

                var totalAllocatedQty = allocations.Sum(x => x.QtyAllocated) / salesFactor;
                   
                if (remainingQtyToBeTakenOut == totalAllocatedQty) return;
                if (entryLine.Quantity <= 0.001) return;
                if (Math.Abs(remainingQtyToBeTakenOut) < 0.001) return;
                if (salesLst.Any() == false) return;


                var startAllocationItemIndex = 0;
                var currentSalesItemIndex = 0;
                var saleItm = salesLst.ElementAt(currentSalesItemIndex);



                for (var s = currentSalesItemIndex; s < salesLst.Count(); s++)
                {
                    var currentAllocationItemIndex = startAllocationItemIndex;
                    if (currentSalesItemIndex != s)
                    {
                        currentSalesItemIndex = s;
                        saleItm = salesLst.ElementAt(currentSalesItemIndex);
                    }



                    if (saleItm == null) break;
                    var saleAllocationsLst = allocations
                        .Where(x => x.EntryDataDetailsId == saleItm.EntryDataDetailsId)
                        .OrderBy(x => x.AllocationId).ToList();

                    if (!saleAllocationsLst.Any()) break;

                    var allocation = saleAllocationsLst.ElementAt(currentAllocationItemIndex);

                    for (var i = currentAllocationItemIndex; i < saleAllocationsLst.Count(); i++)
                    {

                        if (currentAllocationItemIndex != i ||
                            saleAllocationsLst.ElementAt(currentAllocationItemIndex).AllocationId !=
                            allocation.AllocationId)
                        {
                            if (i < 0) i = 0;
                            currentAllocationItemIndex = i;
                            allocation = saleAllocationsLst.ElementAt(currentAllocationItemIndex);

                        }

                        var piData = allocation.PIData.Sum(x => x.xQuantity) ?? 0;
                        //var takeOut = piData;//CalculateTakeOut(totalAllocatedQty, remainingQtyToBeTakenOut, allocation.QtyAllocated, piData,  salesFactor);

                        totalAllocatedQty -= piData;
                        entryLine.Quantity -= piData;
                        allocation.QtyAllocated -= piData * salesFactor;
                        saleItm.QtyAllocated -= piData * salesFactor;


                        if (Math.Abs(allocation.QtyAllocated) < 0.001) //&& saleAllocationsLst.Count > 1
                        {
                             UpdateXStatus(new List<AsycudaSalesAllocations>() {allocation},
                                $@"Failed Ex9 Bucket",ref sql);
                            allocations.Remove(allocation);

                            if (totalAllocatedQty < 0) continue;
                        }
                        else
                        {
                            if (piData > 0)
                            {
                                UpdatePIData(piData, salesFactor, allocation, sql);
                            }
                        }


                    }

                    if (Math.Abs(saleItm.QtyAllocated) < 0.001)
                    {
                        entryLine.EntryDataDetails.Remove(saleItm);
                        // salesLst.RemoveAt(0);
                    }


                }


                // entryLine.Quantity = remainingQtyToBeTakenOut;
                   
                //}
                //if (entryLine.Quantity + alreadyTakenOutTotalQuantity > asycudaTotalQuantity) entryLine.Quantity = asycudaTotalQuantity - alreadyTakenOutTotalQuantity;


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private  void UpdatePIData(double piData, double salesFactor, AsycudaSalesAllocations allocation,string sql)
        {
            // Create New allocation
            if (allocation.AllocationId == 0)
            {
                sql +=
                    $@"Insert into AsycudaSalesAllocations(QtyAllocated, EntryDataDetailsId, PreviousItem_Id, EANumber, SANumber)
                                           Values ({piData * salesFactor},{allocation.EntryDataDetailsId},{allocation.PreviousItem_Id},{allocation.EANumber + 1},{allocation.SANumber + 1})";
            }
            else
                // update existing allocation
            {
                sql += $@" UPDATE       AsycudaSalesAllocations
                                                            SET                QtyAllocated =  QtyAllocated{(piData >= 0 ? $"-{piData * salesFactor}" : $"+{piData * -1 * salesFactor}")}
                                                            where	AllocationId = {allocation.AllocationId} ; \r\n";
            }
        }

        private xcuda_PreviousItem CreatePreviousItem(AlloEntryLineData pod, int itmcount,
            ConcurrentDictionary<int, List<PreviousItems>> docPreviousItems)
        {

            try
            {
                var previousItem = pod.pDocumentItem;

                var pitm = new global::DocumentItemDS.Business.Entities.xcuda_PreviousItem(true) { TrackingState = TrackingState.Added };
                if (previousItem == null) return pitm;

                

                pitm.Hs_code = pod.EX9Allocation.pTariffCode;
                pitm.Commodity_code = pod.EX9Allocation.pPrecision1;
                pitm.Current_item_number = (itmcount + 1); // piggy back the previous item count
                pitm.Previous_item_number = previousItem.LineNumber;


                SetWeights(pod, pitm, docPreviousItems);


                pitm.Previous_Packages_number = "0";


                pitm.Suplementary_Quantity = Math.Round(Convert.ToDecimal(pod.Quantity), 2);
                pitm.Preveious_suplementary_quantity = Convert.ToDouble(pod.EX9Allocation.pQuantity);


                pitm.Goods_origin = pod.EX9Allocation.Country_of_origin_code;
                double pval = pod.EX9Allocation.Total_CIF_itm;//previousItem.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency;
                pitm.Previous_value = Convert.ToDouble((pval / pod.EX9Allocation.pQuantity));
                pitm.Current_value = Convert.ToDouble((pval) / Convert.ToDouble(pod.EX9Allocation.pQuantity));
                pitm.Prev_reg_ser = "C";
                pitm.Prev_reg_nbr = pod.EX9Allocation.pCNumber;
                pitm.Prev_reg_year = pod.EX9Allocation.pRegistrationDate.Year;
                pitm.Prev_reg_cuo = pod.EX9Allocation.Customs_clearance_office_code;
                pitm.Prev_decl_HS_spec = pod.pDocumentItem.ItemNumber;

                return pitm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetWeights(AlloEntryLineData pod, xcuda_PreviousItem pitm,
            ConcurrentDictionary<int, List<PreviousItems>> previousItems)
        {
            try
            {
                var previousItem = pod.pDocumentItem;
                var docSetPreviousItems = new ConcurrentBag<PreviousItems>( previousItems.Keys.Contains(pod.PreviousDocumentItemId)? previousItems[pod.PreviousDocumentItemId].ToList() : new List<PreviousItems>());
                if (previousItem == null) return;
                var plst = previousItem.previousItems.DistinctBy(x => x.PreviousItem_Id);
                var pw = Convert.ToDecimal(Math.Round(pod.EX9Allocation.Net_weight_itm,2));

                var rw = (decimal)(plst.ToList().Sum(x => Math.Round(x.Net_weight, 2)) +
                                   docSetPreviousItems.Sum(x => x.Net_weight));

                var ra = plst.Sum(x => x.Suplementary_Quantity) + docSetPreviousItems.Sum(x => x.Suplementary_Quantity);

                var iw = (decimal)(pod.EX9Allocation.pQuantity - ra) == (decimal)0.0 ? 0 :  ((pw - (decimal) rw) / (decimal) (pod.EX9Allocation.pQuantity - ra)) * Convert.ToDecimal(pod.Quantity);

                //var iw = Convert.ToDouble((Math.Round(pod.EX9Allocation.Net_weight_itm,2)
                //                           / pod.EX9Allocation.pQuantity) * Convert.ToDouble(pod.Quantity));




                if ((pod.EX9Allocation.pQuantity - (plst.Sum(x => x.Suplementary_Quantity) + pod.Quantity + docSetPreviousItems.Sum(x => x.Suplementary_Quantity)))  <= 0 && pod.EX9Allocation.pQuantity > 1)
                {

                    pitm.Net_weight = Math.Round(Convert.ToDecimal(pw - rw), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    pitm.Net_weight = Convert.ToDecimal(Math.Truncate(iw * 100) / 100);
                }

                pitm.Prev_net_weight = pw; //Convert.ToDouble((pw/pod.EX9Allocation.SalesFactor) - rw);
                if (pitm.Net_weight > pitm.Prev_net_weight) pitm.Net_weight = pitm.Prev_net_weight;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Ex9InitializeCdoc(string dfp, DocumentCT cdoc, AsycudaDocumentSet ads, string DocumentType,
            DateTime? effectiveAssessmentDate, List<DocumentCT> docList, string prefix = null)
        {
            try
            {

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                DataSpace.BaseDataModel.Instance.IntCdoc(cdoc, ads, prefix);
                var customsProcedure = DataSpace.BaseDataModel.GetCustomsProcedure(dfp, DocumentType);

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = $"{DocumentType } {dfp} Entries";

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = false;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = false;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = false;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Cancelled = false;


                DataSpace.BaseDataModel.Instance.AttachCustomProcedure(cdoc, customsProcedure);
                AllocationsModel.Instance.AddDutyFreePaidtoRef(cdoc, dfp, ads, effectiveAssessmentDate, docList);
                


                ExportTemplate Exp = DataSpace.BaseDataModel.Instance.ExportTemplates
                    .Where(x => x.ApplicationSettingsId ==
                                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                    .ApplicationSettingsId)
                    .First(x =>
                        x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                            .CustomsProcedure);
               

                cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination.Destination_country_code =
                    Exp.Destination_country_code;
                cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_region =
                    Exp.Trading_country;
                //if(string.IsNullOrEmpty(ads.Currency_Code)) --- only use template
                cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = Exp.Gs_Invoice_Currency_code;
                if (string.IsNullOrEmpty(ads.Country_of_origin_code))
                {
                    cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = Exp.Trading_country;
                    cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination = Exp.Country_first_destination;

                    cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code = Exp.Export_country_code;
                }

                cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_code = Exp.Exporter_code;
                cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name = Exp.Exporter_name;
                cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_name = Exp.Consignee_name;
                cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_code = Exp.Consignee_code;



                //cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = Convert.ToSingle(ads.Exchange_Rate);

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}