using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep
    {
        private static bool ExtractCsvData(InvoiceProcessingContext context, out List<IDictionary<string, object>> list)
        {
            list = null; // Initialize out parameter
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id;
            _logger.Verbose(
                "Attempting to extract CsvLines[0] as List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}",
                filePath, templateId);

            // Check if CsvLines is valid and has at least one element
            // Context null check happens in Execute
            if (context.CsvLines == null || !context.CsvLines.Any())
            {
                _logger.Warning(
                    "Cannot extract CSV data: CsvLines is null or empty for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath, templateId);
                return false; // Cannot extract
            }

            try
            {
                // Attempt the cast using 'as' for safety
                list = context.CsvLines[0] as List<IDictionary<string, object>>;

                if (list != null)
                {
                    _logger.Verbose(
                        "Successfully cast CsvLines[0] to List<IDictionary<string, object>> with {Count} items for File: {FilePath}, TemplateId: {TemplateId}",
                        list.Count, filePath, templateId);
                    return true; // Extraction successful
                }
                else
                {
                    // Log the actual type if the cast fails
                    _logger.Warning(
                        "Failed to cast CsvLines[0] to List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}. Actual type: {ActualType}",
                        filePath, templateId, context.CsvLines[0]?.GetType().FullName ?? "null");
                    return false; // Cast failed
                }
            }
            catch (ArgumentOutOfRangeException rangeEx) // Catch specific exception if index is invalid
            {
                _logger.Error(rangeEx,
                    "Error accessing CsvLines[0] (Index out of range?) for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath, templateId);
                return false;
            }
            catch (Exception ex) // Catch other potential errors during access/cast
            {
                _logger.Error(ex,
                    "Error during CSV data extraction (accessing/casting CsvLines[0]) for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath, templateId);
                return false; // Error during extraction
            }
        }
    }
}