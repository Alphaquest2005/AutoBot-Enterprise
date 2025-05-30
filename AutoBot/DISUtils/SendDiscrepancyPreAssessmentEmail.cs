using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_TotalAdjustmentsToProcess, SubmitDiscrepanciesErrorReport, TODO_DiscrepancyPreExecutionReport are here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static bool SendDiscrepancyPreAssessmentEmail(List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<TODO_DiscrepancyPreExecutionReport> goodadj, List<string> pdfs)
        {
            // This calls GetContacts, which needs to be in its own partial class
            var contacts = GetContacts(new List<string>() { "Customs" });
            // This calls CreateDiscrepancyPreAssesmentEmailBody, which needs to be in its own partial class
            var body = CreateDiscrepancyPreAssesmentEmailBody(totaladjustments, errBreakdown, goodadj);
            // Assuming Utils.Client exists
            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                $"Discrepancy Pre-Assessment Report",
                contacts, body, pdfs.ToArray());
            return true;
        }
    }
}