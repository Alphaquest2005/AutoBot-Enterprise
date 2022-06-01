using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using InventoryDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetInventoryItems : IInventoryProcessor
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

        public List<InventoryDataItem> Execute(List<InventoryDataItem> lines)
        {
            var data = InventoryItemDataUtils.CreateItemGroupList(_lines);
            var inventorySource = InventoryItemUtils.GetInventorySource(_fileType);
            int applicationSettingsId = _docSet.First().ApplicationSettingsId;
            var inventoryItems =
                InventoryItemUtils.GetInventoryItems(data.Select(x => (string)x.Key.ItemNumber).ToList(), applicationSettingsId);


            var validItems = data.Where(x => !string.IsNullOrEmpty(x.Key.ItemDescription)).ToList();
            var existingInventoryItem = InventoryItemDataUtils.CreateExistingInventoryData(inventorySource, validItems, inventoryItems);

            var newInventoryItems = InventoryItemDataUtils.CreateNewInventoryData(inventorySource, validItems, inventoryItems, applicationSettingsId);

            return existingInventoryItem.Union(newInventoryItems).ToList();

        }
    }

    public interface IInventoryProcessor
    {
        List<InventoryDataItem> Execute(List<InventoryDataItem> lines);
    }
}