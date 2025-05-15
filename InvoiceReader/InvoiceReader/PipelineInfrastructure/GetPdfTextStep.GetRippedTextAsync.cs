using System;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPdfTextStep
    {
        private static Task<string> GetRippedTextAsync(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context.FilePath;
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(GetRippedTextAsync), "Asynchronously extract ripped text from PDF using PdfPig", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", nameof(GetRippedTextAsync), $"File: {filePath}"); // Use logger from context
            var ripTask = Task.Run(() =>
            {
                // PdfPigText handles its own try/catch and logging, returns error string on failure
                var txt = "------------------------------------------Ripped Text-------------------------\r\n";
                context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "PdfPigText Extraction", "SYNC_EXPECTED"); // Use logger from context
                var pdfPigStopwatch = Stopwatch.StartNew();
                string rippedResult = PdfPigText(context, filePath); // Pass context to PdfPigText
                pdfPigStopwatch.Stop();
                 context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "PdfPigText Extraction", pdfPigStopwatch.ElapsedMilliseconds, "Sync call returned"); // Use logger from context
                txt += rippedResult;
 
                // Check if PdfPigText returned an error message and throw if it did
                if (rippedResult.StartsWith("Error reading Ripped Text (PdfPig):", StringComparison.Ordinal))
                {
                    methodStopwatch.Stop(); // Stop stopwatch before throwing
                    context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         nameof(GetRippedTextAsync), "PdfPig text extraction", methodStopwatch.ElapsedMilliseconds, $"Ripped Text (PdfPig) task failed internally for File: {filePath}.");
                     context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(GetRippedTextAsync), "Asynchronously extract ripped text from PDF using PdfPig", methodStopwatch.ElapsedMilliseconds, $"PdfPig text extraction failed for {filePath}. See previous logs.");
                    // Throw an exception so Task.WhenAll catches it in AggregateException
                    throw new Exception($"PdfPig text extraction failed for {filePath}. See previous logs.");
                }
 
                methodStopwatch.Stop(); // Stop stopwatch on success
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(GetRippedTextAsync), $"Successfully extracted ripped text from file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(GetRippedTextAsync), "Successfully extracted ripped text", $"ExtractedTextLength: {txt.Length}", methodStopwatch.ElapsedMilliseconds);

                return txt;
            });
            return ripTask;
        }
    }
}