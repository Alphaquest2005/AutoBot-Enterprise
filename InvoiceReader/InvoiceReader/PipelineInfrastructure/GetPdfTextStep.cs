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
    public class GetPdfTextStep : IPipelineStep<InvoiceProcessingContext>
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
                 return false; // Indicate failure
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                 _logger.Error(ex, "Unexpected error during GetPdfTextStep for File: {FilePath}", filePath);
                 return false; // Indicate failure
            }
        }

        private static void SetupPdfTextExtraction(InvoiceProcessingContext context, out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            // Logging happens within the async methods themselves
            ripTask = GetRippedTextAsync(context);
            singleColumnTask = GetSingleColumnPdfText(context);
            sparseTextTask = GetPdfSparseTextAsync(context);
        }

        // Added optional flag to log errors during append if tasks might have failed
        private static void AppendPdfTextResults(InvoiceProcessingContext context, StringBuilder pdftxt, Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask, bool logErrors = false)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Appending PDF text results. LogErrors: {LogErrors}", logErrors);

            // Helper to safely get result or log error
            Action<Task<string>, string> appendResult = (task, taskName) => {
                string result = string.Empty;
                try
                {
                    if (task != null)
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            result = task.Result;
                            _logger.Verbose("Successfully retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}", taskName, filePath, result.Length);
                        }
                        else if (task.IsFaulted && logErrors)
                        {
                            // Log the specific error from the faulted task
                            _logger.Error(task.Exception?.GetBaseException(), "Error recorded in faulted {TaskName} task for File: {FilePath}", taskName, filePath);
                            result = $"<{taskName} Task Failed - See Logs>"; // Placeholder in output
                        }
                        else if (logErrors) // Task might be null or in another state
                        {
                             _logger.Warning("{TaskName} task was null or not completed successfully when appending results for File: {FilePath}. Status: {Status}", taskName, filePath, task?.Status.ToString() ?? "Null");
                             result = $"<{taskName} Task Not Completed Successfully>"; // Placeholder
                        }
                        // If !logErrors, we assume Task.WhenAll succeeded, so access Result directly (original behavior)
                        else if (task != null)
                        {
                             result = task.Result; // May throw if task faulted, but shouldn't happen if !logErrors
                             _logger.Verbose("Retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}", taskName, filePath, result.Length);
                        }
                    }
                    else if (logErrors)
                    {
                        _logger.Warning("{TaskName} task was null when attempting to append results for File: {FilePath}", taskName, filePath);
                        result = $"<{taskName} Task Was Null>"; // Placeholder
                    }
                }
                catch (Exception ex) when (logErrors) // Catch unexpected errors during Result access if logging errors
                {
                    _logger.Error(ex, "Unexpected error retrieving result from {TaskName} task for File: {FilePath}", taskName, filePath);
                    result = $"<Error Retrieving {taskName} Result - See Logs>"; // Placeholder
                }
                pdftxt.AppendLine(result);
            };

            // Append results safely
            appendResult(singleColumnTask, "Single Column OCR");
            appendResult(sparseTextTask, "Sparse Text OCR");
            appendResult(ripTask, "Ripped Text (PdfPig)");

            context.PdfText = pdftxt;
             _logger.Debug("Final PdfText assigned to context for File: {FilePath}. Total Length: {Length}", filePath, context.PdfText.Length);
        }


        private static Task<string> GetPdfSparseTextAsync(InvoiceProcessingContext context)
        {
            string filePath = context.FilePath; // Null/empty check done in Execute
            _logger.Debug("Starting Sparse Text OCR task for File: {FilePath}", filePath);
            return Task.Run(() =>
            {
                try
                {
                    var txt = "------------------------------------------SparseText-------------------------\r\n";
                    _logger.Verbose("Executing PdfOcr().Ocr with SparseText for File: {FilePath}", filePath);
                    // Assuming PdfOcr().Ocr might throw exceptions
                    txt += new PdfOcr().Ocr(filePath, PageSegMode.SparseText);
                    _logger.Information("Sparse Text OCR task completed successfully for File: {FilePath}. Result Length: {Length}", filePath, txt.Length);
                    return txt;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during Sparse Text OCR task for File: {FilePath}", filePath);
                    // Throw exception to be caught by AggregateException handler in Execute
                    throw; // Re-throw the exception
                }
            });
        }

        private static Task<string> GetSingleColumnPdfText(InvoiceProcessingContext context)
        {
             string filePath = context.FilePath;
             _logger.Debug("Starting Single Column OCR task for File: {FilePath}", filePath);
            return Task.Run(() =>
            {
                 try
                 {
                    var txt = "------------------------------------------Single Column-------------------------\r\n";
                    _logger.Verbose("Executing PdfOcr().Ocr with SingleColumn for File: {FilePath}", filePath);
                    txt += new PdfOcr().Ocr(filePath, PageSegMode.SingleColumn);
                    _logger.Information("Single Column OCR task completed successfully for File: {FilePath}. Result Length: {Length}", filePath, txt.Length);
                    return txt;
                 }
                 catch (Exception ex)
                 {
                     _logger.Error(ex, "Error during Single Column OCR task for File: {FilePath}", filePath);
                     throw; // Re-throw the exception
                 }
            });
        }

        private static Task<string> GetRippedTextAsync(InvoiceProcessingContext context)
        {
             string filePath = context.FilePath;
             _logger.Debug("Starting Ripped Text (PdfPig) task for File: {FilePath}", filePath);
            var ripTask = Task.Run(() =>
            {
                // PdfPigText handles its own try/catch and logging, returns error string on failure
                var txt = "------------------------------------------Ripped Text-------------------------\r\n";
                _logger.Verbose("Calling PdfPigText for File: {FilePath}", filePath);
                string rippedResult = PdfPigText(filePath);
                txt += rippedResult;

                // Check if PdfPigText returned an error message and throw if it did
                if (rippedResult.StartsWith("Error reading Ripped Text (PdfPig):", StringComparison.Ordinal))
                {
                    _logger.Warning("Ripped Text (PdfPig) task failed internally for File: {FilePath}. Throwing exception.", filePath);
                    // Throw an exception so Task.WhenAll catches it in AggregateException
                    throw new Exception($"PdfPig text extraction failed for {filePath}. See previous logs.");
                }
                 _logger.Information("Ripped Text (PdfPig) task completed successfully for File: {FilePath}. Result Length: {Length}", filePath, rippedResult.Length);
                return txt;
            });
            return ripTask;
        }

        private static string PdfPigText(string file)
        {
             _logger.Debug("Extracting text using PdfPig for File: {FilePath}", file);
            try
            {
                var sb = new StringBuilder();
                using (var pdf = PdfDocument.Open(file))
                {
                     _logger.Verbose("Opened PDF document with {PageCount} pages for File: {FilePath}", pdf.NumberOfPages, file);
                    foreach (var page in pdf.GetPages())
                    {
                         _logger.Verbose("Extracting text from Page {PageNumber} using ContentOrderTextExtractor for File: {FilePath}", page.Number, file);
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                    }
                }
                string result = sb.ToString();
                 _logger.Verbose("PdfPig text extraction completed successfully for File: {FilePath}. Result Length: {Length}", file, result.Length); // Changed level to Verbose
                return result;
            }
            catch (Exception ex) // Catch specific exceptions if possible (e.g., PdfDocumentInvalidPasswordException)
            {
                 _logger.Error(ex, "Error during PdfPig text extraction for File: {FilePath}", file);
                 // Return specific error message instead of throwing; caller will check and throw.
                 return $"Error reading Ripped Text (PdfPig): {ex.Message}";
            }
        }
    }
}