using System;
using System.Collections.Generic;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
   
    public class ChainedDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentProcessor _first;
        private readonly IDocumentProcessor _second;

        public ChainedDocumentProcessor(IDocumentProcessor first, IDocumentProcessor second)
        {
            _first = first;
            _second = second;
        }
        
        public List<dynamic> Execute(List<dynamic> list)
        {
            return _second.Execute(_first.Execute(list));
        }
    }

    public static class ChainedDocumentProcessorConstruction
    {
        public static IDocumentProcessor Then(this IDocumentProcessor first, IDocumentProcessor next) =>
            new ChainedDocumentProcessor(first, next);
    }
}