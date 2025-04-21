using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep
    {
        private static void ProcessCsvDataExtraction(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id;
            _logger.Verbose("Starting ProcessCsvDataExtraction for File: {FilePath}, TemplateId: {TemplateId}",
                filePath,
                templateId);

            // Check if CsvLines is valid before trying to extract data
            if (context.CsvLines == null || !context.CsvLines.Any())
            {
                _logger.Warning(
                    "Cannot process CSV data: CsvLines is null or empty for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath, templateId);
                return; // Nothing to process
            }

            List<IDictionary<string, object>> list = null;
            bool extractionSuccess = ExtractCsvData(context, out list); // Handles its own logging

            if (extractionSuccess && list != null)
            {
                _logger.Information(
                    "Successfully extracted first data structure (List<IDictionary<string, object>>) with {Count} items for File: {FilePath}, TemplateId: {TemplateId}",
                    list.Count, filePath, templateId);
            }
            else
            {
                // Logging for failure happens within ExtractCsvData
                _logger.Warning(
                    "Failed to extract first data structure as List<IDictionary<string, object>> for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath, templateId);
            }

            _logger.Verbose("Finished ProcessCsvDataExtraction for File: {FilePath}, TemplateId: {TemplateId}",
                filePath,
                templateId);
        }
    }
}