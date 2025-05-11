using System.Collections.Generic;
using Core.Common.Extensions;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class ChainedProcessor<T> : IProcessor<T>
    {
        private readonly IProcessor<T> _first;
        private readonly IProcessor<T> _second;
 
        public ChainedProcessor(IProcessor<T> first, IProcessor<T> second)
        {
            _first = first;
            _second = second;
        }
 
        public async Task<Result<List<T>>> Execute(List<T> data)
        {
            var firstResult = await _first.Execute(data).ConfigureAwait(false);
            var results = firstResult.IsSuccess && _second == null ? firstResult : await _second.Execute(firstResult.Value).ConfigureAwait(false);
            return results;
        }
    }
}