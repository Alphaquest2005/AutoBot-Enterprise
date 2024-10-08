﻿using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class MergedDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentProcessor _first;
        private readonly IDocumentProcessor _second;

        public MergedDocumentProcessor(IDocumentProcessor first, IDocumentProcessor second)
        {
            _first = first;
            _second = second;
        }

        public List<dynamic> Execute(List<dynamic> list)
        {
            return _first.Execute(list).Union(_second.Execute(list)).ToList();
        }
    }

    public class MergedProcessor<T> : IProcessor<T>
    {
        private readonly IProcessor<T> _first;
        private readonly IProcessor<T> _second;

        public MergedProcessor(IProcessor<T> first, IProcessor<T> second)
        {
            _first = first;
            _second = second;
        }

        public Result<List<T>> Execute(List<T> data)
        {
            var secondResults = _second.Execute(data);
            var firstResults = _first.Execute(data);
            if (firstResults.IsSuccess && secondResults.IsSuccess) return new Result<List<T>>(firstResults.Value.Union(secondResults.Value).ToList(),true, "");
            if (firstResults.IsSuccess) return firstResults;
            if (secondResults.IsSuccess) return secondResults;
            return new Result<List<T>>(new List<T>(), false, $"{firstResults.Error} & {secondResults.Error}");
        }
    }
}