using OCR.Business.Entities; // Added
using System.Collections.Generic; // Added
using System.IO; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class HandleErrorStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<HandleErrorStateStep>();
        private readonly bool _isLastTemplate;
        // Removed static InvoiceProcessingContext _context; - Use context passed to Execute

        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
            _logger.Debug("HandleErrorStateStep initialized with IsLastTemplate: {IsLastTemplate}", _isLastTemplate);
            _logger.Warning("Note: _isLastTemplate field is initialized but currently commented out in IsValidErrorState check.");
            

        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
             // Basic context validation
            if (context == null)
            {
                _logger.Error("HandleErrorStateStep executed with null context.");
                return false;
            }
             // Removed static context assignment: _context = context;
             
             // Check for templates - if none, step succeeds vacuously.
             if (!context.Templates.Any())
             {
                  _logger.Warning("Skipping HandleErrorStateStep: No Templates found in context for File: {FilePath}", context.FilePath ?? "Unknown");
                  return true;
             }

            string filePath = context.FilePath ?? "Unknown";
            bool overallStepSuccess = true; // Track success across all templates

            foreach (var template in context.Templates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Safe access
                 _logger.Information("Executing HandleErrorStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 _logger.Verbose("Context details at start of HandleErrorStateStep: {@Context}", context);

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template/context ---
                     // Assuming HasMissingRequiredData checks the *overall* context, not just the template
                     if (HasMissingRequiredData(context)) // Handles its own logging
                     {
                         string errorMsg = $"HandleErrorStateStep cannot proceed due to missing required data in context for File: {filePath}, TemplateId: {templateId}";
                         _logger.Warning(errorMsg); // Logged by helper, but log again for step context
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false; // Mark step as failed
                         break; // Stop processing other templates
                     }
                     // --- End Validation ---

                     // --- Process Template Logic ---
                     // Pass the full context, not just the template
                     bool templateProcessResult = await ProcessTemplate(context, template, filePath, templateId).ConfigureAwait(false);
                     
                     if (!templateProcessResult)
                     {
                          // ProcessTemplate or its helpers should have logged and added the specific error to context.Errors
                          _logger.Error("ProcessTemplate failed for File: {FilePath}, TemplateId: {TemplateId}. See previous errors.", filePath, templateId);
                          overallStepSuccess = false;
                          break; // Stop processing other templates
                     }
                     // --- End Process Template Logic ---

                     _logger.Information("Finished processing HandleErrorStateStep successfully for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Unexpected error during HandleErrorStateStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     _logger.Error(ex, errorMsg); // Log the error with exception details
                     context.AddError(errorMsg); // Add error to context
                     overallStepSuccess = false; // Mark the overall step as failed
                     break; // Stop processing immediately on error
                 }
            }

            // Log final status based on whether all templates were processed without error
            if (overallStepSuccess)
            {
                 _logger.Information("HandleErrorStateStep completed successfully for all applicable templates in File: {FilePath}.", filePath);
            }
            else
            {
                 _logger.Error("HandleErrorStateStep failed for at least one template in File: {FilePath}. See previous errors.", filePath);
            }
            
            return overallStepSuccess;
        }

        // Corrected signature: Takes InvoiceProcessingContext and the specific Invoice template
        private async Task<bool> ProcessTemplate(InvoiceProcessingContext context, Invoice template, string filePath, int? templateId)
        {
            // Assuming helper methods now correctly use 'template' where appropriate,
            // but might need 'context' for broader info (like Client, EmailId etc.)
            
            List<Line> failedLines = GetFailedLinesWithLogging(template, filePath, templateId); // Pass template
            AddExistingFailedLinesWithLogging(template, failedLines, filePath, templateId); // Pass template

            List<Line> allRequired = GetDistinctRequiredLinesWithLogging(template, filePath, templateId); // Pass template

            if (allRequired.Any() && failedLines.Count >= allRequired.Count)
            {
                string errorMsg = $"All {allRequired.Count} required lines appear to have failed for File: {filePath}, TemplateId: {templateId}.";
                _logger.Warning(errorMsg);
                context.AddError(errorMsg); // Add specific error
                return false; // Indicate failure for this template
            }

            // Pass the full context to HandleErrorState as it likely needs it for email/reporting
            return await HandleErrorState(context, template, failedLines, filePath, templateId).ConfigureAwait(false);
        }

        // Corrected signature: Takes Invoice template
        private List<Line> GetFailedLinesWithLogging(Invoice template, string filePath, int? templateId)
        {
            _logger.Debug("Getting failed lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> failedLines = GetFailedLines(template); // Pass template
            _logger.Debug("Initial failed lines count: {Count}", failedLines.Count);
            _logger.Verbose("Initial failed lines details: {@FailedLines}", failedLines);
            return failedLines;
        }

        // Corrected signature: Takes Invoice template
        private void AddExistingFailedLinesWithLogging(Invoice template, List<Line> failedLines, string filePath, int? templateId)
        {
            _logger.Debug("Adding existing failed lines from template parts for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            AddExistingFailedLines(template, failedLines); // Pass template
            _logger.Debug("Total failed lines count after adding existing: {Count}", failedLines.Count);
            _logger.Verbose("Total failed lines details after adding existing: {@FailedLines}", failedLines);
        }

        // Corrected signature: Takes Invoice template
        private List<Line> GetDistinctRequiredLinesWithLogging(Invoice template, string filePath, int? templateId)
        {
            _logger.Debug("Getting distinct required lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> allRequired = GetDistinctRequiredLines(template); // Pass template
            _logger.Debug("Distinct required lines count: {Count}", allRequired.Count);
            _logger.Verbose("Distinct required lines details: {@RequiredLines}", allRequired);
            return allRequired;
        }

        // Corrected signature: Takes InvoiceProcessingContext and Invoice template
        private async Task<bool> HandleErrorState(InvoiceProcessingContext context, Invoice template, List<Line> failedLines, string filePath, int? templateId)
        {
            _logger.Debug("Checking if current state is a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            bool isValidErrorState = IsValidErrorState(template, failedLines, filePath); // Pass template and filePath
            _logger.Debug("IsValidErrorState check result: {Result}", isValidErrorState);

            if (isValidErrorState)
            {
                _logger.Information("Valid error state detected. Handling error email pipeline for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                // Pass context, template, and filePath to HandleErrorEmailPipeline
                bool emailResult = await HandleErrorEmailPipeline(context, template, filePath).ConfigureAwait(false);
                _logger.Information("HandleErrorEmailPipeline finished for File: {FilePath}. Result: {EmailResult}", filePath, emailResult);
                // If email sending fails, should the step fail? Assuming yes for now.
                if (!emailResult)
                {
                     context.AddError($"HandleErrorEmailPipeline failed for File: {filePath}, TemplateId: {templateId}.");
                }
                return emailResult; // Return success/failure of email handling
            }

            _logger.Information("Not a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}. Handling as standard unsuccessful import.", filePath, templateId);
            // HandleUnsuccessfulImport only needs the file path
            bool importHandled = HandleUnsuccessfulImport(filePath); // Assuming sync or handle async if needed
             if (!importHandled)
             {
                  context.AddError($"HandleUnsuccessfulImport failed for File: {filePath}, TemplateId: {templateId}.");
             }
            return importHandled; // Return success/failure of handling
        }
    }
}