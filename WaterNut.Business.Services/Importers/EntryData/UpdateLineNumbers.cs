using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class UpdateLineNumbers : IProcessor<InventoryDataItem>
    {
        public Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            throw new System.NotImplementedException();
        }
    }
}