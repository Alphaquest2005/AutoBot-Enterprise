﻿using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class AddInventorySource : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;

        public AddInventorySource(FileTypes fileType)
        {
            _fileType = fileType;
            
        }

        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);

            var inventoryDataItems = data
                .Where(i => i.Item.InventoryItemSources.All(x => x.InventorySourceId != inventorySource.Id))
                .Select(x =>
                {x.Item.InventoryItemSources.Add(
                        InventorySourceProcessor.CreateItemSource(inventorySource, x.Item));
                    return x;
                })
                .ToList();
            return new Result<List<InventoryDataItem>>(inventoryDataItems,true,"");
        }
    }
}