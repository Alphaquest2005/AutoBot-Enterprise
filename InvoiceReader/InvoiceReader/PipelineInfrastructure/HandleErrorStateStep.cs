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
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<HandleErrorStateStep>();

        private readonly bool _isLastTemplate; // Need to get this from somewhere, maybe context or constructor

        // Assuming _isLastTemplate is passed in the constructor
        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
             _logger.Debug("HandleErrorStateStep initialized with IsLastTemplate: {IsLastTemplate}", _isLastTemplate);
             // Consider logging a warning if _isLastTemplate is intended to be used but is commented out in IsValidErrorState
             _logger.Warning("Note: _isLastTemplate field is initialized but currently commented out in IsValidErrorState check.");
        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id; // Safe access
            _logger.Debug("Executing HandleErrorStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

            // Null check context first
            if (context == null)
            {
                 _logger.Error("HandleErrorStateStep executed with null context.");
                 return false;
            }

            if (HasMissingRequiredData(context)) // Handles its own logging
            {
                 _logger.Warning("HandleErrorStateStep cannot proceed due to missing required data in context for File: {FilePath}", filePath);
                 return false; // Step fails if required data is missing
            }

            _logger.Debug("Getting failed lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> failedlines = GetFailedLines(context); // Handles its own logging
            _logger.Debug("Initial failed lines count: {Count}", failedlines.Count);

            _logger.Debug("Adding existing failed lines from template parts for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            AddExistingFailedLines(context, failedlines); // Handles its own logging
            _logger.Debug("Total failed lines count after adding existing: {Count}", failedlines.Count);


            _logger.Debug("Getting distinct required lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> allRequired = GetDistinctRequiredLines(context); // Handles its own logging
            _logger.Debug("Distinct required lines count: {Count}", allRequired.Count);

            // Check if all required lines failed (and there are required lines)
            // Use safe count access
            if (allRequired.Any() && failedlines.Count >= allRequired.Count)
            {
                 _logger.Warning("All {RequiredCount} required lines appear to have failed for File: {FilePath}, TemplateId: {TemplateId}. Returning false from HandleErrorStateStep.", allRequired.Count, filePath, templateId);
                 return false; // Indicate failure if all required lines failed
            }
             _logger.Debug("Not all required lines failed (or no required lines found). Proceeding with error state check.");


            _logger.Debug("Checking if current state is a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            if (IsValidErrorState(context, failedlines)) // Handles its own logging
            {
                 _logger.Information("Valid error state detected. Handling error email pipeline for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 // HandleErrorEmailPipeline handles its own logging
                 // The return value indicates if the email pipeline was *attempted*, not necessarily if it succeeded.
                 bool emailAttempted = await HandleErrorEmailPipeline(context).ConfigureAwait(false);
                 _logger.Information("HandleErrorEmailPipeline finished for File: {FilePath}. Email Attempted: {EmailAttempted}", filePath, emailAttempted);
                 // Decide what Execute should return. If email was attempted, maybe it's 'true' for this step?
                 // Original code returned true here. Let's assume attempting email means the step 'succeeded' in its error handling task.
                 return true;
            }
            else
            {
                 _logger.Information("Not a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}. Handling as unsuccessful import.", filePath, templateId);
                 // HandleUnsuccessfulImport handles its own logging
                 return HandleUnsuccessfulImport(filePath); // Pass context for logging
            }
        }

        // Added filePath for context
    }
}