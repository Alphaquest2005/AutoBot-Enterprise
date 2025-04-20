// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Invoice

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
            int? templateId = context?.Template?.OcrInvoices?.Id; // Safe access
            _logger.Debug("Executing ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

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
                _logger.Debug("Extracting CsvLines using Template.Read for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                ExtractCsvLines(context); // Handles its own logging
                _logger.Debug("CsvLines extraction finished for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

                // Check if extraction failed (indicated by null CsvLines)
                if (context.CsvLines == null)
                {
                    _logger.Warning("CsvLines is null after extraction attempt for File: {FilePath}, TemplateId: {TemplateId}. Step fails.", filePath, templateId);
                    return false;
                }

                _logger.Debug("Processing extracted CsvData for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                ProcessCsvDataExtraction(context); // Handles its own logging
                _logger.Debug("CsvData processing finished for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

                _logger.Debug("Validating Template.Success flag for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                bool success = ValidateTemplateSuccess(context); // Handles its own logging
                _logger.Information("ReadFormattedTextStep finished for File: {FilePath}, TemplateId: {TemplateId}. Step Success: {Success}", filePath, templateId, success);
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

        // Returns true if extraction was *attempted* and CsvLines[0] *is* the expected list type, false otherwise.
        // Outputs the extracted list (or null) via the out parameter.
    }
}