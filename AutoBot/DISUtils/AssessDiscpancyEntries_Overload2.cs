using System;
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void AssessDiscpancyEntries()
        {
            try
            {
                var info = BaseDataModel.CurrentSalesInfo(0); // Assuming CurrentSalesInfo exists
                // This calls AssessDISEntries, which needs to be in its own partial class
                AssessDISEntries(info.Item3.Declarant_Reference_Number); // Potential NullReferenceException if info or Item3 is null
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}