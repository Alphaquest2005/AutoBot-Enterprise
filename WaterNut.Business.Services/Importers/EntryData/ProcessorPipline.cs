using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class ProcessorPipline<T> : IProcessor<T>
    {
        private readonly List<IProcessor<T>> _processors;

        public ProcessorPipline(List<IProcessor<T>> processors)
        {
            _processors = processors;
        }

        public Result<List<T>> Execute(List<T> data) => _processors.Aggregate((o, n) => ChainedProcessorConstruction.Then(o, n)).Execute(data);
    }
    
}