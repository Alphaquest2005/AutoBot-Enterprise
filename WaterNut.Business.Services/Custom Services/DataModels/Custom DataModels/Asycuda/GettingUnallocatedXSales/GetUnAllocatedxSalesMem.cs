using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingUnallocatedXSales
{
    public class GetUnAllocatedxSalesMem : IGetUnAllocatedxSalesProcessor
    {
        private static List<PreAllocations> _preAllocations = null;
        static readonly object Identity = new object();

        public GetUnAllocatedxSalesMem()
        {
            lock (Identity)
            {
                if (_preAllocations == null)
                    using (var ctx = new AllocationDSContext() { StartTracking = false })
                    {
                        _preAllocations = ctx.XSales_UnAllocated
                            .AsNoTracking()
                            .Where(x => x.Date >=
                                        (WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.AllocationsOpeningStockDate
                                         ?? WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                            .ToList()
                            .Select(x => new PreAllocations()
                            {
                                EntryDataDetailsId = x.EntryDataDetailsId, PItemId = x.pItemId, XItemId = x.xItemId,
                                QtyAllocated = x.SalesQuantity ?? 0.0, DutyFreePaid = x.DutyFreePaid,
                                InventoryItemId = x.InventoryItemId, SalesQuantity = x.SalesQuantity ?? 0
                            })
                            .ToList();
                    }
            }

        }

        public List<PreAllocations> Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {

            return _preAllocations
                .Join(itemSetLst.Select(z => z.InventoryItemId).ToList(), x => x.InventoryItemId, i => i, (x, i) => x)
                .ToList();

        }
    }
}