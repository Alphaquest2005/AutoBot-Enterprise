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
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<GetPossibleInvoicesStep>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5); // Define a timeout for regex

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing GetPossibleInvoicesStep for File: {FilePath}", filePath);

            // Null checks for required context properties
            if (context == null)
            {
                 _logger.Error("GetPossibleInvoicesStep executed with null context.");
                 return false;
            }
            if (context.Templates == null)
            {
                 _logger.Warning("Skipping GetPossibleInvoicesStep: Templates collection is null for File: {FilePath}", filePath);
                 context.PossibleInvoices = new List<Invoice>(); // Ensure PossibleInvoices is initialized
                 return true; // Not necessarily an error, just no templates to check
            }
             if (context.PdfText == null)
            {
                 _logger.Warning("Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {FilePath}", filePath);
                 context.PossibleInvoices = new List<Invoice>(); // Ensure PossibleInvoices is initialized
                 return false; // Cannot proceed without text
            }

             _logger.Information("Getting possible invoices for File: {FilePath}", filePath);

            try
            {
                string pdfTextString = context.PdfText.ToString(); // Get text once
                int totalTemplateCount = context.Templates.Count();
                _logger.Debug("Processing {TemplateCount} templates to find possible invoices for File: {FilePath}", totalTemplateCount, filePath);

                 _logger.Verbose("Ordering templates: Non-Tropical first (case-insensitive), then by Id.");

                // Refined LINQ with better null handling and logging within Where
                var possibleInvoices = context.Templates
                    .OrderBy(x => !(x?.OcrInvoices?.Name?.ToUpperInvariant().Contains("TROPICAL") ?? false)) // Non-Tropical first, handle nulls
                    .ThenBy(x => x?.OcrInvoices?.Id ?? int.MaxValue) // Then by Id, handle nulls
                    .Where(tmp =>
                        {
                            // Perform checks and log within the Where clause for clarity
                            if (tmp == null) {
                                _logger.Verbose("Skipping null template during filtering.");
                                return false;
                            }
                            if (tmp.OcrInvoices == null) {
                                _logger.Verbose("Skipping template with null OcrInvoices. Template details: {@Template}", tmp); // Log template if possible
                                return false;
                            }
                            // Call IsInvoiceDocument, which now includes logging
                            bool isMatch = IsInvoiceDocument(tmp.OcrInvoices, pdfTextString, filePath);
                            // Log result of check for this specific template
                            _logger.Verbose("Template InvoiceId: {InvoiceId} IsMatch result: {IsMatch}", tmp.OcrInvoices.Id, isMatch);
                            return isMatch;
                        })
                    .ToList();

                 context.PossibleInvoices = possibleInvoices;
                 _logger.Information("Found {PossibleInvoiceCount} possible invoice(s) out of {TotalTemplateCount} templates for File: {FilePath}",
                     possibleInvoices.Count, totalTemplateCount, filePath);

                 // Log the names and IDs of the found possible invoices
                 if (possibleInvoices.Any())
                 {
                     var invoiceDetails = possibleInvoices.Select(inv => new { Name = inv.OcrInvoices?.Name, Id = inv.OcrInvoices?.Id }).ToList();
                     _logger.Information("Details of possible invoices found for File: {FilePath}: {@InvoiceDetails}", filePath, invoiceDetails);
                 }
                 else
                 {
                     _logger.Information("No possible invoices found for File: {FilePath}.", filePath);
                 }


                  _logger.Debug("Finished executing GetPossibleInvoicesStep successfully for File: {FilePath}", filePath);
                 return true; // Indicate success
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error during GetPossibleInvoicesStep processing templates for File: {FilePath}", filePath);
                  context.PossibleInvoices = new List<Invoice>(); // Ensure PossibleInvoices is initialized even on error
                  return false; // Indicate failure
             }
         }

         // Added filePath parameter for logging context
     }
 }