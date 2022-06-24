using System.Collections.Generic;
using Core.Common.Extensions;

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

        public Result<List<T>> Execute(List<T> data)
        {
            var firstResult = _first.Execute(data);
            var results = firstResult.IsSuccess && _second == null ? firstResult : _second.Execute(firstResult.Value);
            return results;
        }
    }
}