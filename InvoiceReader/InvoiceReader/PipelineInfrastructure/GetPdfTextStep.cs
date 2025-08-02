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
        
        /// <summary>
        /// 🔧 **EXECUTION_MODE_TOGGLE**: Controls whether OCR operations run in parallel or sequential mode
        /// - false: Parallel execution (faster but may cause ThreadAbortException conflicts)
        /// - true: Sequential execution (slower but prevents threading conflicts)
        /// </summary>
        private static readonly bool UseSequentialOcrExecution = true; // **THREADABORT_FIX**: Default to sequential to prevent threading conflicts

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
                    nameof(Execute), "TaskSetup", UseSequentialOcrExecution ? "Setting up sequential text extraction tasks." : "Setting up concurrent text extraction tasks.", $"FilePath: {filePath}, ExecutionMode: {(UseSequentialOcrExecution ? "Sequential" : "Parallel")}");
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

        private void SetupAndRunTasks(InvoiceProcessingContext context, string filePath,
            out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            if (UseSequentialOcrExecution)
            {
                context.Logger?.Information("🔄 **SEQUENTIAL_OCR_MODE**: Using sequential OCR execution to prevent ThreadAbortException conflicts");
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(SetupAndRunTasks), "SequentialSetup", "Setting up sequential PDF text extraction tasks.", $"FilePath: {filePath}", "");
                SetupSequentialPdfTextExtraction(context, filePath, out ripTask, out singleColumnTask, out sparseTextTask);
            }
            else
            {
                context.Logger?.Information("⚡ **PARALLEL_OCR_MODE**: Using parallel OCR execution (may cause ThreadAbortException under load)");
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(SetupAndRunTasks), "ParallelSetup", "Setting up concurrent PDF text extraction tasks.", $"FilePath: {filePath}", "");
                SetupPdfTextExtraction(context, filePath, out ripTask, out singleColumnTask, out sparseTextTask);
            }
            
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(SetupAndRunTasks), "TasksCreated", "Tasks created for Ripped Text, Single Column OCR, Sparse Text OCR.", $"FilePath: {filePath}", "");
        }

        /// <summary>
        /// 🔄 **SEQUENTIAL_OCR_SETUP**: Creates tasks that will execute sequentially instead of parallel
        /// This prevents ThreadAbortException conflicts in OCR processing
        /// </summary>
        private void SetupSequentialPdfTextExtraction(InvoiceProcessingContext context, string filePath,
            out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            context.Logger?.Information("🔄 **SEQUENTIAL_TASK_CREATION**: Creating sequential OCR tasks to prevent threading conflicts");
            
            // Create tasks that will execute one after another
            ripTask = Task.Run(async () =>
            {
                context.Logger?.Information("1️⃣ **SEQUENTIAL_STEP_1**: Starting Ripped Text extraction");
                var result = await GetRippedTextAsync(context).ConfigureAwait(false);
                context.Logger?.Information("✅ **SEQUENTIAL_STEP_1_COMPLETE**: Ripped Text extraction finished");
                return result;
            });
            
            singleColumnTask = ripTask.ContinueWith(async _ =>
            {
                context.Logger?.Information("2️⃣ **SEQUENTIAL_STEP_2**: Starting Single Column OCR (after Ripped Text)");
                var result = await GetSingleColumnPdfText(context).ConfigureAwait(false);
                context.Logger?.Information("✅ **SEQUENTIAL_STEP_2_COMPLETE**: Single Column OCR finished");
                return result;
            }).Unwrap();
            
            sparseTextTask = singleColumnTask.ContinueWith(async _ =>
            {
                context.Logger?.Information("3️⃣ **SEQUENTIAL_STEP_3**: Starting Sparse Text OCR (after Single Column)");
                var result = await GetPdfSparseTextAsync(context).ConfigureAwait(false);
                context.Logger?.Information("✅ **SEQUENTIAL_STEP_3_COMPLETE**: Sparse Text OCR finished");
                return result;
            }).Unwrap();
            
            context.Logger?.Information("🔄 **SEQUENTIAL_CHAIN_CREATED**: Created sequential task chain - RipText → SingleColumn → SparseText");
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
                    context.Logger?.Information("🕐 **OCR_TIMEOUT_PROTECTION**: Using {TimeoutMinutes}min timeout with {ExecutionMode} execution mode", timeoutMinutes, UseSequentialOcrExecution ? "SEQUENTIAL" : "PARALLEL");
                    
                    // Wait for all tasks with timeout protection
                    var allTasks = Task.WhenAll(ripTask, singleColumnTask, sparseTextTask);
                    await allTasks.ConfigureAwait(false);
                    
                    context.Logger?.Information("✅ **OCR_TASKS_COMPLETED**: All PDF text extraction tasks completed successfully within timeout");
                }
            }
            catch (System.Threading.ThreadAbortException threadAbortEx)
            {
                var diagnosisStopwatch = Stopwatch.StartNew();
                context.Logger?.Error(threadAbortEx, "🚨 **THREADABORT_DETECTED**: ThreadAbortException caught during PDF OCR processing - starting comprehensive diagnosis");
                
                // **COMPREHENSIVE_DIAGNOSIS**: Capture complete system state
                try
                {
                    // 1. **DEEP_STACK_TRACE_ANALYSIS**
                    context.Logger?.Error("🔍 **DEEP_STACK_TRACE**: Full ThreadAbortException details:");
                    context.Logger?.Error("   📍 **ABORT_SOURCE**: {Source}", threadAbortEx.Source ?? "Unknown");
                    context.Logger?.Error("   📍 **ABORT_MESSAGE**: {Message}", threadAbortEx.Message);
                    context.Logger?.Error("   📍 **ABORT_TARGETSITE**: {TargetSite}", threadAbortEx.TargetSite?.ToString() ?? "Unknown");
                    context.Logger?.Error("   📍 **ABORT_HELPLINK**: {HelpLink}", threadAbortEx.HelpLink ?? "None");
                    context.Logger?.Error("   📍 **FULL_STACKTRACE**: {StackTrace}", threadAbortEx.StackTrace ?? "No StackTrace Available");
                    
                    // 2. **THREAD_STATE_DIAGNOSIS**
                    var currentThread = System.Threading.Thread.CurrentThread;
                    context.Logger?.Error("🧵 **THREAD_STATE_ANALYSIS**:");
                    context.Logger?.Error("   🆔 **THREAD_ID**: {ThreadId}", currentThread.ManagedThreadId);
                    context.Logger?.Error("   📛 **THREAD_NAME**: {ThreadName}", currentThread.Name ?? "Unnamed");
                    context.Logger?.Error("   🏃 **THREAD_STATE**: {ThreadState}", currentThread.ThreadState);
                    context.Logger?.Error("   🔥 **IS_BACKGROUND**: {IsBackground}", currentThread.IsBackground);
                    context.Logger?.Error("   🎯 **IS_THREADPOOL**: {IsThreadPool}", currentThread.IsThreadPoolThread);
                    context.Logger?.Error("   ⏰ **ABORT_REQUESTED**: {AbortRequested}", currentThread.ThreadState.HasFlag(System.Threading.ThreadState.AbortRequested));
                    
                    // 3. **DETAILED_TASK_ANALYSIS**
                    context.Logger?.Error("📊 **COMPREHENSIVE_TASK_ANALYSIS**:");
                    LogTaskDiagnostics(context, "RIP_TASK", ripTask);
                    LogTaskDiagnostics(context, "SINGLE_COLUMN_TASK", singleColumnTask);  
                    LogTaskDiagnostics(context, "SPARSE_TEXT_TASK", sparseTextTask);
                    
                    // 4. **EXECUTION_CONTEXT_DIAGNOSIS**
                    context.Logger?.Error("🏗️ **EXECUTION_CONTEXT**:");
                    context.Logger?.Error("   📁 **FILE_PATH**: {FilePath}", filePath);
                    context.Logger?.Error("   📄 **FILE_EXISTS**: {FileExists}", System.IO.File.Exists(filePath));
                    context.Logger?.Error("   📏 **FILE_SIZE**: {FileSize} bytes", System.IO.File.Exists(filePath) ? new System.IO.FileInfo(filePath).Length : -1);
                    context.Logger?.Error("   🔧 **EXECUTION_MODE**: {ExecutionMode}", UseSequentialOcrExecution ? "SEQUENTIAL" : "PARALLEL");
                    
                    // 5. **SYSTEM_RESOURCE_ANALYSIS**
                    context.Logger?.Error("💾 **SYSTEM_RESOURCE_STATE**:");
                    context.Logger?.Error("   🧠 **WORKING_SET**: {WorkingSet} MB", System.Environment.WorkingSet / 1024 / 1024);
                    context.Logger?.Error("   🏭 **PROCESSOR_COUNT**: {ProcessorCount}", System.Environment.ProcessorCount);
                    context.Logger?.Error("   📊 **THREAD_COUNT**: {ThreadCount}", System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
                    context.Logger?.Error("   ⏱️ **UPTIME**: {Uptime} ms", System.Environment.TickCount);
                    
                    // 6. **TIMING_ANALYSIS**
                    var elapsedTime = DateTime.UtcNow.Subtract(DateTime.UtcNow.AddMilliseconds(-System.Environment.TickCount));
                    context.Logger?.Error("⏰ **TIMING_CONTEXT**:");
                    context.Logger?.Error("   🕐 **ABORT_TIME**: {AbortTime:yyyy-MM-dd HH:mm:ss.fff} UTC", DateTime.UtcNow);
                    context.Logger?.Error("   ⏱️ **PROCESS_RUNTIME**: {ProcessRuntime} ms", System.Environment.TickCount);
                    context.Logger?.Error("   🔄 **OCR_PHASE**: AwaitTasksCompletion");
                    
                    // 7. **INNER_EXCEPTION_ANALYSIS**
                    if (threadAbortEx.InnerException != null)
                    {
                        context.Logger?.Error("🔗 **INNER_EXCEPTION_ANALYSIS**:");
                        context.Logger?.Error("   📝 **INNER_TYPE**: {InnerType}", threadAbortEx.InnerException.GetType().FullName);
                        context.Logger?.Error("   💬 **INNER_MESSAGE**: {InnerMessage}", threadAbortEx.InnerException.Message);
                        context.Logger?.Error("   📍 **INNER_STACKTRACE**: {InnerStackTrace}", threadAbortEx.InnerException.StackTrace ?? "No Inner StackTrace");
                    }
                    
                }
                catch (Exception diagEx)
                {
                    context.Logger?.Error(diagEx, "❌ **DIAGNOSIS_ERROR**: Exception during ThreadAbortException diagnosis");
                }
                finally
                {
                    diagnosisStopwatch.Stop();
                    context.Logger?.Information("🔍 **DIAGNOSIS_COMPLETE**: ThreadAbortException diagnosis completed in {DiagnosisTime}ms", diagnosisStopwatch.ElapsedMilliseconds);
                }
                
                // **CRITICAL**: Reset thread abort to prevent automatic re-throw
                System.Threading.Thread.ResetAbort();
                context.Logger?.Information("✅ **THREADABORT_RESET**: Thread abort reset successfully for main OCR coordination");
                
                // **RECOVERY_STRATEGY_LOGGING**
                context.Logger?.Warning("🔄 **RECOVERY_STRATEGY**: Implementing graceful recovery with partial OCR results");
                context.Logger?.Warning("   📋 **RECOVERY_ACTION**: Continuing execution with available task results");
                context.Logger?.Warning("   🛡️ **SAFETY_MEASURE**: ThreadAbort has been reset to prevent cascading failures");
                context.Logger?.Warning("   📊 **EXPECTED_BEHAVIOR**: AppendResults will handle null/failed tasks gracefully");
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

        /// <summary>
        /// 🔍 **TASK_DIAGNOSTICS_HELPER**: Provides detailed diagnostic information for individual OCR tasks
        /// Used during ThreadAbortException analysis to understand task states and completion status
        /// </summary>
        private void LogTaskDiagnostics(InvoiceProcessingContext context, string taskName, Task<string> task)
        {
            try
            {
                context.Logger?.Error("   🔍 **{TaskName}_ANALYSIS**:", taskName);
                context.Logger?.Error("     📊 **STATUS**: {TaskStatus}", task?.Status.ToString() ?? "NULL_TASK");
                context.Logger?.Error("     🏁 **COMPLETED**: {IsCompleted}", task?.IsCompleted ?? false);
                context.Logger?.Error("     ❌ **FAULTED**: {IsFaulted}", task?.IsFaulted ?? false);
                context.Logger?.Error("     🚫 **CANCELED**: {IsCanceled}", task?.IsCanceled ?? false);
                context.Logger?.Error("     🆔 **TASK_ID**: {TaskId}", task?.Id ?? -1);
                
                if (task?.Exception != null)
                {
                    context.Logger?.Error("     💥 **TASK_EXCEPTION**: {TaskException}", task.Exception.GetBaseException()?.Message ?? "Unknown Exception");
                    context.Logger?.Error("     📍 **EXCEPTION_TYPE**: {ExceptionType}", task.Exception.GetBaseException()?.GetType().FullName ?? "Unknown Type");
                }
                
                if (task?.IsCompleted == true && !task.IsFaulted && !task.IsCanceled)
                {
                    var resultLength = task.Result?.Length ?? 0;
                    context.Logger?.Error("     ✅ **RESULT_LENGTH**: {ResultLength} characters", resultLength);
                    if (resultLength > 0)
                    {
                        var preview = task.Result.Length > 100 ? task.Result.Substring(0, 100) + "..." : task.Result;
                        context.Logger?.Error("     📝 **RESULT_PREVIEW**: {ResultPreview}", preview);
                    }
                }
            }
            catch (Exception diagEx)
            {
                context.Logger?.Error(diagEx, "❌ **TASK_DIAGNOSTIC_ERROR**: Failed to analyze task {TaskName}", taskName);
            }
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