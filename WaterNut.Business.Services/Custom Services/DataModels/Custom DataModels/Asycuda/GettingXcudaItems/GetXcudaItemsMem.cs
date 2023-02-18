using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems
{
    public class GetXcudaItemsMem : IGetXcudaItemsProcessor
    {
        static readonly object Identity = new object();
        private static ConcurrentDictionary<int, xcuda_Item> _asycudaItems = null;

        public  GetXcudaItemsMem()
        {
            lock (Identity)
            {
                if (_asycudaItems != null) return;
                List<int> res;
                using (var ctx = new AllocationDSContext { StartTracking = false })
                {
                    ctx.Database.CommandTimeout = 0;
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    res = ctx.AsycudaSalesAllocations_XcudaItemsToAllocate.AsNoTracking()
                        .Where(x => x.ApplicationSettingsId ==
                                    WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AssessmentDate >=
                                    (WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                        .Select(x => x.Item_Id)
                        .Distinct()
                        .ToList();

                }

                using (var ctx = new AllocationDSContext { StartTracking = false })
                {
                    ctx.Database.CommandTimeout = 0;
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    var lst = ctx.xcuda_Item.AsNoTracking()
                        .WhereBulkContains(res, x => x.Item_Id)
                        .Include("AsycudaDocument.Customs_Procedure")
                        .Include("xcuda_Tarification.xcuda_HScode.xcuda_Inventory_Item")
                        //.Include("EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument")
                        .Include("xcuda_Tarification.xcuda_Supplementary_unit")
                        .Include("SubItems")
                        //.Include("EntryPreviousItems.xcuda_PreviousItem")
                        //.Include("EntryPreviousItems.PreviousItemEx")
                        .ToList();

                    var elst = ctx.EntryPreviousItems.AsNoTracking()
                        .WhereBulkContains(res, x => x.Item_Id)
                        .Include("xcuda_PreviousItem.xcuda_Item.AsycudaDocument")
                        .Include("PreviousItemsEx")
                        .ToList();

                    var aelst = lst.GroupJoin(elst, x => x.Item_Id, z => z.Item_Id,
                        (a, e) => new {a,e})
                        .Select(x =>
                        {
                             x.a.EntryPreviousItems = x.e.ToList();
                             return x.a;
                        }).ToList();

                    _asycudaItems = new ConcurrentDictionary<int, xcuda_Item>(aelst.ToDictionary(x => x.Item_Id, x => x));
                }

              
            }
        }

        public List<xcuda_Item> Execute(int? asycudaDocumentSetId, List<(string ItemNumber, int InventoryItemId)> itemslst)
            {
            var xlst = new GetXcudaInventoryItems().Execute(itemslst).Select(z => z.Item_Id).ToList();

            return _asycudaItems
                    .Join(xlst, a => a.Key, i => i, (a, i) => a)
                    .Where(x => asycudaDocumentSetId == null || x.Value.AsycudaDocument.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Select(x => x.Value)
                    .ToList();
            }
        }
    
}