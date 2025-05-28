using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterNut.DataSpace;
using Serilog;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryDataSelector : ICreateEntryDataProcessor
    {
        private bool isDBMem = false;

        public  async Task Execute(DataFile dataFile, List<RawEntryData> goodLst, ILogger log)
        {
            if (isDBMem)
               await new CreateEntryData().Execute(dataFile, goodLst, log).ConfigureAwait(false);
            else
                await new CreateEntryDataSetBased().Execute(dataFile, goodLst, log).ConfigureAwait(false);
        
        }
    }
}