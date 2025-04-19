using System.Collections.Generic;
using AdjustmentQS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public interface IGetAllDiscrepancyDetailsProcessor
    {
        List<AdjustmentDetail> Execute(List<(string ItemNumber, int InventoryItemId)> itemList, bool overwriteExisting);
    }
}