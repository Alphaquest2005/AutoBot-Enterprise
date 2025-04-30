using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class InventoryImporter : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;
   
        private ProcessorPipline<InventoryDataItem> _importer;

        public InventoryImporter(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            try
            {

           

            _importer = new ProcessorPipline<InventoryDataItem>(new List<IProcessor<InventoryDataItem>>()
            {
                new GetInventoryItems(_importSettings.FileType, _importSettings.DocSet, lines),
                new MergedProcessor<InventoryDataItem>
                (
                    new ProcessorPipline<InventoryDataItem>(new List<IProcessor<InventoryDataItem>>()
                    {

                        new GetExistingInventoryItems(_importSettings.FileType, _importSettings.DocSet),
                        new UpdateItemTariffCode(),
                        new UpdateItemDescription(),
                        new UpdateLineDescription(),
                        new AddInventorySource(_importSettings.FileType),
                        new SaveInventoryItems(),
                        new UpdateLineInventoryItemId(),
                        new SaveInventoryCodes(_importSettings.FileType),
                        new SaveInventoryAlias(_importSettings.FileType),



                    }),
                    new ProcessorPipline<InventoryDataItem>(new List<IProcessor<InventoryDataItem>>()
                    {
                        new GetNewInventoryItems(_importSettings.FileType, _importSettings.DocSet),
                        new UpdateLineInventoryItemId(),
                        new SaveInventoryItems(),
                        new SaveInventoryCodes(_importSettings.FileType),
                        new SaveInventoryAlias(_importSettings.FileType),
                    })
                ),
            });

            var res = _importer.Execute(new List<InventoryDataItem>());


            return lines;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


    
    }
}