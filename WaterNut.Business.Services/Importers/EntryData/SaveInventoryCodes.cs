using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveInventoryCodes : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;

        public SaveInventoryCodes(FileTypes fileType)
        {
            _fileType = fileType;
            
        }

        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
            data
                .Select(x => (DataItem: x, Code: InventoryCodesProcessor.GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => InventoryCodesProcessor.SaveInventoryCodes(inventorySource, x.Code, x.DataItem.Item));
            return new Result<List<InventoryDataItem>>(data, true, "");
        }
    }
}