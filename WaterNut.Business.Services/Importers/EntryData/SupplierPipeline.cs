using System.Collections.Generic;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class SupplierPipeline : IDocumentProcessor
    {
        
        private ProcessorPipline<SupplierData> _importer;
 
       
 
        public async Task<List<dynamic>> Execute(List<dynamic> lines, ILogger log)
        {
            _importer = new ProcessorPipline<SupplierData>(new List<IProcessor<SupplierData>>()
            {
                new GetSupplierData(lines),
                new SaveNewSuppliers(),
 
            });
            
            await _importer.Execute(new List<SupplierData>()).ConfigureAwait(false);
            return lines;
        }
    }
}