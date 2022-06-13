using System;
using System.Collections.Generic;
using System.Linq;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class ValidateEntryDataProcessor
    {
        public static void ValidationChecks(DataFile dataFile)
        {
            if (dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Sales &&
                !(((IDictionary<string, object>)dataFile.Data.First()).ContainsKey("Tax") ||
                  ((IDictionary<string, object>)dataFile.Data.First()).ContainsKey("TotalTax")))
                throw new ApplicationException("Sales file dose not contain Tax");
        }
    }
}