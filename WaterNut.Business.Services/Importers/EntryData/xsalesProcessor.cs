using System.Collections.Generic;
using Core.Common.Extensions;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class xsalesProcessor : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;

        public xsalesProcessor(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            var importer = new ProcessorPipline<BetterExpando>(new List<IProcessor<BetterExpando>>()
            {
                new GetRawXSalesData(_importSettings,lines),
                new SaveXSales(_importSettings)
            });
            importer.Execute(new List<BetterExpando>());
            return lines;
        }

       
    }
}