using System;
using System.Threading.Tasks;
using pdf_ocr;
using Tesseract;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPdfTextStep
    {
        private static Task<string> GetPdfSparseTextAsync(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context.FilePath; // Null/empty check done in Execute
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(GetPdfSparseTextAsync), "Asynchronously extract sparse text from PDF using PdfOcr", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", nameof(GetPdfSparseTextAsync), $"File: {filePath}"); // Use logger from context
            return Task.Run(() =>
            {
                try
                {
                    var txt = "------------------------------------------SparseText-------------------------\r\n";
                    context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "PdfOcr().Ocr with SparseText", "SYNC_EXPECTED"); // Use logger from context
                    // Assuming PdfOcr().Ocr might throw exceptions
                    var ocrStopwatch = Stopwatch.StartNew();
                    txt += new PdfOcr(context.Logger).Ocr(filePath, PageSegMode.SparseText); // Pass logger
                    ocrStopwatch.Stop();
                    context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result Length: {Length}",
                        "PdfOcr().Ocr with SparseText", ocrStopwatch.ElapsedMilliseconds, "Sync call returned.", txt.Length); // Use logger from context
 
                    methodStopwatch.Stop(); // Stop stopwatch on success
                    context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(GetPdfSparseTextAsync), $"Successfully extracted sparse text from file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                    context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(GetPdfSparseTextAsync), "Successfully extracted sparse text", $"ExtractedTextLength: {txt.Length}", methodStopwatch.ElapsedMilliseconds);

                    return txt;
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop(); // Stop stopwatch before re-throwing
                    context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            nameof(GetPdfSparseTextAsync), "Sparse Text OCR task", methodStopwatch.ElapsedMilliseconds, $"Error during Sparse Text OCR task for File: {filePath}. Error: {ex.Message}");
                    context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetPdfSparseTextAsync), "Asynchronously extract sparse text from PDF using PdfOcr", methodStopwatch.ElapsedMilliseconds, $"Error during Sparse Text OCR task for File: {filePath}. Error: {ex.Message}");
                    // Throw exception to be caught by AggregateException handler in Execute
                    throw; // Re-throw the exception
                }
            });
        }
    }
}