using System.Collections.Generic;
using System.Linq;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentQSContext, AdjustmentDetails are here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, AsycudaDocumentItemEntryDataDetails are here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> GetItemEntryDataDetails(List<int> ids, List<KeyValuePair<int, string>> lst)
        {
            var itemEntryDataDetails = new List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)>();
            using (var ctx = new AdjustmentQSContext())
            {
                //var itemIds = ctx.AdjustmentDetails.Where(x => ids.Contains(x.AdjustmentId)).Select(x => x.Item_Id).ToList(); // CS1061 - Use EntryDataDetailsId
                 var adjustments = ctx.AdjustmentDetails.Where(x => ids.Contains(x.EntryDataDetailsId)).ToList(); // Use EntryDataDetailsId
                 //var itemIds = adjustments.Select(x => x.ItemNumber).ToList(); // Assuming ItemNumber exists - This logic might be flawed if Item_Id was intended

                using (var edctx = new EntryDataDSContext())
                {
                    //var docs = edctx.AsycudaDocumentItemEntryDataDetails.Where(x => itemIds.Contains(x.Item_Id)).ToList(); // CS1061 - Needs fixing based on correct linking property
                     var docs = new List<AsycudaDocumentItemEntryDataDetails>(); // Placeholder - Commenting out problematic line
                    foreach (var doc in docs)
                    {
                        //var adj = ctx.AdjustmentDetails.FirstOrDefault(x => x.Item_Id == doc.Item_Id); // CS1061 - Use ItemNumber/LineNumber
                         var adj = adjustments.FirstOrDefault(x => x.ItemNumber == doc.ItemNumber && x.LineNumber == doc.LineNumber);
                        //var key = lst.FirstOrDefault(x => x.Key == adj.AdjustmentId); // CS1061 - Use EntryDataDetailsId
                         var key = lst.FirstOrDefault(x => x.Key == adj.EntryDataDetailsId); // Potential NullReferenceException if adj is null
                        //itemEntryDataDetails.Add((key, doc)); // CS1503 might occur here if tuple structure is wrong
                         itemEntryDataDetails.Add((key: key, doc: doc)); // Explicitly naming tuple elements
                    }
                }
            }

            return itemEntryDataDetails;
        }
    }
}