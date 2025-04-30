using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetExistingInventoryItems : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;

        public GetExistingInventoryItems(FileTypes fileType, List<AsycudaDocumentSet> docSet)
        {
            _fileType = fileType;
            _docSet = docSet;
        }

        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {

            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
            var existingItems = data.Where(x => x.Item != null).Select(x => x.Data).ToList();
            var existingInventoryItemFromData = InventoryItemDataUtils.GetExistingInventoryItemFromData(existingItems, inventorySource);
            var distinctlst = existingInventoryItemFromData.DistinctBy(x => x.Item.Id).ToList();
            return new Result<List<InventoryDataItem>>(distinctlst, true, "");

        }
    }
}