using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using InventoryDS.Business.Entities;
using MoreLinq;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetInventoryItems : IProcessor<InventoryDataItem>
    {
        public readonly List<dynamic> _lines;
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;

        public GetInventoryItems(FileTypes fileType, List<AsycudaDocumentSet> docSet, List<dynamic> lines)
        {
            _lines = lines;
            _fileType = fileType;
            _docSet = docSet;
        }

        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data)
        {
            var rawData = InventoryItemDataUtils.CreateItemGroupList(_lines);
 
            var inventoryItems =
                InventoryItemUtils.GetInventoryItems(rawData.Select(x => (string)x.Key.ItemNumber).ToList());
 
            var left = rawData.GroupJoin(
                    inventoryItems,
                    r => r.Key.ItemNumber,
                    i => i.ItemNumber,
                    (r, i) => new InventoryDataItem(r, i.FirstOrDefault()));
 
            var right = inventoryItems.GroupJoin(
                rawData,
                i => i.ItemNumber,
                r => r.Key.ItemNumber,
                (i, r) => new InventoryDataItem(r.FirstOrDefault(), i));
 
            return Task.FromResult(new Result<List<InventoryDataItem>>(left.Union(right).ToList(), true, "")).Result; // Wrap in Task.FromResult
 
 
        }
    }
}