using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class ProcessorPipline<T> : IProcessor<T>
    {
        private readonly List<IProcessor<T>> _processors;
 
        public ProcessorPipline(List<IProcessor<T>> processors)
        {
            _processors = processors;
        }
 
        public async Task<Result<List<T>>> Execute(List<T> data, ILogger log) => await _processors.Aggregate((o, n) => ChainedProcessorConstruction.Then(o, n)).Execute(data).ConfigureAwait(false);
    }
    
}