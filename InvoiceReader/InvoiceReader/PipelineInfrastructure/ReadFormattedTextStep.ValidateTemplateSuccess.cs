namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep
    {
        private static bool ValidateTemplateSuccess(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id;
            // Safe access to Success property, default to false if Template is null (though checked earlier)
            bool templateSuccess = context?.Template?.Success ?? false;

            _logger.Verbose(
                "Validating Template.Success flag for File: {FilePath}, TemplateId: {TemplateId}. Current Value: {TemplateSuccess}",
                filePath, templateId, templateSuccess);

            if (!templateSuccess)
            {
                _logger.Warning(
                    "Template.Success is false after reading for File: {FilePath}, TemplateId: {TemplateId}. Indicates template did not match or read failed.",
                    filePath, templateId);
                return false; // Step fails
            }
            else
            {
                _logger.Information(
                    "Template.Success is true after reading for File: {FilePath}, TemplateId: {TemplateId}.", filePath,
                    templateId);
                return true; // Indicate success
            }
        }
    }
}
