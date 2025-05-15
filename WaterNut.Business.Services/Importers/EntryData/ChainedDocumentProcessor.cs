using System;
using System.Collections.Generic;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class ChainedDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentProcessor _first;
        private readonly IDocumentProcessor _second;
 
        public ChainedDocumentProcessor(IDocumentProcessor first, IDocumentProcessor second)
        {
            _first = first;
            _second = second;
        }
        
        public async Task<List<dynamic>> Execute(List<dynamic> list, ILogger log)
        {
            var firstResult = await _first.Execute(list).ConfigureAwait(false);
            return await _second.Execute(firstResult).ConfigureAwait(false);
        }
    }
 
    public static class ChainedDocumentProcessorConstruction
    {
        public static IDocumentProcessor Then(this IDocumentProcessor first, IDocumentProcessor next) =>
            new ChainedDocumentProcessor(first, next);
    }
}