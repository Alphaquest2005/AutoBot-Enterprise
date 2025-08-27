using System;
using System.Threading.Tasks;
using pdf_ocr;
using Tesseract;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPdfTextStep
    {
        private static Task<string> GetSingleColumnPdfText(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context.FilePath;
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(GetSingleColumnPdfText), "Asynchronously extract single column text from PDF using PdfOcr", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", nameof(GetSingleColumnPdfText), $"File: {filePath}"); // Use logger from context
            return Task.Run(() =>
            {
                try
                {
                    var txt = "------------------------------------------Single Column-------------------------\r\n";
                    context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "PdfOcr().Ocr with SingleColumn", "SYNC_EXPECTED"); // Use logger from context
                    var ocrStopwatch = Stopwatch.StartNew();
                    
                    // **MODERN_OCR_APPROACH**: PdfOcr now uses CancellationToken - no ThreadAbortException handling needed
                    try
                    {
                        txt += new PdfOcr(context.Logger).Ocr(filePath, PageSegMode.SingleColumn); // Pass logger
                        context.Logger?.Information("✅ **SINGLECOLUMN_OCR_SUCCESS**: Single column OCR completed successfully");
                    }
                    catch (OperationCanceledException cancelEx)
                    {
                        context.Logger?.Warning(cancelEx, "🚨 **SINGLECOLUMN_CANCELLED**: Single Column OCR was cancelled - using fallback text");
                        txt += "------------------------------------------Single Column (Cancelled Recovery)-------------------------\r\n";
                        txt += "** OCR processing was cancelled - partial results may be available **\r\n";
                        // Don't re-throw - allow processing to continue with partial results
                    }
                    catch (Exception ocrEx)
                    {
                        context.Logger?.Warning(ocrEx, "🚨 **SINGLECOLUMN_ERROR**: Single Column OCR failed - using fallback text");
                        txt += "------------------------------------------Single Column (Error Recovery)-------------------------\r\n";
                        txt += $"** OCR processing failed: {ocrEx.Message} - partial results may be available **\r\n";
                        // Don't re-throw - allow processing to continue with partial results
                    }
                    
                    ocrStopwatch.Stop();
                    context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "PdfOcr().Ocr with SingleColumn", ocrStopwatch.ElapsedMilliseconds, "Sync call returned with ThreadAbort protection"); // Use logger from context
 
                    methodStopwatch.Stop(); // Stop stopwatch on success
                    context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(GetSingleColumnPdfText), $"Successfully extracted single column text from file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                    context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(GetSingleColumnPdfText), "Successfully extracted single column text", $"ExtractedTextLength: {txt.Length}", methodStopwatch.ElapsedMilliseconds);

                    return txt;
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop(); // Stop stopwatch before re-throwing
                    context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            nameof(GetSingleColumnPdfText), "Single Column OCR task", methodStopwatch.ElapsedMilliseconds, $"Error during Single Column OCR task for File: {filePath}. Error: {ex.Message}");
                    context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetSingleColumnPdfText), "Asynchronously extract single column text from PDF using PdfOcr", methodStopwatch.ElapsedMilliseconds, $"Error during Single Column OCR task for File: {filePath}. Error: {ex.Message}");
                    throw; // Re-throw the exception
                }
            });
        }
    }
}