using System.Collections.Generic;
using Core.Common.Extensions;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class xsalesProcessor : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;
 
        public xsalesProcessor(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }
 
        public async Task<List<dynamic>> Execute(List<dynamic> lines)
        {
            var importer = new ProcessorPipline<BetterExpando>(new List<IProcessor<BetterExpando>>()
            {
                new GetRawXSalesData(_importSettings,lines),
                new SaveXSales(_importSettings)
            });
            await importer.Execute(new List<BetterExpando>()).ConfigureAwait(false);
            return lines;
        }
 
       
    }
}