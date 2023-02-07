using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryDataSelector : ICreateEntryDataProcessor
    {
        private bool isDBMem = false;

        public  async Task Execute(DataFile dataFile, List<RawEntryData> goodLst)
        {
            if (isDBMem)
               await new CreateEntryData().Execute(dataFile, goodLst).ConfigureAwait(false);
            else
                await new CreateEntryDataSetBased().Execute(dataFile, goodLst).ConfigureAwait(false);

        }
    }
}