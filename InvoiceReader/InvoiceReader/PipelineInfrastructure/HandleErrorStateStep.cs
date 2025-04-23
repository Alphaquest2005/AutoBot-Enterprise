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
        static InvoiceProcessingContext _context; // Added for sending emails

        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
            _logger.Debug("HandleErrorStateStep initialized with IsLastTemplate: {IsLastTemplate}", _isLastTemplate);
            _logger.Warning("Note: _isLastTemplate field is initialized but currently commented out in IsValidErrorState check.");
            

        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context == null)
            {
                _logger.Error("HandleErrorStateStep executed with null context.");
                return false;
            }
            _context = context;
            bool stepResult = false;
            foreach (var template in context.Templates)
            {
                string filePath = context?.FilePath ?? "Unknown";
                int? templateId = template?.OcrInvoices?.Id;

                _logger.Information("Executing HandleErrorStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                _logger.Verbose("Context details at start of HandleErrorStateStep: {@Context}", context);

                if (HasMissingRequiredData(context))
                {
                    _logger.Warning("HandleErrorStateStep cannot proceed due to missing required data in context for File: {FilePath}", filePath);
                    return false;
                }

                try
                {
                    stepResult = await ProcessTemplate(template, filePath, templateId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during HandleErrorStateStep execution for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                    stepResult = false;
                }
                finally
                {
                    _logger.Information("Finished executing HandleErrorStateStep for File: {FilePath}, TemplateId: {TemplateId}. Result: {Result}", filePath, templateId, stepResult);
                }
            }

            return stepResult;
        }

        private async Task<bool> ProcessTemplate(Invoice context, string filePath, int? templateId)
        {
            List<Line> failedLines = GetFailedLinesWithLogging(context, filePath, templateId);
            AddExistingFailedLinesWithLogging(context, failedLines, filePath, templateId);

            List<Line> allRequired = GetDistinctRequiredLinesWithLogging(context, filePath, templateId);

            if (allRequired.Any() && failedLines.Count >= allRequired.Count)
            {
                _logger.Warning("All {RequiredCount} required lines appear to have failed for File: {FilePath}, TemplateId: {TemplateId}. Returning false from HandleErrorStateStep.", allRequired.Count, filePath, templateId);
                return false;
            }

            return await HandleErrorState(context, failedLines, filePath, templateId).ConfigureAwait(false);
        }

        private List<Line> GetFailedLinesWithLogging(Invoice context, string filePath, int? templateId)
        {
            _logger.Debug("Getting failed lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> failedLines = GetFailedLines(context);
            _logger.Debug("Initial failed lines count: {Count}", failedLines.Count);
            _logger.Verbose("Initial failed lines details: {@FailedLines}", failedLines);
            return failedLines;
        }

        private void AddExistingFailedLinesWithLogging(Invoice context, List<Line> failedLines, string filePath, int? templateId)
        {
            _logger.Debug("Adding existing failed lines from template parts for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            AddExistingFailedLines(context, failedLines);
            _logger.Debug("Total failed lines count after adding existing: {Count}", failedLines.Count);
            _logger.Verbose("Total failed lines details after adding existing: {@FailedLines}", failedLines);
        }

        private List<Line> GetDistinctRequiredLinesWithLogging(Invoice context, string filePath, int? templateId)
        {
            _logger.Debug("Getting distinct required lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> allRequired = GetDistinctRequiredLines(context);
            _logger.Debug("Distinct required lines count: {Count}", allRequired.Count);
            _logger.Verbose("Distinct required lines details: {@RequiredLines}", allRequired);
            return allRequired;
        }

        private async Task<bool> HandleErrorState(Invoice context, List<Line> failedLines, string filePath, int? templateId)
        {
            _logger.Debug("Checking if current state is a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            bool isValidErrorState = IsValidErrorState(context, failedLines);
            _logger.Debug("IsValidErrorState check result: {Result}", isValidErrorState);

            if (isValidErrorState)
            {
                _logger.Information("Valid error state detected. Handling error email pipeline for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                bool emailAttempted = await HandleErrorEmailPipeline(context, filePath).ConfigureAwait(false);
                _logger.Information("HandleErrorEmailPipeline finished for File: {FilePath}. Email Attempted: {EmailAttempted}", filePath, emailAttempted);
                return true;
            }

            _logger.Information("Not a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}. Handling as unsuccessful import.", filePath, templateId);
            return HandleUnsuccessfulImport(filePath);
        }
    }
}