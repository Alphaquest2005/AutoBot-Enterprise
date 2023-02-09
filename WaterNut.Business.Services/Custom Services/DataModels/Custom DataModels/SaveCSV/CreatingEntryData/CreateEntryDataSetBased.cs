using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryDataSetBased : ICreateEntryDataProcessor
    {
        public async Task Execute(DataFile dataFile, List<RawEntryData> goodLst)
        {



            try
            { 
                new InventoryItemsAliasProcessor().UpdateInventoryItems(goodLst);
                var entryDataLst = new EntryDataCreator()
                    .GetEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, goodLst);


                var existingEntryData = entryDataLst.Select(x => x.existingEntryData).Where(x => x.EntryData_Id > 0).ToList();
                var newEntryData = entryDataLst.Select(x => x.existingEntryData).Where(x => x.EntryData_Id == 0).ToList();

                new EntryDataDSContext().BulkMerge<EntryData>(existingEntryData, o => o.IncludeGraph = true);
                new EntryDataDSContext().BulkInsert<EntryData>(newEntryData, o => o.IncludeGraph = true);


                var entryDataWithDetailsLst = entryDataLst
                        .Select(entryData => new EntryDataDetailsCreator()
                        .CreateEntryDataDetails(entryData))
                        .SelectMany(x => x.ToList())
                        .ToList();
               
                var existingEntryDataDetails = entryDataWithDetailsLst.Where(x => x.EntryDataDetailsId > 0).ToList();
                var newEntryDataDetails = entryDataWithDetailsLst.Where(x => x.EntryDataDetailsId == 0).ToList();

                new EntryDataDSContext().BulkMerge(existingEntryDataDetails);
                new EntryDataDSContext().BulkInsert(newEntryDataDetails);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DataSpace.BaseDataModel.EmailExceptionHandler(e);

            }


        }
    }
}