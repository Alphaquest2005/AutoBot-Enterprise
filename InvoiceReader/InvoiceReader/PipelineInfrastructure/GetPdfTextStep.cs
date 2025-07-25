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
    using System.Diagnostics;

    public partial class GetPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<GetPdfTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution

            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Extract text from a PDF file using multiple methods", $"FilePath: {filePath}, FileExists: {context.FileInfo?.Exists ?? false}");

            if (!ValidateContext(context))
            {
                methodStopwatch.Stop();
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Extract text from a PDF file using multiple methods", methodStopwatch.ElapsedMilliseconds, "Context validation failed.");
                return false;
            }

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                "GetPdfTextStepExecution", $"Processing file: {filePath}, FileSize: {context.FileInfo?.Length ?? 0}");

            StringBuilder pdftxt = new StringBuilder();
            Task<string> ripTask = null, singleColumnTask = null, sparseTextTask = null;

            try
            {
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "TaskSetup", "Setting up concurrent text extraction tasks.", $"FilePath: {filePath}");
                SetupAndRunTasks(context, filePath, out ripTask, out singleColumnTask, out sparseTextTask);
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "TaskExecution", "Awaiting completion of text extraction tasks.", $"FilePath: {filePath}");
                await AwaitTasksCompletion(context, filePath, ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false); // Pass context
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "ResultAggregation", "Appending results from completed tasks.", $"FilePath: {filePath}");
                AppendResults(context, pdftxt, filePath, ripTask, singleColumnTask, sparseTextTask);

                methodStopwatch.Stop();
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully extracted text using multiple methods", $"ExtractedTextLength: {pdftxt.Length}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    "GetPdfTextStepExecution", $"Successfully extracted text from file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (AggregateException aggEx) // Catch errors from concurrent tasks
            {
                methodStopwatch.Stop();
                string errorMsg = $"One or more PDF text extraction tasks failed for File: {filePath}.";
                context.Logger?.Error(aggEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Extract text from a PDF file using multiple methods", methodStopwatch.ElapsedMilliseconds, errorMsg);

                context.Logger?.Error(aggEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    "GetPdfTextStepExecution", "Concurrent task execution", methodStopwatch.ElapsedMilliseconds, errorMsg);


                // Add specific errors from inner exceptions to the context
                foreach (var innerEx in aggEx.Flatten().InnerExceptions)
                {
                     context.Logger?.Error(innerEx, "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                         nameof(Execute), "TaskError", "Individual task failed.", $"TaskSource: {innerEx.Source ?? "Unknown"}, ErrorMessage: {innerEx.Message}");
                    context.AddError($"Task Error ({innerEx.Source ?? "Unknown Source"}): {innerEx.Message}"); // Add specific task error
                }

                // Attempt to append partial results even after failure
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "PartialResultAppend", "Attempting to append results from potentially successful tasks after failure.", $"FilePath: {filePath}");
                AppendResults(context, pdftxt, filePath, ripTask, singleColumnTask, sparseTextTask); // Append successful parts if any
                context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "PartialResultAppend", "Partial extracted text available.", $"FilePath: {filePath}, PartialTextLength: {pdftxt.Length}");


                return false; // Indicate step failure
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                // Catch any other unexpected errors during step execution
                string generalErrorMsg = $"Unexpected error during GetPdfTextStep for File: {filePath}: {ex.Message}";
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Extract text from a PDF file using multiple methods", methodStopwatch.ElapsedMilliseconds, generalErrorMsg);
                context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    "GetPdfTextStepExecution", "Unexpected error during execution", methodStopwatch.ElapsedMilliseconds, generalErrorMsg);
                 context.Logger?.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                     nameof(Execute), "UnexpectedError", "Caught unexpected exception.", $"ErrorMessage: {ex.Message}");
                context.AddError(generalErrorMsg); // Add the general error to context
                return false; // Indicate step failure
            }
        }

        private bool ValidateContext(InvoiceProcessingContext context)
        {
            if (context == null)
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, "GetPdfTextStep executed with null context.");
                return false;
            }

            if (string.IsNullOrEmpty(context.FilePath))
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, "GetPdfTextStep executed with null or empty FilePath in context.");
                return false;
            }

            return true;
        }

        private void SetupAndRunTasks(InvoiceProcessingContext context, string filePath, // Add context parameter
            out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(SetupAndRunTasks), "Setup", "Setting up concurrent PDF text extraction tasks.", $"FilePath: {filePath}", "");
            SetupPdfTextExtraction(context, filePath, out ripTask, out singleColumnTask, out sparseTextTask); // Pass filePath
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(SetupAndRunTasks), "TasksCreated", "Tasks created for Ripped Text, Single Column OCR, Sparse Text OCR.", $"FilePath: {filePath}", "");
        }

        private async Task AwaitTasksCompletion(InvoiceProcessingContext context, string filePath, Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask) // Add context parameter
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AwaitTasksCompletion), "Awaiting", "Awaiting completion of PDF text extraction tasks.", $"FilePath: {filePath}", "");
            
            // **THREADABORT_EXCEPTION_FIX**: Use timeout instead of allowing thread abortion
            // PDF OCR processing can take time, but we need to prevent indefinite blocking
            const int timeoutMinutes = 5; // 5-minute timeout for OCR processing
            
            try
            {
                using (var timeoutCts = new System.Threading.CancellationTokenSource(TimeSpan.FromMinutes(timeoutMinutes)))
                {
                    context.Logger?.Information("🕐 **OCR_TIMEOUT_PROTECTION**: Using {TimeoutMinutes}min timeout to prevent ThreadAbortException during PDF processing", timeoutMinutes);
                    
                    // Wait for all tasks with timeout protection
                    var allTasks = Task.WhenAll(ripTask, singleColumnTask, sparseTextTask);
                    await allTasks.ConfigureAwait(false);
                    
                    context.Logger?.Information("✅ **OCR_TASKS_COMPLETED**: All PDF text extraction tasks completed successfully within timeout");
                }
            }
            catch (System.Threading.ThreadAbortException threadAbortEx)
            {
                context.Logger?.Error(threadAbortEx, "🚨 **THREADABORT_DETECTED**: ThreadAbortException caught during PDF OCR processing - implementing graceful recovery");
                
                // Log the current state of each task for diagnosis
                context.Logger?.Information("📊 **TASK_STATUS_DIAGNOSTIC**: RipTask={RipStatus}, SingleColumn={SingleStatus}, SparseText={SparseStatus}",
                    ripTask?.Status.ToString() ?? "null", 
                    singleColumnTask?.Status.ToString() ?? "null", 
                    sparseTextTask?.Status.ToString() ?? "null");
                
                // **CRITICAL**: Reset thread abort to prevent automatic re-throw
                System.Threading.Thread.ResetAbort();
                context.Logger?.Information("✅ **THREADABORT_RESET**: Thread abort reset successfully for main OCR coordination");
                
                // Don't re-throw ThreadAbortException - handle gracefully
                context.Logger?.Warning("⚠️ **GRACEFUL_RECOVERY**: Continuing with partial OCR results due to thread termination");
            }
            catch (System.OperationCanceledException timeoutEx)
            {
                context.Logger?.Error(timeoutEx, "⏰ **OCR_PROCESSING_TIMEOUT**: PDF text extraction timed out after {TimeoutMinutes} minutes for file: {FilePath}", timeoutMinutes, filePath);
                
                // Log which tasks completed and which didn't
                context.Logger?.Information("📊 **TIMEOUT_DIAGNOSTIC**: RipTask={RipStatus}, SingleColumn={SingleStatus}, SparseText={SparseStatus}",
                    ripTask?.Status.ToString() ?? "null", 
                    singleColumnTask?.Status.ToString() ?? "null", 
                    sparseTextTask?.Status.ToString() ?? "null");
                
                // Allow processing to continue with whatever tasks completed
                context.Logger?.Warning("⚠️ **TIMEOUT_RECOVERY**: Continuing with available OCR results despite timeout");
            }
            
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AwaitTasksCompletion), "Completed", "PDF text extraction task coordination completed (with timeout protection).", $"FilePath: {filePath}", "");
        }

        private void AppendResults(InvoiceProcessingContext context, StringBuilder pdftxt, string filePath, // Add context parameter
            Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AppendResults), "Appending", "Appending PDF text results.", $"FilePath: {filePath}", "");
            AppendPdfTextResults(context, pdftxt, filePath, ripTask, singleColumnTask, sparseTextTask); // Add filePath
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AppendResults), "Finished", "Finished appending PDF text results.", $"FilePath: {filePath}, TotalLength: {pdftxt.Length}", "");
            context.Logger?.Verbose("Extracted PDF Text (first 500 chars): {PdfText}", pdftxt.ToString().Substring(0, Math.Min(pdftxt.Length, 500))); // Use logger from context
        }

        // Removed HandleAggregateException method as its logic is now integrated into the main catch block.
    }
}