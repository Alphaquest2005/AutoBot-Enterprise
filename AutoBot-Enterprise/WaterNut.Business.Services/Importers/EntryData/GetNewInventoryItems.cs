using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added missing using
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

        // Revert signature to match IProcessor<InventoryDataItem> interface
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {

            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
            var newItems = data.Where(x => x.Item == null).Select(x => x.Data).ToList();
            // Synchronously wait for the async call result (potential blocking issue)
            var newInventoryItemFromData = InventoryItemDataUtils.GetNewInventoryItemFromData(newItems, inventorySource).Result;
            return new Result<List<InventoryDataItem>>(newInventoryItemFromData, true, "") ;

        }
    }
}