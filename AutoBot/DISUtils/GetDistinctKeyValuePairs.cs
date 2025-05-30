using System.Collections.Generic;
using System.Linq;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentDetail is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<KeyValuePair<int, string>> GetDistinctKeyValuePairs(List<AdjustmentDetail> alst)
        {
            //var lst = alst.Select(x => new KeyValuePair<int, string>(x.AdjustmentId, x.CNumber)).Distinct().ToList(); // CS1061 - Use EntryDataDetailsId
             var lst = alst.Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.CNumber)).Distinct().ToList(); // Use EntryDataDetailsId
            return lst;
        }
    }
}