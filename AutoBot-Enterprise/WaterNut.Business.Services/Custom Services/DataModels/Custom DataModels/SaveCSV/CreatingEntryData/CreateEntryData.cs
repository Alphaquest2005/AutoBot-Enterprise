using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryData : ICreateEntryDataProcessor
    {
        public  async Task Execute(DataFile dataFile, List<RawEntryData> goodLst)
        {
            Parallel.ForEach(goodLst, new ParallelLinqOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount},
                async item => // foreach (RawEntryData item in goodLst)
                {
                    try
                    {
                        var entryData = await new EntryDataCreator()
                            .GetSaveEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, item.Item)
                            .ConfigureAwait(false);

                        await new EntryDataDetailsCreator()
                            .CreateAndSaveEntryDataDetails(dataFile.DocSet, dataFile.OverWriteExisting, entryData)
                            .ConfigureAwait(false);

                        new InventoryItemsAliasProcessor().UpdateInventoryItems(item);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        DataSpace.BaseDataModel.EmailExceptionHandler(e);

                    }
                    
                });
        }
    }
}