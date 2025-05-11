using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public interface ISaveInventoryItemsProcessor
    {
        Task Execute(List<InventoryDataItem> processedInventoryItems);
    }
}