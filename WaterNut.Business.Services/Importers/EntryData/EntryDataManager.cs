using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class EntryDataManager
    {

        public static Dictionary<string, IDocumentProcessor> CSVDocumentProcessors(ImportSettings importSettings)
        {
            return new Dictionary<string, IDocumentProcessor>()
            {

                { FileTypeManager.EntryTypes.Po, new DocumentProcessorPipline(new List<IDocumentProcessor>()
                                                                    {
                                                                        new InventoryImporter(importSettings),
                                                                        new SupplierPipeline(),
                                                                        new SaveEntryDataFile(importSettings),
                                                                        new EntryDataProcessor(importSettings)
                                                                    })},
                { FileTypeManager.EntryTypes.Sales, new DocumentProcessorPipline(new List<IDocumentProcessor>()
                {
                    new InventoryImporter(importSettings),
                    new SupplierPipeline(),
                    new SaveEntryDataFile(importSettings),
                    new EntryDataProcessor(importSettings)
                })},
                { FileTypeManager.EntryTypes.Dis, new DocumentProcessorPipline(new List<IDocumentProcessor>()
                {
                    new InventoryImporter(importSettings),
                    new SupplierPipeline(),
                    new SaveEntryDataFile(importSettings),
                    new EntryDataProcessor(importSettings)
                })},
                { FileTypeManager.EntryTypes.Adj, new DocumentProcessorPipline(new List<IDocumentProcessor>()
                {
                    new InventoryImporter(importSettings),
                    new SupplierPipeline(),
                    new SaveEntryDataFile(importSettings),
                    new EntryDataProcessor(importSettings)
                })},
                { FileTypeManager.EntryTypes.xSales, new DocumentProcessorPipline(new List<IDocumentProcessor>()
                {
                    new xsalesProcessor(importSettings)
                })},

            };
        }



    }
}
