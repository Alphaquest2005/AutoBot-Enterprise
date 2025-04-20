// Assuming this is needed for _template.Format

using System.Text; // Added for StringBuilder
using System.Threading.Tasks;
using Serilog; // Added
using System; // Added for Exception

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class FormatPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<FormatPdfTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing FormatPdfTextStep for File: {FilePath}", filePath);

            // Add null checks for required context properties
            if (context == null)
            {
                 _logger.Error("FormatPdfTextStep executed with null context.");
                 return false;
            }
            if (context.Template == null)
            {
                 _logger.Warning("Skipping FormatPdfTextStep: Template is null for File: {FilePath}", filePath);
                 return false;
            }
             // Check Template.OcrInvoices as well, as it's used later
             if (context.Template.OcrInvoices == null)
             {
                 _logger.Warning("Skipping FormatPdfTextStep: Template.OcrInvoices is null for File: {FilePath}", filePath);
                 return false;
             }
            if (context.PdfText == null)
            {
                 _logger.Warning("Skipping FormatPdfTextStep: PdfText (StringBuilder) is null for File: {FilePath}", filePath);
                 return false;
            }

            int templateId = context.Template.OcrInvoices.Id; // Already checked OcrInvoices is not null
            _logger.Debug("Formatting PDF text using TemplateId: {TemplateId} for File: {FilePath}", templateId, filePath);
            _logger.Verbose("Original PdfText Length: {Length}", context.PdfText.Length);

            try
            {
                // Assuming Template.Format exists and takes/returns string
                string pdfTextString = context.PdfText.ToString();
                _logger.Verbose("Calling Template.Format for TemplateId: {TemplateId}", templateId);
                // Assuming Format is a synchronous method
                context.FormattedPdfText = context.Template.Format(pdfTextString);
                _logger.Verbose("Formatted PdfText Length: {Length}", context.FormattedPdfText?.Length ?? 0);

                // Log success (replaces LogFormattedPdfText)
                _logger.Information("PDF text formatted using TemplateId: {TemplateId} for File: {FilePath}.", templateId, filePath);

                _logger.Debug("Finished executing FormatPdfTextStep successfully for File: {FilePath}", filePath);
                return true; // Indicate success
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error formatting PDF text using TemplateId: {TemplateId} for File: {FilePath}", templateId, filePath);
                 return false; // Indicate failure
            }
        }

        // Removed private LogFormattedPdfText method as logging is now inline
    }
}