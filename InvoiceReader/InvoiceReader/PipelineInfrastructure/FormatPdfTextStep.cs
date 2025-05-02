// Assuming this is needed for _template.Format

using System.Text; // Added for StringBuilder
using System.Threading.Tasks;
using Serilog; // Added
using System; // Added for Exception
using System.Linq; // Added for OrderBy

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class FormatPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<FormatPdfTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            LogExecutionStart(filePath);

            if (!ValidateContext(context, filePath))
            {
                return false;
            }

            foreach (var template in context?.Templates)
            {
                int templateId = template.OcrInvoices?.Id ?? 0;
                LogTemplateDetails(templateId, filePath, context.PdfText.Length);

                try
                {
                    string pdfTextString = context.PdfText.ToString();
                    LogFormattingStart(templateId);

                    ////////////////////////////////////////
                    template.FormattedPdfText = template.Format(pdfTextString);
                    ////////////////////////////////////////

                    LogFormattedTextDetails(template.FormattedPdfText, templateId);

                    LogExecutionSuccess(templateId, filePath);
                    
                }
                catch (Exception ex) // Catch errors during formatting for a specific template
                {
                    string errorMsg = $"Error formatting PDF text using TemplateId: {templateId} for File: {filePath}: {ex.Message}";
                    LogExecutionError(ex, templateId, filePath); // Log the error with details
                    context.AddError(errorMsg); // Add the specific error to the context
                    return false; // Stop processing this step and indicate failure
                }
            }

            return true;

        }

        private void LogExecutionStart(string filePath)
        {
            _logger.Debug("Executing FormatPdfTextStep for File: {FilePath}", filePath);
        }

        private bool ValidateContext(InvoiceProcessingContext context, string filePath)
        {
            if (context == null)
            {
                _logger.Error("FormatPdfTextStep executed with null context.");
                return false;
            }
            if (!context.Templates.Any())
            {
                _logger.Warning("Skipping FormatPdfTextStep: No Templates for File: {FilePath}", filePath);
                return false;
            }
            if (context.PdfText == null)
            {
                _logger.Warning("Skipping FormatPdfTextStep: PdfText (StringBuilder) is null for File: {FilePath}", filePath);
                return false;
            }
            return true;
        }

        private void LogTemplateDetails(int templateId, string filePath, int pdfTextLength)
        {
            _logger.Debug("Formatting PDF text using TemplateId: {TemplateId} for File: {FilePath}", templateId, filePath);
            _logger.Verbose("Original PdfText Length: {Length}", pdfTextLength);
        }

        private void LogFormattingStart(int templateId)
        {
            _logger.Verbose("Starting formatting using formatters for TemplateId: {TemplateId}", templateId);
        }

        private void LogFormattedTextDetails(string formattedPdfText, int templateId)
        {
            _logger.Verbose("Formatted PdfText Length: {Length}", formattedPdfText?.Length ?? 0);
            if (!string.IsNullOrEmpty(formattedPdfText))
            {
                _logger.Verbose("Formatted PdfText (first 500 chars): {FormattedText}", formattedPdfText.Substring(0, Math.Min(formattedPdfText.Length, 500)));
            }
        }

        private void LogExecutionSuccess(int templateId, string filePath)
        {
            _logger.Information("PDF text formatted using TemplateId: {TemplateId} for File: {FilePath}.", templateId, filePath);
            _logger.Debug("Finished executing FormatPdfTextStep successfully for File: {FilePath}", filePath);
        }

        private void LogExecutionError(Exception ex, int templateId, string filePath)
        {
            _logger.Error(ex, "Error formatting PDF text using TemplateId: {TemplateId} for File: {FilePath}", templateId, filePath);
        }
    }
}