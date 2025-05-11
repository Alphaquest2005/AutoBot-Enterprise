using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets;
using WaterNut.DataSpace;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using xcuda_Item = AdjustmentQS.Business.Entities.xcuda_Item;

namespace AdjustmentQS.Business.Services
{
    public class AutoMatchUtils
    {
        private static List<InventoryItemAliasX> _itemAliaCache;
        private readonly AutoMatchProcessor _autoMatchProcessor = new AutoMatchProcessor();


        private static List<InventoryItemAliasX> ItemAliasCache
        {
            get
            {
                if (_itemAliaCache != null) return _itemAliaCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemAliaCache = ctx.InventoryItemAliasX
                        .Include(x => x.InventoryItemsEx)
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                }

                return _itemAliaCache;
            }
        }

        public AutoMatchProcessor AutoMatchProcessor
        {
            get { return _autoMatchProcessor; }
        }


        public static void MatchToAsycudaDocument(AsycudaDocument asycudaDocument, EntryDataDetail ed)
        {
            if (asycudaDocument != null)
            {
                ed.EffectiveDate = asycudaDocument.AssessmentDate;
                ed.QtyAllocated = ed.Quantity;
               // ed.IsReconciled = true;
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    //PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = asycudaDocument.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                });
            }
            else
            {
                ed.Status = "No Asycuda Entry Found";
            }
        }

