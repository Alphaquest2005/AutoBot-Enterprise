﻿using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateLineNumbers : IProcessor<InventoryDataItem>
    {
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            throw new System.NotImplementedException();
        }
    }
}