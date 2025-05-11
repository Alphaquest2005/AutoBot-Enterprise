using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateLineNumbers : IProcessor<InventoryDataItem>
    {
        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data)
        {
            throw new System.NotImplementedException();
        }
    }
}