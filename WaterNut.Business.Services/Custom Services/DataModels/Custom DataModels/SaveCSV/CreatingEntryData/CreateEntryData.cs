using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;
using WaterNut.DataSpace;
using Serilog;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryData : ICreateEntryDataProcessor
    {
        public Task Execute(DataFile dataFile, List<RawEntryData> goodLst, ILogger log)
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
                        await DataSpace.BaseDataModel.EmailExceptionHandlerAsync(e, log, true).ConfigureAwait(false);

                    }
                    
                });
            return Task.CompletedTask;
        }
    }
}