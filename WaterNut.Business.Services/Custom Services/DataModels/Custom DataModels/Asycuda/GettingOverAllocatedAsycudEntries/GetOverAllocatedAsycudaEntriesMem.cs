using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class GetOverAllocatedAsycudaEntriesMem : IGetOverAllocatedAsycudaEntriesProcessor
    {
        private static ConcurrentDictionary<int , xcuda_Item> _imAsycudaEntries;
        static readonly object Identity = new object();

        public GetOverAllocatedAsycudaEntriesMem()
        {
            lock (Identity)
            {
                if (_imAsycudaEntries == null)
                {
                    using (var ctx = new AllocationDSContext { StartTracking = false })
                    {
                        ctx.Database.CommandTimeout = 0;
                        var lst = ctx.xcuda_Item
                            .Include(x => x.AsycudaDocument)
                            .Include(x => x.xcuda_Tarification.xcuda_HScode)
                            .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                            .Include(x => x.SubItems)
                            .Include(x => x.xcuda_Goods_description)
                            .Where(x => (x.DFQtyAllocated + x.DPQtyAllocated) > x.xcuda_Tarification.xcuda_Supplementary_unit
                                .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity)
                            .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                        (x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Import
                                         || x.AsycudaDocument.Customs_Procedure.CustomsOperationId ==
                                         (int)CustomsOperations.Warehouse)
                                        //&& x.AsycudaDocument.Customs_Procedure.Sales == true 
                                        && x.AsycudaDocument.DoNotAllocate != true)
                            .Where(x => x.AsycudaDocument.AssessmentDate >=
                                        (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                            .AsNoTracking()
                            
                            .ToDictionary(x=>x.Item_Id, x => x);
                        _imAsycudaEntries =
                            new ConcurrentDictionary<int, xcuda_Item>(lst);
                    }
                }
            }
        }

        public List<int> Execute(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            var lst = new GetXcudaInventoryItems().Execute(itemList).Select(x => x.Item_Id).ToList();
            return _imAsycudaEntries
                .Join(lst, x => x.Key, i => i, (x, i) => x)
                .Select(x => x.Value.Item_Id)
                .ToList();
        }
    }
}