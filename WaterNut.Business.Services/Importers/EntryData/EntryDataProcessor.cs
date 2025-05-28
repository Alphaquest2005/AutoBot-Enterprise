using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class EntryDataProcessor : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;
 
 
       public EntryDataProcessor(ImportSettings importSettings)
         {
             _importSettings = importSettings;
         }
 
        public async Task<List<dynamic>> Execute(List<dynamic> lines, ILogger log)
        {
            var pipline = new ProcessorPipline<RawEntryData>(new List<IProcessor<RawEntryData>>()
            {
                new GetRawEntryData(_importSettings, lines),
                new FilterValidEntryData(),
                new SaveRawEntryData(_importSettings)
            });
            await pipline.Execute(new List<RawEntryData>(), log).ConfigureAwait(false);
            return lines;
 
        }
    }
}