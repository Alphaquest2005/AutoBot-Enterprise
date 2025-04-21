// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Invoice
using Core.Common.Extensions; // Added for BetterExpando

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        // Made method async as it calls Task.Run indirectly via helpers if they were async
        // but helpers are synchronous in this version. Keeping async Task<bool> signature.
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id; // Access Id through OcrInvoices
            string templateName = context?.Template?.OcrInvoices?.Name ?? "Unknown";
            _logger.Debug("Executing ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}, TemplateName: '{TemplateName}'", filePath, templateId, templateName);

            // Null checks
            if (context == null)
            {
                 _logger.Error("ReadFormattedTextStep executed with null context.");
                 return false;
            }
            if (context.Template == null)
            {
                 _logger.Warning("Skipping ReadFormattedTextStep: Template is null for File: {FilePath}", filePath);
                 return false; // Handle the case where the template is not available
            }
             if (string.IsNullOrEmpty(context.FormattedPdfText))
             {
                  _logger.Warning("Skipping ReadFormattedTextStep: FormattedPdfText is null or empty for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  return false; // Cannot read without text
             }

            try
            {
                _logger.Debug("Starting data extraction using template for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

                // Log the FormattedPdfText
                _logger.Verbose("FormattedPdfText:\n{FormattedPdfText}", context.FormattedPdfText);

                // Log regex patterns for lines in the template
                if (context.Template?.OcrInvoices?.Parts != null)
                {
                    _logger.Verbose("Template Regex Patterns:");
                    foreach (var part in context.Template.OcrInvoices.Parts)
                    {
                        if (part.Lines != null)
                        {
                            foreach (var line in part.Lines)
                            {
                                if (line.RegularExpressions != null)
                                {
                                    _logger.Verbose("  PartId: {PartId}, Line: {LineName}, Regex: {RegexPattern}",
                                        part.Id, line.Name ?? "Unknown", line.RegularExpressions.RegEx);
                                }
                            }
                        }
                    }
                }

                // 1. Split the input text into lines
                var textLines = context.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                _logger.Verbose("Split FormattedPdfText into {LineCount} lines.", textLines.Count);

                // 2. Get the top-level parts from the template
                // In the original Invoice.cs, this was filtered to parts with no parents or no children.
                // Replicating that logic here.
                var topLevelParts = context.Template.Parts
                    .Where(p => (p.OCR_Part.ParentParts.Any() && !p.OCR_Part.ChildParts.Any()) ||
                                (!p.OCR_Part.ParentParts.Any() && !p.OCR_Part.ChildParts.Any()))
                    .ToList(); // context.Template.Parts is already List<WaterNut.DataSpace.Part>
                _logger.Verbose("Identified {TopLevelPartCount} top-level parts from template.", topLevelParts.Count);

                // Call the Read method on the context.Template (WaterNut.DataSpace.Invoice)
                // This method encapsulates the logic for processing lines, parts, and assembling results.
                _logger.Debug("Calling context.Template.Read with {LineCount} lines for File: {FilePath}, TemplateId: {TemplateId}", textLines.Count, filePath, templateId);
                context.CsvLines = context.Template.Read(textLines);
                _logger.Debug("context.Template.Read finished for File: {FilePath}, TemplateId: {TemplateId}. Result count: {ResultCount}", filePath, templateId, context.CsvLines?.Count ?? 0);

                // Check if extraction failed (indicated by null or empty CsvLines or empty inner list)
                if (context.CsvLines == null || !context.CsvLines.Any() || (context.CsvLines.First() is List<IDictionary<string, object>> innerList && !innerList.Any()))
                {
                    _logger.Warning("CsvLines is null or empty after extraction attempt for File: {FilePath}, TemplateId: {TemplateId}. Step fails.", filePath, templateId);
                    return false;
                }

                // Assuming the success of the read operation is now determined by the presence of CsvLines
                bool success = true; // If we reached here, extraction was considered successful

                _logger.Information("ReadFormattedTextStep finished for File: {FilePath}, TemplateId: {TemplateId}. Step Success: {Success}.", filePath, templateId, success);
                return success;
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error during ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 // Ensure CsvLines is cleared or handled appropriately on error
                 if (context != null) context.CsvLines = null;
                 return false; // Indicate failure
            }
        }

        // Removed helper methods ExtractCsvLines, ProcessCsvDataExtraction, and ValidateTemplateSuccess
    }
}