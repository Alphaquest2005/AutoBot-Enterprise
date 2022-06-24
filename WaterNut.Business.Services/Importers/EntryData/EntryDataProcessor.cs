using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class EntryDataProcessor : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;


       public EntryDataProcessor(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            var pipline = new ProcessorPipline<RawEntryData>(new List<IProcessor<RawEntryData>>()
            {
                new GetRawEntryData(_importSettings, lines),
                new FilterValidEntryData(),
                new SaveRawEntryData(_importSettings)
            });
            pipline.Execute(new List<RawEntryData>());
            return lines;

        }
    }
}