        public static Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
                                                                                         string sPreviousInvoiceNumber, string sEntryDataId)
        {
            try
            {

                // get document item in cnumber
                var aItem = Services.AutoMatchProcessor.AsycudaDocumentItemCache.Where( x => x.PreviousInvoiceNumber != null
                                                                   && x.AsycudaDocument.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation()
                                                                   && x.AsycudaDocument.ImportComplete == true
                                                                   && x.ApplicationSettingsId == applicationSettingsId &&
                                                                   (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                    || sEntryDataId.ToUpper().Trim() == x.PreviousInvoiceNumber.ToUpper().Trim()
                                                                   ));// contains is too vague
                var res = aItem.ToList();


                return Task.FromResult(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
                                                                                         string sPreviousInvoiceNumber, string sEntryDataId, string sItemNumber)
        {
            try
            {

                // get document item in cnumber
                var aItem = Enumerable.Where<AsycudaDocumentItem>(Services.AutoMatchProcessor.AsycudaDocumentItemCache, x => x.PreviousInvoiceNumber != null
                                                                                                   && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                                                   && x.AsycudaDocument.ImportComplete == true
                                                                                                   && x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim() &&
                                                                                                   (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                                                    || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())));
                var res = aItem.ToList();
                var alias = Enumerable.Where<InventoryItemAliasX>(ItemAliasCache, x => x.InventoryItemsEx.ApplicationSettingsId == applicationSettingsId &&
                                                                     x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim())
                    .Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return Task.FromResult(res);

                var ae = Enumerable.Where<AsycudaDocumentItem>(Services.AutoMatchProcessor.AsycudaDocumentItemCache, x => x.PreviousInvoiceNumber != null
                                                                                                && alias.Contains(x.ItemNumber)
                                                                                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                                                && x.AsycudaDocument.ImportComplete == true
                                                                                                && (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                                                    || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())))
                    .ToList();
                if (ae.Any()) res.AddRange(ae);



                return Task.FromResult(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static AsycudaDocument GetAsycudaDocumentInCNumber(int applicationSettingsId, string cNumber)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                try
                {


                    // get document item in cnumber
                    var clst = GetCNumbersFromString(cNumber);
                    var res = Queryable.FirstOrDefault(ctx.AsycudaDocuments
                            .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                            .Where(x => (x.CNumber != null || x.IsManuallyAssessed == true) &&
                                        (x.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse) &&
                                        // x.WarehouseError == null && 
                                        (x.Cancelled == null || x.Cancelled == false) &&
                                        x.DoNotAllocate != true)
                            .OrderByDescending(x => x.AssessmentDate), x => clst.Contains(x.CNumber));
                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumber(string cNumber,
                                                                                 int? previousCLineNumber, string itemNumber)
        {

            try
            {

                var clst = GetCNumbersFromString(cNumber);
                // get document item in cnumber
                var aItem = Enumerable.Where<AsycudaDocumentItem>(Services.AutoMatchProcessor.AsycudaDocumentItemCache, x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                                                                                   && x.AsycudaDocument.CNumber != null
                                                                                                   && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                                                   && x.AsycudaDocument.ImportComplete == true
                                                                                                   && x.ItemNumber == itemNumber && Enumerable.Any<string>(clst, z => z == x.AsycudaDocument.CNumber)
                                                                                                   && x.LineNumber == (previousCLineNumber == null ? x.LineNumber : previousCLineNumber.Value.ToString()));
                var res = aItem.ToList();
                var alias = GetAlias(itemNumber);

                var reverseAlias = GetReverseAlias(itemNumber);
                 alias.AddRange(reverseAlias);

                if (!alias.Any()) return Task.FromResult(res);

                var ae = Enumerable.Where<AsycudaDocumentItem>(Services.AutoMatchProcessor.AsycudaDocumentItemCache, x => x.AsycudaDocument.CNumber != null
                                                                                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                                                && x.AsycudaDocument.ImportComplete == true
                                                                                                && alias.Contains(x.ItemNumber) && cNumber.Contains(x.AsycudaDocument.CNumber)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return Task.FromResult(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }


        private static List<string> GetAlias(string itemNumber) => ItemAliasCache.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

        private static List<string> GetReverseAlias(string itemNumber)
        {
            return Enumerable
                .Where<InventoryItemAliasX>(ItemAliasCache, x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.AliasName.ToUpper().Trim() == itemNumber)
                .Select(y => y.ItemNumber.ToUpper().Trim()).ToList();
        }

        public static Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumberReference(int applicationSettingsId,
                                                                                          string cNumber, string itemNumber)
        {

            try
            {
                List<string> cnumberlst = GetCNumbersFromString(cNumber);
                var doc = Services.AutoMatchProcessor.AsycudaDocumentItemCache.FirstOrDefault(x => x.CNumber != null && cnumberlst.Contains(x.CNumber));
                
                if (doc == null) return Task.FromResult(new List<AsycudaDocumentItem>());
                var docref = doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal) > 0
                    ? doc.ReferenceNumber.Substring(0, doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal))
                    : doc.ReferenceNumber;


                var aItem = Services.AutoMatchProcessor.AsycudaDocumentItemCache
                    .Where(x => x.ItemNumber == itemNumber
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref));
                var res = aItem.ToList();
                var alias = GetAlias(itemNumber);

                var reverseAlias = GetReverseAlias(itemNumber);
                alias.AddRange(reverseAlias);


                if (!alias.Any()) return Task.FromResult(res);

                var ae = Services.AutoMatchProcessor.AsycudaDocumentItemCache
                    .Where(x => alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref)).ToList();

                if (ae.Any()) res.AddRange(ae);



                return Task.FromResult(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        

        private static List<string> GetCNumbersFromString(string cNumber)
        {
            var str = cNumber.ToUpper().Replace("C", "");
            var res = str.Split(',', ' ').ToList();
            return res;
        }

        public async Task AutoMatchItems(int applicationSettingsId, string strLst)
        {
            try
            {

                var lst = GetSelectedAdjustmentDetails(applicationSettingsId, strLst);

                await Services.AutoMatchProcessor.DoAutoMatch(applicationSettingsId, lst).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<AdjustmentDetail> GetSelectedAdjustmentDetails(int applicationSettingsId, string strLst)
        {
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;

                var lst = ctx.AdjustmentDetails
                    //.Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                && strLst.Contains(x.EntryDataDetailsId.ToString()))
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .OrderBy(x => x.EntryDataDetailsId)
                    .ToList();
                return lst;
            }
        }
    }
}