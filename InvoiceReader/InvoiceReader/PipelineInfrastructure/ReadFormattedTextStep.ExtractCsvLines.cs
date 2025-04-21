using System;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep
    {
        private static void ExtractCsvLines(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id;
            _logger.Verbose("Calling Template.Read method for File: {FilePath}, TemplateId: {TemplateId}", filePath,
                templateId);
            // Template and FormattedPdfText null checks happen in Execute

            try
            {
                // Assuming Read method returns List<dynamic> and is synchronous
                context.CsvLines = context.Template.Read(context.FormattedPdfText);
                _logger.Information(
                    "Template.Read returned {Count} data structure(s) for File: {FilePath}, TemplateId: {TemplateId}",
                    context.CsvLines?.Count ?? 0, filePath, templateId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error calling Template.Read for File: {FilePath}, TemplateId: {TemplateId}",
                    filePath,
                    templateId);
                context.CsvLines = null; // Ensure CsvLines is null on error
                // Re-throw the exception so the main Execute block catches it and fails the step
                throw;
            }
        }
    }
}