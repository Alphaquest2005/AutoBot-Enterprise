using System.Collections.Generic;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SupplierPipeline : IDocumentProcessor
    {
        
        private ProcessorPipline<SupplierData> _importer;

       

        public List<dynamic> Execute(List<dynamic> lines)
        {
            _importer = new ProcessorPipline<SupplierData>(new List<IProcessor<SupplierData>>()
            {
                new GetSupplierData(lines),
                new SaveNewSuppliers(),

            });
            
            _importer.Execute(new List<SupplierData>());
            return lines;
        }
    }
}