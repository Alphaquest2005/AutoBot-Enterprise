using System.Collections.Generic;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentDetail is here
using CoreEntities.Business.Entities; // Assuming FileTypes is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<KeyValuePair<int, string>> GetDISList(FileTypes fileType)
        {
            // These call private methods which need to be in their own partial classes
            var alst = GetAdjustmentDetailsForFileType(fileType);
            var lst = GetDistinctKeyValuePairs(alst);
            return lst;
        }
    }
}