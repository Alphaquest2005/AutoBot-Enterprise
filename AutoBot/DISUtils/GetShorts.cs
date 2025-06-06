using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static string GetShorts(List<KeyValuePair<int, string>> lst, FileTypes fileType)
        {
            // Aggregate might throw InvalidOperationException if lst is empty
            var strLst = lst.Any() ? lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o, n) => $"{o},{n}") : "";
            if (fileType.Data.Any())
            {
                var cnumberList = fileType.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();
                if (cnumberList.Any())
                {
                    strLst = string.Join(",",
                        lst.Where(x => cnumberList.Contains(x.Value)).Select(x => $"{x.Key.ToString()}-{x.Value}"));
                }
            }

            return strLst;
        }
    }
}