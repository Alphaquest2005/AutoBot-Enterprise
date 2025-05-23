using System.Collections.Generic;
using System.Linq;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentQSContext, AdjustmentDetail are here
using CoreEntities.Business.Entities; // Assuming FileTypes is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<AdjustmentDetail> GetAdjustmentDetailsForFileType(FileTypes fileType)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                var alst = ctx.AdjustmentDetails.Where(x =>
                        x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId && x.Status == null)
                    .ToList();
                return alst;
            }
        }
    }
}