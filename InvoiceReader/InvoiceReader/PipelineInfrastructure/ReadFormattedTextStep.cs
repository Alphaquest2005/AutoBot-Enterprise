// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Invoice

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
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

        private static void ProcessCsvDataExtraction(InvoiceProcessingContext context)
        {
             string filePath = context?.FilePath ?? "Unknown";
             int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Starting ProcessCsvDataExtraction for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

             // Check if CsvLines is valid before trying to extract data
             if (context.CsvLines == null || !context.CsvLines.Any())
             {
                  _logger.Warning("Cannot process CSV data: CsvLines is null or empty for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  return; // Nothing to process
             }

             List<IDictionary<string, object>> list = null;
             bool extractionSuccess = ExtractCsvData(context, out list); // Handles its own logging

             if (extractionSuccess && list != null)
             {
                  _logger.Information("Successfully extracted first data structure (List<IDictionary<string, object>>) with {Count} items for File: {FilePath}, TemplateId: {TemplateId}", list.Count, filePath, templateId);
             }
             else
             {
                  // Logging for failure happens within ExtractCsvData
                  _logger.Warning("Failed to extract first data structure as List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
             }
             _logger.Verbose("Finished ProcessCsvDataExtraction for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }

        private static bool ValidateTemplateSuccess(InvoiceProcessingContext context)
        {
             string filePath = context?.FilePath ?? "Unknown";
             int? templateId = context?.Template?.OcrInvoices?.Id;
             // Safe access to Success property, default to false if Template is null (though checked earlier)
             bool templateSuccess = context?.Template?.Success ?? false;

             _logger.Verbose("Validating Template.Success flag for File: {FilePath}, TemplateId: {TemplateId}. Current Value: {TemplateSuccess}", filePath, templateId, templateSuccess);

            if (!templateSuccess)
            {
                 _logger.Warning("Template.Success is false after reading for File: {FilePath}, TemplateId: {TemplateId}. Indicates template did not match or read failed.", filePath, templateId);
                 return false; // Step fails
            }
            else
            {
                 _logger.Information("Template.Success is true after reading for File: {FilePath}, TemplateId: {TemplateId}.", filePath, templateId);
                 return true; // Indicate success
            }
        }

        // Returns true if extraction was *attempted* and CsvLines[0] *is* the expected list type, false otherwise.
        // Outputs the extracted list (or null) via the out parameter.
        private static bool ExtractCsvData(InvoiceProcessingContext context, out List<IDictionary<string, object>> list)
        {
            list = null; // Initialize out parameter
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Attempting to extract CsvLines[0] as List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

             // Check if CsvLines is valid and has at least one element
             // Context null check happens in Execute
             if (context.CsvLines == null || !context.CsvLines.Any())
             {
                  _logger.Warning("Cannot extract CSV data: CsvLines is null or empty for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  return false; // Cannot extract
             }

             try
             {
                 // Attempt the cast using 'as' for safety
                 list = context.CsvLines[0] as List<IDictionary<string, object>>;

                 if (list != null)
                 {
                      _logger.Verbose("Successfully cast CsvLines[0] to List<IDictionary<string, object>> with {Count} items for File: {FilePath}, TemplateId: {TemplateId}", list.Count, filePath, templateId);
                      return true; // Extraction successful
                 }
                 else
                 {
                      // Log the actual type if the cast fails
                      _logger.Warning("Failed to cast CsvLines[0] to List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}. Actual type: {ActualType}", filePath, templateId, context.CsvLines[0]?.GetType().FullName ?? "null");
                      return false; // Cast failed
                 }
             }
             catch (ArgumentOutOfRangeException rangeEx) // Catch specific exception if index is invalid
             {
                  _logger.Error(rangeEx, "Error accessing CsvLines[0] (Index out of range?) for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  return false;
             }
             catch (Exception ex) // Catch other potential errors during access/cast
             {
                  _logger.Error(ex, "Error during CSV data extraction (accessing/casting CsvLines[0]) for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  return false; // Error during extraction
             }
        }

        private static void ExtractCsvLines(InvoiceProcessingContext context)
        {
             string filePath = context?.FilePath ?? "Unknown";
             int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Calling Template.Read method for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
             // Template and FormattedPdfText null checks happen in Execute

             try
             {
                 // Assuming Read method returns List<dynamic> and is synchronous
                 context.CsvLines = context.Template.Read(context.FormattedPdfText);
                  _logger.Information("Template.Read returned {Count} data structure(s) for File: {FilePath}, TemplateId: {TemplateId}", context.CsvLines?.Count ?? 0, filePath, templateId);
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error calling Template.Read for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  context.CsvLines = null; // Ensure CsvLines is null on error
                  // Re-throw the exception so the main Execute block catches it and fails the step
                  throw;
             }
        }
    }
}