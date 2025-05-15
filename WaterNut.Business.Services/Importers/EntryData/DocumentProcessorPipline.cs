using System.Collections.Generic;
using System.Linq;
using java.util.concurrent;
using MoreLinq;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class DocumentProcessorPipline : IDocumentProcessor
    {
        private readonly List<IDocumentProcessor> _processors;
 
        public DocumentProcessorPipline(List<IDocumentProcessor> processors)
        {
            _processors = processors;
        }
 
        public async Task<List<dynamic>> Execute(List<dynamic> lines, ILogger log) => await _processors.Aggregate( (o,n)=> o.Then(n)).Execute(lines, log).ConfigureAwait(false);
    }
}