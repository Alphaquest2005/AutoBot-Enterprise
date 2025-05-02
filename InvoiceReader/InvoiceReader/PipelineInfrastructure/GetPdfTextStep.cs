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
        private static readonly ILogger _logger = Log.ForContext<GetPdfTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing GetPdfTextStep for File: {FilePath}", filePath);

            if (!ValidateContext(context))
                return false;

            _logger.Information("Getting PDF text for file '{FilePath}'", filePath);

            StringBuilder pdftxt = new StringBuilder();
            Task<string> ripTask = null, singleColumnTask = null, sparseTextTask = null;

            try
            {
                SetupAndRunTasks(context, filePath, out ripTask, out singleColumnTask, out sparseTextTask);

                await AwaitTasksCompletion(filePath, ripTask, singleColumnTask, sparseTextTask);
                //////////////////////
                AppendResults(context, pdftxt, filePath, ripTask, singleColumnTask, sparseTextTask);
                //////////////////////
                return true;
            }
            catch (AggregateException aggEx) // Catch errors from concurrent tasks
            {
                string errorMsg = $"One or more PDF text extraction tasks failed for File: {filePath}.";
                _logger.Error(aggEx, errorMsg); // Log the aggregate exception

                // Add specific errors from inner exceptions to the context
                foreach (var innerEx in aggEx.Flatten().InnerExceptions)
                {
                    string innerErrorMsg = $"Task Error ({innerEx.Source ?? "Unknown Source"}): {innerEx.Message}";
                     _logger.Error(innerEx, "Inner Task Exception Detail for File: {FilePath}", filePath);
                    context.AddError(innerErrorMsg); // Add specific task error
                }
                
                // Attempt to append partial results even after failure
                _logger.Warning("Attempting to append results from potentially successful tasks after failure for File: {FilePath}", filePath);
                AppendResults(context, pdftxt, filePath, ripTask, singleColumnTask, sparseTextTask); // Append successful parts if any
                _logger.Verbose("Partial Extracted PDF Text after failure (first 500 chars): {PdfText}", pdftxt.ToString().Substring(0, Math.Min(pdftxt.Length, 500)));

                return false; // Indicate step failure
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors during step execution
                string generalErrorMsg = $"Unexpected error during GetPdfTextStep for File: {filePath}: {ex.Message}";
                _logger.Error(ex, generalErrorMsg);
                context.AddError(generalErrorMsg); // Add the general error to context
                return false; // Indicate step failure
            }
        }

        private bool ValidateContext(InvoiceProcessingContext context)
        {
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

            return true;
        }

        private void SetupAndRunTasks(InvoiceProcessingContext context, string filePath,
            out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            _logger.Debug("Setting up concurrent PDF text extraction tasks for File: {FilePath}", filePath);
            SetupPdfTextExtraction(context, out ripTask, out singleColumnTask, out sparseTextTask);
            _logger.Debug("Tasks created for Ripped Text, Single Column OCR, Sparse Text OCR for File: {FilePath}", filePath);
        }

        private async Task AwaitTasksCompletion(string filePath, Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask)
        {
            _logger.Information("Awaiting completion of PDF text extraction tasks for File: {FilePath}", filePath);
            await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false);
            _logger.Information("All PDF text extraction tasks completed successfully for File: {FilePath}", filePath);
        }

        private void AppendResults(InvoiceProcessingContext context, StringBuilder pdftxt, string filePath,
            Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask)
        {
            _logger.Debug("Appending PDF text results for File: {FilePath}", filePath);
            AppendPdfTextResults(context, pdftxt, ripTask, singleColumnTask, sparseTextTask);
            _logger.Debug("Finished appending PDF text results for File: {FilePath}. Total Length: {Length}", filePath, pdftxt.Length);
            _logger.Verbose("Extracted PDF Text (first 500 chars): {PdfText}", pdftxt.ToString().Substring(0, Math.Min(pdftxt.Length, 500)));
        }

        // Removed HandleAggregateException method as its logic is now integrated into the main catch block.
    }
}