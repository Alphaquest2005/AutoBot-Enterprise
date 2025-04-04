using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here
using MoreLinq; // For DistinctBy

namespace AutoBot
{
    // Assuming DiscpancyExecData is defined elsewhere or needs moving
    public class DiscpancyExecData
    {
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; } // Assuming DateTime? based on usage
        public string ItemNumber { get; set; }
        public double? InvoiceQty { get; set; } // Assuming double? based on usage
        public double? ReceivedQty { get; set; } // Assuming double? based on usage
        public string CNumber { get; set; }
        public string Status { get; set; }
        public string xCNumber { get; set; }
        public int? xLineNumber { get; set; } // Assuming int? based on usage
        public string xRegistrationDate { get; set; }
        public string EmailId { get; set; }
        public int ApplicationSettingsId { get; set; }
        public int AsycudaDocumentSetId { get; set; }
        public string DocumentType { get; set; }
        public string ReferenceNumber { get; set; }
        public string CustomsProcedure { get; set; }
        public int? ASYCUDA_Id { get; set; } // Assuming int? based on usage
    }

    public partial class DISUtils
    {
        private static void CreateDiscrepancyEmail(IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data, string[] contacts)
        {
            // These call private methods which need to be in their own partial classes
            var cNumbers = GetCNumbers(data);
            var directory = GetDiscrepancyDirectory(cNumbers);
            var executionFile = CreateExecutionFile(directory, data);

            var otherCNumbers = executionFile.execData
                .Where(x => !string.IsNullOrEmpty(x.xCNumber) && cNumbers.All(z => z.CNumber != x.xCNumber))
                .DistinctBy(x => x.CNumber)
                .Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                {
                    CNumber = x.xCNumber,
                    ApplicationSettingsId = x.ApplicationSettingsId,
                    AssessmentDate = DateTime.Parse(x.xRegistrationDate), // Potential FormatException
                    AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                    CustomsProcedure = x.CustomsProcedure,
                    DocumentType = x.DocumentType,
                    EmailId = x.EmailId,
                    ReferenceNumber = x.ReferenceNumber,
                    RegistrationDate = DateTime.Parse(x.xRegistrationDate), // Potential FormatException
                    Status = x.Status,
                    ToBePaid = "",
                    ASYCUDA_Id = x.ASYCUDA_Id.GetValueOrDefault()

                })
                .ToList();

            cNumbers.AddRange(otherCNumbers);
            // This calls a private method which needs to be in its own partial class
            var pdfs = AttachDiscrepancyPdfs(cNumbers);

            //if (pdfs.Count == 0) return;

            // This calls a private method which needs to be in its own partial class
            var body = CreateEmailBody(cNumbers);

            if (pdfs.Count == 0) body = body + $"\r\n Sorry no pdfs were downloaded for this discrepancy.";

            pdfs.Add(executionFile.executionFile);

            // This calls a private method which needs to be in its own partial class
            var summaryFile = CreateSummaryFile(cNumbers, directory);
            pdfs.Add(summaryFile);

            // This calls a private method which needs to be in its own partial class
            SendDiscrepancyEmail(cNumbers, data, contacts, body, pdfs);

            // This calls a private method which needs to be in its own partial class
            AttachDocumentsPerEmail(cNumbers);
        }
    }
}