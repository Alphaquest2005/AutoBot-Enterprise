using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class GetUnderAllocatedAsycudaItems : IGetUnderAllocatedAsycudaItemsProcessor
    {
        public List<int> Execute(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            List<int> IMAsycudaEntries;
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                var lst = ctx.xcuda_Inventory_Item.AsNoTracking()
                    .Join(itemList.Select(z => z.InventoryItemId).ToList(),
                        x => x.InventoryItemId,
                        i => i,
                        (x, i) => x.Item_Id)
                    .ToList();
                IMAsycudaEntries =
                    ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                        .Include(x => x.xcuda_Tarification.xcuda_HScode)
                        .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                        .Include(x => x.SubItems)
                        .Include(x => x.xcuda_Goods_description)
                        .Where(x => (x.DFQtyAllocated + x.DPQtyAllocated) < 0)
                        .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                    (x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Import
                                     || x.AsycudaDocument.Customs_Procedure.CustomsOperationId ==
                                     (int)CustomsOperations.Warehouse)
                                    && x.AsycudaDocument.Customs_Procedure.Sales == true
                                    && x.AsycudaDocument.DoNotAllocate != true)
                        .Where(x => x.AsycudaDocument.AssessmentDate >=
                                    (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                        .AsNoTracking()
                        .Join(lst, x => x.Item_Id, i => i, (x, i) => x.Item_Id)
                        .ToList();
            }

            return IMAsycudaEntries;
        }
    }
}