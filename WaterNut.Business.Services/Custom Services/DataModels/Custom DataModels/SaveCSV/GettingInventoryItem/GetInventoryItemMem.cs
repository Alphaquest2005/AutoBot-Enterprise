using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities.Common;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem
{
    public class GetInventoryItemMem : IGetInventoryItemProcessor
    {
        private static ConcurrentDictionary<(int Id, string ItemNumber), InventoryItem> _inventoryItems = null;

        static readonly object Identity = new object();

        public GetInventoryItemMem()
        {
            lock (Identity)
            {
              var lst = new InventoryDSContext().InventoryItems
                  .Include("InventoryItemAlias")
                  .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                  .ToDictionary(x => (x.Id, x.ItemNumber), x => x);
              _inventoryItems = new ConcurrentDictionary<(int Id, string ItemNumber), InventoryItem>(lst);
            }
        }
        public InventoryItem Execute( string itemNumber) =>
            ////this list says updated by adding new items to db
            _inventoryItems.FirstOrDefault(x => x.Key.ItemNumber == itemNumber).Value 
            ?? AddInventoryItem(SaveInventoryItems(CreateInventoryItem(itemNumber)));

        private InventoryItem SaveInventoryItems(InventoryItem itm)
        {
            try
            {
                new InventoryDSContext().BulkMerge(new List<InventoryItem>() { itm });
                itm.AcceptChanges();
                return itm;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private InventoryItem CreateInventoryItem(string itemNumber) =>
            new InventoryItem()
            {
                ItemNumber = itemNumber,
                ApplicationSettingsId =
                    DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
            };

        private InventoryItem AddInventoryItem(InventoryItem itm)
        {
            _inventoryItems.AddOrUpdate((itm.Id, itm.ItemNumber), itm, (o, n) => n);
            return itm;
        }
    }
}