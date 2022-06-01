using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class InventoryImporter : IDocumentProcessor
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;
        private readonly bool _overWrite;
        private InventoryProcessorPipline _importer;

        public InventoryImporter(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite)
        {
            _fileType = fileType;
            _docSet = docSet;
            _overWrite = overWrite; 
            
          
        }

        public List<dynamic> Execute (List<dynamic> lines)
        {
            _importer = new InventoryProcessorPipline(new List<IInventoryProcessor>()
            {
                new GetInventoryItems(_fileType, _docSet, lines),
                new MergedInventoryProcessor
                (new InventoryProcessorPipline (new List<IInventoryProcessor>()
                    {
                        new GetExistingInventoryItems(),
                        new UpdateItemTariffCode(),
                        new UpdateItemDescription(),
                        new UpdateLineDescription(),
                        new AddInventorySource(),
                        new SaveInventoryItems()
                    }),
                    new InventoryProcessorPipline(new List<IInventoryProcessor>()
                    {
                        new CreateInventoryItems(),
                        new SaveInventoryItems(),
                    })
                ),
                new UpdateLineNumbers(),
                new MergedInventoryProcessor
                (new InventoryProcessorPipline (new List<IInventoryProcessor>()
                    {
                        new GetItemAlias(),
                        new SaveItemAlias(),

                    }),
                    new InventoryProcessorPipline(new List<IInventoryProcessor>()
                    {
                        new GetItemCode(),
                        new SaveItemCode(),
                    })
                ),


            });

            _importer.Execute(new List<InventoryDataItem>());
            return lines;

          
        }
    }

    public class GetExistingInventoryItems : IInventoryProcessor
    {
        public List<InventoryDataItem> Execute(List<InventoryDataItem> lines)
        {
            throw new System.NotImplementedException();
        }
    }
}