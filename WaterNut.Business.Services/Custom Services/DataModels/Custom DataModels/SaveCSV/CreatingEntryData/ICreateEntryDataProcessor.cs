using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public interface ICreateEntryDataProcessor
    {
        Task Execute(DataFile dataFile, List<RawEntryData> goodLst);
    }
}