using OCR.Business.Entities;
using System.Text.RegularExpressions; // Added using directive
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class GetPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<GetPossibleInvoicesStep>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing GetPossibleInvoicesStep for File: {FilePath}", filePath);

            if (!ValidateContext(context, filePath))
                return false;

            try
            {
                string pdfTextString = context.PdfText.ToString();
                int totalTemplateCount = context.Templates.Count();
                _logger.Debug("Processing {TemplateCount} templates to find possible invoices for File: {FilePath}", totalTemplateCount, filePath);

                var possibleInvoices = GetPossibleInvoices(context.Templates, pdfTextString, filePath);

                context.Templates = possibleInvoices;
                LogPossibleInvoices(possibleInvoices, totalTemplateCount, filePath);

                _logger.Debug("Finished executing GetPossibleInvoicesStep successfully for File: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during GetPossibleInvoicesStep processing templates for File: {FilePath}", filePath);
                context.Templates = new List<Invoice>();
                return false;
            }
        }

        private bool ValidateContext(InvoiceProcessingContext context, string filePath)
        {
            if (context == null)
            {
                _logger.Error("GetPossibleInvoicesStep executed with null context.");
                return false;
            }

            if (context.Templates == null)
            {
                _logger.Warning("Skipping GetPossibleInvoicesStep: Templates collection is null for File: {FilePath}", filePath);
                context.Templates = new List<Invoice>();
                return true;
            }

            if (context.PdfText == null)
            {
                _logger.Warning("Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {FilePath}", filePath);
                context.Templates = new List<Invoice>();
                return false;
            }

            _logger.Information("Getting possible invoices for File: {FilePath}", filePath);
            return true;
        }

        private List<Invoice> GetPossibleInvoices(IEnumerable<Invoice> templates, string pdfTextString, string filePath)
        {
            _logger.Verbose("Ordering templates: Non-Tropical first (case-insensitive), then by Id.");

            return templates
                .OrderBy(x => !(x?.OcrInvoices?.Name?.ToUpperInvariant().Contains("TROPICAL") ?? false))
                .ThenBy(x => x?.OcrInvoices?.Id ?? int.MaxValue)
                .Where(tmp =>
                {
                    if (tmp == null)
                    {
                        _logger.Verbose("Skipping null template during filtering.");
                        return false;
                    }

                    if (tmp.OcrInvoices == null)
                    {
                        _logger.Verbose("Skipping template with null OcrInvoices. Template details: {@Template}", tmp);
                        return false;
                    }

                    bool isMatch = IsInvoiceDocument(tmp.OcrInvoices, pdfTextString, filePath);
                    _logger.Verbose("Template InvoiceId: {InvoiceId} IsMatch result: {IsMatch}", tmp.OcrInvoices.Id, isMatch);
                    return isMatch;
                })
                .ToList();
        }

        private void LogPossibleInvoices(List<Invoice> possibleInvoices, int totalTemplateCount, string filePath)
        {
            _logger.Information("Found {PossibleInvoiceCount} possible invoice(s) out of {TotalTemplateCount} templates for File: {FilePath}",
                possibleInvoices.Count, totalTemplateCount, filePath);

            if (possibleInvoices.Any())
            {
                var invoiceDetails = possibleInvoices.Select(inv => new { Name = inv.OcrInvoices?.Name, Id = inv.OcrInvoices?.Id }).ToList();
                _logger.Information("Details of possible invoices found for File: {FilePath}: {@InvoiceDetails}", filePath, invoiceDetails);
            }
            else
            {
                _logger.Information("No possible invoices found for File: {FilePath}.", filePath);
            }
        }
    }
 }