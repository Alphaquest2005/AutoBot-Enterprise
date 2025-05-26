using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using Serilog;

namespace AutoBot
{
    public class ImportIssueDetector
    {
        private readonly ILogger _logger;

        public ImportIssueDetector(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImportIssueAnalysis> AnalyzeEmailImportAsync(string emailId, int applicationSettingsId)
        {
            _logger.Information("Starting import issue analysis for EmailId: {EmailId}", emailId);

            var analysis = new ImportIssueAnalysis
            {
                EmailId = emailId,
                ApplicationSettingsId = applicationSettingsId,
                AnalysisDate = DateTime.Now
            };

            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    // Get all shipment invoices for this email
                    var shipmentInvoices = ctx.ShipmentInvoice
                        .Include(x => x.InvoiceDetails)
                        .Where(x => x.EmailId == emailId)
                        .ToList();

                    analysis.TotalInvoices = shipmentInvoices.Count;

                    foreach (var invoice in shipmentInvoices)
                    {
                        var issueDetails = AnalyzeInvoiceIssues(invoice);
                        if (issueDetails.HasIssues)
                        {
                            analysis.InvoicesWithIssues.Add(issueDetails);
                        }
                    }

                    analysis.TotalZeroSum = shipmentInvoices.Sum(x => x.TotalsZero);
                    analysis.HasIssues = analysis.InvoicesWithIssues.Any();

                    _logger.Information("Analysis completed. Total invoices: {Total}, Issues found: {Issues}, TotalZeroSum: {Sum}",
                        analysis.TotalInvoices, analysis.InvoicesWithIssues.Count, analysis.TotalZeroSum);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during import issue analysis for EmailId: {EmailId}", emailId);
                analysis.ErrorMessage = ex.Message;
            }

            return analysis;
        }

        private InvoiceIssueDetails AnalyzeInvoiceIssues(ShipmentInvoice invoice)
        {
            var details = new InvoiceIssueDetails
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNo,
                TotalZeroValue = invoice.TotalsZero
            };

            if (Math.Abs(invoice.TotalsZero) < 0.01)
            {
                details.HasIssues = false;
                return details;
            }

            details.HasIssues = true;

            // Calculate detail level differences
            details.DetailLevelDifference = invoice.InvoiceDetails
                .Sum(detail => (detail.TotalCost ?? 0.0) - ((detail.Cost) * (detail.Quantity)));

            // Calculate header level differences
            double calculatedSubTotal = invoice.InvoiceDetails
                .Sum(detail => detail.TotalCost ?? ((detail.Cost) * (detail.Quantity)));

            details.HeaderLevelDifference = (calculatedSubTotal
                                           + (invoice.TotalInternalFreight ?? 0)
                                           + (invoice.TotalOtherCost ?? 0)
                                           + (invoice.TotalInsurance ?? 0)
                                           - (invoice.TotalDeduction ?? 0))
                                          - (invoice.InvoiceTotal ?? 0);

            // Determine issue types
            details.IssueTypes = DetermineIssueTypes(invoice, details);

            return details;
        }

        private List<string> DetermineIssueTypes(ShipmentInvoice invoice, InvoiceIssueDetails details)
        {
            var issues = new List<string>();

            if (Math.Abs(details.DetailLevelDifference) > 0.01)
            {
                issues.Add("DetailLevelMismatch");
            }

            if (Math.Abs(details.HeaderLevelDifference) > 0.01)
            {
                issues.Add("HeaderLevelMismatch");
            }

            if ((invoice.TotalOtherCost ?? 0) == 0 && details.HeaderLevelDifference < 0)
            {
                issues.Add("MissingShippingOrTax");
            }

            if ((invoice.TotalDeduction ?? 0) == 0 && details.HeaderLevelDifference > 0)
            {
                issues.Add("MissingDiscountOrCoupon");
            }

            if (invoice.InvoiceDetails.Any(d => d.TotalCost == null || d.TotalCost == 0))
            {
                issues.Add("MissingLineItemTotals");
            }

            return issues;
        }
    }

    public class ImportIssueAnalysis
    {
        public string EmailId { get; set; }
        public int ApplicationSettingsId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public int TotalInvoices { get; set; }
        public List<InvoiceIssueDetails> InvoicesWithIssues { get; set; } = new List<InvoiceIssueDetails>();
        public double TotalZeroSum { get; set; }
        public bool HasIssues { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class InvoiceIssueDetails
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public double TotalZeroValue { get; set; }
        public double DetailLevelDifference { get; set; }
        public double HeaderLevelDifference { get; set; }
        public bool HasIssues { get; set; }
        public List<string> IssueTypes { get; set; } = new List<string>();
    }
}
