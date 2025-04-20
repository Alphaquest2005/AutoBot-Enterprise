using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using pdf_ocr; // Assuming this namespace contains PdfOcr
using Tesseract; // Assuming this namespace contains PageSegMode
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for AggregateException handling

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class GetPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<GetPdfTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing GetPdfTextStep for File: {FilePath}", filePath);

            // Null check for context and essential properties
            if (context == null)
            {
                 _logger.Error("GetPdfTextStep executed with null context.");
                 return false;
            }
             if (string.IsNullOrEmpty(context.FilePath))
             {
                  _logger.Error("GetPdfTextStep executed with null or empty FilePath in context.");
                  return false;
             }

             _logger.Information("Getting PDF text for file '{FilePath}'", filePath);

            StringBuilder pdftxt = new StringBuilder();
            Task<string> ripTask = null, singleColumnTask = null, sparseTextTask = null;

            try
            {
                _logger.Debug("Setting up concurrent PDF text extraction tasks for File: {FilePath}", filePath);
                // Setup tasks (logging is within the called methods)
                SetupPdfTextExtraction(context, out ripTask, out singleColumnTask, out sparseTextTask);
                _logger.Debug("Tasks created for Ripped Text, Single Column OCR, Sparse Text OCR for File: {FilePath}", filePath);

                _logger.Information("Awaiting completion of PDF text extraction tasks for File: {FilePath}", filePath);
                // Wait for all tasks, exceptions will be aggregated
                await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false);
                _logger.Information("All PDF text extraction tasks completed successfully for File: {FilePath}", filePath);

                _logger.Debug("Appending PDF text results for File: {FilePath}", filePath);
                // Append results (handles logging internally)
                AppendPdfTextResults(context, pdftxt, ripTask, singleColumnTask, sparseTextTask);
                _logger.Debug("Finished appending PDF text results for File: {FilePath}. Total Length: {Length}", filePath, pdftxt.Length);
                _logger.Verbose("Extracted PDF Text (first 500 chars): {PdfText}", pdftxt.ToString().Substring(0, Math.Min(pdftxt.Length, 500))); // Log a portion of the text

                 _logger.Debug("Finished executing GetPdfTextStep successfully for File: {FilePath}", filePath);
                return true; // Indicate success
            }
            catch (AggregateException aggEx) // Catch failures from Task.WhenAll
            {
                 _logger.Error("One or more PDF text extraction tasks failed for File: {FilePath}. See inner exceptions.", filePath);
                 // Log individual exceptions from the AggregateException
                 foreach (var ex in aggEx.Flatten().InnerExceptions) // Flatten for nested AggregateExceptions
                 {
                     _logger.Error(ex, "Task Exception Detail for File: {FilePath}", filePath);
                 }
                 // Attempt to append any successful results before indicating failure
                 _logger.Warning("Attempting to append results from potentially successful tasks after failure for File: {FilePath}", filePath);
                 AppendPdfTextResults(context, pdftxt, ripTask, singleColumnTask, sparseTextTask, logErrors: true); // Log errors during append
                 _logger.Verbose("Partial Extracted PDF Text after failure (first 500 chars): {PdfText}", pdftxt.ToString().Substring(0, Math.Min(pdftxt.Length, 500))); // Log a portion of the text
                 return false; // Indicate failure
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                 _logger.Error(ex, "Unexpected error during GetPdfTextStep for File: {FilePath}", filePath);
                 return false; // Indicate failure
            }
        }

        // Added optional flag to log errors during append if tasks might have failed
    }
}