using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static void SendDiscrepancyEmail(List<TODO_SubmitDiscrepanciesToCustoms> emailIds, IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data, string[] contacts, string body, List<string> pdfs)
        {
            // Potential InvalidOperationException if emailIds is empty
            // Potential ArgumentOutOfRangeException if ReferenceNumber doesn't contain '-'
            string reference = emailIds.First().ReferenceNumber.Substring(0, emailIds.First().ReferenceNumber.IndexOf('-'));

            if (data.Key == null)
            {
                // Assuming Utils.Client exists
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Assessed Shipping Discrepancy Entries: {reference}",
                    contacts, body, pdfs.ToArray());
            }
            else
            {
                // Assuming Utils.Client exists
                EmailDownloader.EmailDownloader.ForwardMsg(data.Key, Utils.Client,
                    $"Assessed Shipping Discrepancy Entries: {reference}", body, contacts, pdfs.ToArray());
            }
        }
    }
}