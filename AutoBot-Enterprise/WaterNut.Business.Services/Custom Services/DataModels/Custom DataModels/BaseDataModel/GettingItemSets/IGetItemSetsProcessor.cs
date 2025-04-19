using System.Collections.Generic;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets
{
    public interface IGetItemSetsProcessor
    {
        List<List<(string ItemNumber, int InventoryItemId)>> Execute(string lst);
    }
}