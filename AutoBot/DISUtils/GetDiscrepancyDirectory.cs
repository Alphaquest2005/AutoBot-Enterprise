using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here (for Tuple)

namespace AutoBot
{
    public partial class DISUtils
    {
        private static string GetDiscrepancyDirectory(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            // This calls POUtils.CurrentPOInfo, which needs to be accessible (likely moved to its own partial class)
            var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(
                POUtils.CurrentPOInfo(emailIds.First().AsycudaDocumentSetId)); // Potential InvalidOperationException if emailIds is empty
            var directory = info.Item2; // Potential NullReferenceException if info is null
            return directory;
        }
    }
}