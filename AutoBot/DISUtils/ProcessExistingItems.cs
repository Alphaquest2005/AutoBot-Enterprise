using System.Collections.Generic;
using System.Linq;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentQSContext, AdjustmentDetails are here
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using EntryDataDS.Business.Entities; // Assuming AsycudaDocumentItemEntryDataDetails is here
using TrackableEntities; // Assuming TrackingState is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static void ProcessExistingItems(FileTypes fileType, List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> itemEntryDataDetails)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                foreach (var item in itemEntryDataDetails)
                {
                    //var adj = ctx.AdjustmentDetails.FirstOrDefault(x => x.Item_Id == item.doc.Item_Id); // CS1061 - Use ItemNumber/LineNumber instead
                    var adj = ctx.AdjustmentDetails.FirstOrDefault(x => x.ItemNumber == item.doc.ItemNumber && x.LineNumber == item.doc.LineNumber);
                    if (adj != null)
                    {
                        adj.Status = "Imported";
                        adj.TrackingState = TrackingState.Modified;
                    }
                }

                //ctx.ApplyChanges(); // CS1061 - Use SaveChanges()
                 ctx.SaveChanges();
            }
        }
    }
}