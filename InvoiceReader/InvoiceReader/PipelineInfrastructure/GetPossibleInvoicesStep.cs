using OCR.Business.Entities;
using System.Text.RegularExpressions; // Added using directive
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class GetPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
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
        private static bool IsInvoiceDocument(Invoices invoice, string fileText, string filePath)
        {
             // Invoice null check happens in caller's Where clause
             int invoiceId = invoice.Id;
             _logger.Verbose("Checking if document matches Invoice Template using InvoiceId: {InvoiceId} for File: {FilePath}", invoiceId, filePath);

             // Check if InvoiceIdentificatonRegEx collection exists and has items
             if (invoice.InvoiceIdentificatonRegEx == null || !invoice.InvoiceIdentificatonRegEx.Any())
             {
                  _logger.Warning("No Invoice Identification Regex patterns found for InvoiceId: {InvoiceId}. Cannot determine if it's an invoice document based on regex.", invoiceId);
                  return false; // Cannot match without patterns
             }

             bool isMatch = false;
             try
             {
                 // Iterate through regex patterns, ensuring inner objects aren't null
                 foreach (var regexInfo in invoice.InvoiceIdentificatonRegEx.Where(r => r?.OCR_RegularExpressions != null))
                 {
                     string pattern = regexInfo.OCR_RegularExpressions.RegEx;
                     int regexId = regexInfo.OCR_RegularExpressions.Id;

                     if (string.IsNullOrEmpty(pattern))
                     {
                         _logger.Verbose("Skipping empty regex pattern for RegexId: {RegexId}, InvoiceId: {InvoiceId}", regexId, invoiceId);
                         continue; // Skip empty patterns
                     }

                     _logger.Verbose("Testing RegexId: {RegexId}, Pattern: '{Pattern}' against file text for InvoiceId: {InvoiceId}", regexId, pattern, invoiceId);
                     // Use compiled regex options and add a timeout
                     isMatch = Regex.IsMatch(fileText,
                         pattern,
                         RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                         RegexTimeout); // Use defined timeout

                     if (isMatch)
                     {
                         _logger.Information("Regex match FOUND! RegexId: {RegexId} matched for InvoiceId: {InvoiceId}, File: {FilePath}. Document identified as this invoice type.", regexId, invoiceId, filePath);
                         return true; // Exit method on first match
                     }
                     else
                     {
                          _logger.Verbose("RegexId: {RegexId} did not match for InvoiceId: {InvoiceId}", regexId, invoiceId);
                     }
                 }
             }
             catch (RegexMatchTimeoutException timeoutEx)
             {
                  // Log timeout specifically
                  _logger.Error(timeoutEx, "Regex match timed out (>{TimeoutSeconds}s) while checking InvoiceId: {InvoiceId}, RegexId: {RegexId} for File: {FilePath}",
                        RegexTimeout.TotalSeconds, invoiceId, "N/A", filePath); // Cannot easily get RegexId here if it timed out
                  return false; // Treat timeout as non-match
             }
             catch (Exception ex)
             {
                  // Log any other exceptions during regex processing
                  _logger.Error(ex, "Error during regex matching process for InvoiceId: {InvoiceId} for File: {FilePath}", invoiceId, filePath);
                  return false; // Treat error as non-match
             }

             // Log only if no patterns matched after checking all of them
             _logger.Debug("No identifying regex patterns matched for InvoiceId: {InvoiceId}, File: {FilePath}. Document NOT identified as this invoice type.", invoiceId, filePath);
             return false; // No match found
        }
    }
}