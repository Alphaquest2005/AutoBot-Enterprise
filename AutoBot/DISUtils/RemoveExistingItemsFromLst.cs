using System.Collections.Generic;
using EntryDataDS.Business.Entities; // Assuming AsycudaDocumentItemEntryDataDetails is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static void RemoveExistingItemsFromLst(List<KeyValuePair<int, string>> lst, List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> itemEntryDataDetails)
        {
            foreach (var item in itemEntryDataDetails)
            {
                lst.Remove(item.key);
            }
        }
    }
}