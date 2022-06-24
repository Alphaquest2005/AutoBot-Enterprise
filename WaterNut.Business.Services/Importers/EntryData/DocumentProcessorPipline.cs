using System.Collections.Generic;
using System.Linq;
using java.util.concurrent;
using MoreLinq;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class DocumentProcessorPipline : IDocumentProcessor
    {
        private readonly List<IDocumentProcessor> _processors;

        public DocumentProcessorPipline(List<IDocumentProcessor> processors)
        {
            _processors = processors;
        }

        public List<dynamic> Execute(List<dynamic> lines) => _processors.Aggregate( (o,n)=> o.Then(n)).Execute(lines);
    }
}