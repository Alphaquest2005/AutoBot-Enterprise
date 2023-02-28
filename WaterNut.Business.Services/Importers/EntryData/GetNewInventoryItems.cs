using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetNewInventoryItems : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;

        public GetNewInventoryItems(FileTypes fileType, List<AsycudaDocumentSet> docSet)
        {
            _fileType = fileType;
            _docSet = docSet;
        }

        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {

            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
            var newItems = data.Where(x => x.Item == null).Select(x => x.Data).ToList();
            var newInventoryItemFromData = InventoryItemDataUtils.GetNewInventoryItemFromData(newItems, inventorySource);
            return new Result<List<InventoryDataItem>>(newInventoryItemFromData, true, "") ;

        }
    }
}