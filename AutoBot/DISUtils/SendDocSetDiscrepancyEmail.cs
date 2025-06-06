using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, SubmitDiscrepanciesErrorReport, TODO_TotalAdjustmentsToProcess, TODO_DiscrepancyPreExecutionReport are here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static bool SendDocSetDiscrepancyEmail(FileTypes fileType, List<SubmitDiscrepanciesErrorReport> errors, List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<TODO_DiscrepancyPreExecutionReport> goodadj, List<string> pdfs)
        {
            // This calls GetContacts, which needs to be in its own partial class
            var contacts = GetContacts(new List<string>() { "Customs" });
            // This calls CreateDiscrepancyWithErrorsEmailBody, which needs to be in its own partial class
            var body = CreateDiscrepancyWithErrorsEmailBody(errors, totaladjustments, errBreakdown, goodadj);
            // Assuming Utils.Client exists
            EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId, Utils.Client,
                $"Discrepancy Pre-Assessment Report", body, contacts, pdfs.ToArray());
            return true;
        }
    }
}