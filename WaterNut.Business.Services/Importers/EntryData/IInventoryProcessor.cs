using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public interface IProcessor<TData>
    {
        Task<Result<List<TData>>> Execute(List<TData> data, ILogger log);
    }
 
 
    
}