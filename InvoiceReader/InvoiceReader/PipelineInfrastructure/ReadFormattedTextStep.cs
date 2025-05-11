// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System;
using System.Runtime.InteropServices; // Added
using OCR.Business.Entities; // Added for Invoice
using Core.Common.Extensions; // Added for BetterExpando

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            // Basic context validation (null check)
            if (context == null)
            {
                LogNullContextError();
                // Cannot add error as context is null
                return Task.FromResult(false);
            }
             if (!context.Templates.Any())
            {
                 _logger.Warning("Skipping ReadFormattedTextStep: No Templates found in context.");
                 // Not necessarily an error, but nothing to process. Consider if this should be true or false based on pipeline logic.
                 // Returning true as no processing *failed*, just skipped.
                 return Task.FromResult(true);
            }


            string filePath = context.FilePath ?? "Unknown"; // Safe now due to null check above

            foreach (var template in context.Templates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Get template ID safely

                try
                {
                    // --- Validation ---
                    if (!ExecutionValidation(template, filePath))
                    {
                        // ExecutionValidation logs the specific reason
                        string errorMsg = $"Validation failed for TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}.";
                        context.AddError(errorMsg); // Add error to context
                        return Task.FromResult(false); // Stop processing immediately
                    }
                    // --- End Validation ---

                    var textLines = GetTextLinesFromFormattedPdfText(template, filePath);

                    // --- Template Read Execution ---
                    try
                    {
                         LogCallingTemplateRead(textLines.Count, filePath, templateId);
                         template.CsvLines = template.Read(textLines); // The core operation
                         LogTemplateReadFinished(filePath, templateId, template.CsvLines?.Count ?? 0);
                    }
                    catch (Exception readEx) // Catch errors specifically from template.Read()
                    {
                         string errorMsg = $"Error executing template.Read() for TemplateId: {templateId}, File: {filePath}: {readEx.Message}";
                         LogExecutionError(readEx, filePath, templateId); // Log detailed error
                         context.AddError(errorMsg); // Add error to context
                         template.CsvLines = null; // Ensure CsvLines is null after failure
                         return Task.FromResult(false); // Stop processing immediately
                    }
                    // --- End Template Read Execution ---


                    // --- Result Check ---
                    if (!ExecutionSuccess(template, filePath)) // Checks if CsvLines is null or empty
                    {
                         // ExecutionSuccess logs the specific reason (empty CsvLines)
                         string errorMsg = $"No CsvLines generated after read for TemplateId: {templateId}, File: {filePath}.";
                         context.AddError(errorMsg); // Add error to context
                         return Task.FromResult(false); // Stop processing immediately
                    }
                     // --- End Result Check ---

                     // If we reach here, this template was processed successfully. Continue to the next if any.
                     LogExecutionSuccess(filePath, templateId); // Log individual template success

                }
                catch (Exception ex) // Catch unexpected errors within the loop but outside template.Read()
                {
                    string errorMsg = $"Unexpected error processing TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}: {ex.Message}";
                    LogExecutionError(ex, filePath, templateId); // Log detailed error
                    context.AddError(errorMsg); // Add error to context
                    template.CsvLines = null; // Ensure CsvLines is null
                    return Task.FromResult(false); // Stop processing immediately
                }
            }

            // If the loop completes without any template causing a 'return false', the step is successful.
             _logger.Information("ReadFormattedTextStep completed successfully for all applicable templates in File: {FilePath}.", filePath);
            return Task.FromResult(true);
        }

        // Validation specific to one template instance
        private bool ExecutionValidation(Invoice template, string filePath)
        {
             if (template == null || template.OcrInvoices == null)
             {
                  LogNullTemplateWarning(filePath); // Logs appropriate message
                  return false;
             }

             int? templateId = template.OcrInvoices.Id; // Safe now
             string templateName = template.OcrInvoices.Name; // Safe now
             LogExecutionStart(filePath, templateId, templateName);
           
            if (string.IsNullOrEmpty(template.FormattedPdfText))
            {
                LogEmptyFormattedPdfTextWarning(filePath, templateId);
                return false;
            }

            return true;
        }

        // Checks if the result of template.Read() is valid (not null/empty)
        private bool ExecutionSuccess(Invoice template, string filePath)
        {
            // Note: Logging for finish/counts moved to main Execute method for better flow control view

            if (template.CsvLines == null || !template.CsvLines.Any())
            {
                LogEmptyCsvLinesWarning(filePath, template?.OcrInvoices?.Id); // Log the specific issue
                return false; // Indicate failure
            }

            // Logging for success moved to main Execute method after this check passes
            return true; // Indicate success
        }

        private List<string> GetTextLinesFromFormattedPdfText(Invoice template, string filePath)
        {
            LogDataExtractionStart(filePath, template.OcrInvoices.Id);
            LogFormattedPdfText(template.FormattedPdfText);

            if (template?.OcrInvoices?.Parts != null)
            {
                LogTemplateRegexPatterns(template.OcrInvoices.Parts);
            }

            var textLines = template.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            LogSplitTextLines(textLines.Count);

            var topLevelParts = template.OcrInvoices.Parts
                .Where(p => (p.ParentParts.Any() && !p.ChildParts.Any()) ||
                            (!p.ParentParts.Any() && !p.ChildParts.Any()))
                .ToList();

            LogTopLevelPartsIdentified(topLevelParts.Count);
            // Logging moved to main Execute method just before the call
            return textLines;
        }

        private void LogExecutionStart(string filePath, int? templateId, string templateName)
        {
            _logger.Debug("Executing ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}, TemplateName: '{TemplateName}'", filePath, templateId, templateName);
        }

        private void LogNullContextError()
        {
            _logger.Error("ReadFormattedTextStep executed with null context.");
        }

        private void LogNullTemplateWarning(string filePath)
        {
            _logger.Warning("Skipping ReadFormattedTextStep: Template is null for File: {FilePath}", filePath);
        }

        private void LogEmptyFormattedPdfTextWarning(string filePath, int? templateId)
        {
            _logger.Warning("Skipping ReadFormattedTextStep: FormattedPdfText is null or empty for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }

        private void LogDataExtractionStart(string filePath, int? templateId)
        {
            _logger.Debug("Starting data extraction using template for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }

        private void LogFormattedPdfText(string formattedPdfText)
        {
            _logger.Verbose("FormattedPdfText:\n{FormattedPdfText}", formattedPdfText);
        }

        private void LogTemplateRegexPatterns(List<Parts> parts)
        {
            _logger.Verbose("Template Regex Patterns:");
            foreach (var part in parts)
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

        private void LogSplitTextLines(int lineCount)
        {
            _logger.Verbose("Split FormattedPdfText into {LineCount} lines.", lineCount);
        }

        private void LogTopLevelPartsIdentified(int topLevelPartCount)
        {
            _logger.Verbose("Identified {TopLevelPartCount} top-level parts from template.", topLevelPartCount);
        }

        private void LogCallingTemplateRead(int lineCount, string filePath, int? templateId)
        {
            _logger.Debug("Calling context.Template.Read with {LineCount} lines for File: {FilePath}, TemplateId: {TemplateId}", lineCount, filePath, templateId);
        }

        // Log message updated slightly for clarity
        private void LogTemplateReadFinished(string filePath, int? templateId, int resultCount)
        {
            _logger.Debug("template.Read() finished for File: {FilePath}, TemplateId: {TemplateId}. Resulting CsvLines count: {ResultCount}", filePath, templateId, resultCount);
        }

        private void LogEmptyCsvLinesWarning(string filePath, int? templateId)
        {
            _logger.Warning("CsvLines is null or empty after extraction attempt for File: {FilePath}, TemplateId: {TemplateId}. Step fails.", filePath, templateId);
        }

        private void LogExecutionSuccess(string filePath, int? templateId)
        {
            _logger.Information("ReadFormattedTextStep finished for File: {FilePath}, TemplateId: {TemplateId}. Step Success: true.", filePath, templateId);
        }

        private void LogExecutionError(Exception ex, string filePath, int? templateId)
        {
            _logger.Error(ex, "Error during ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }
    }
}