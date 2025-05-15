using System.IO; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class WriteFormattedTextFileStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<WriteFormattedTextFileStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Write formatted PDF text to a file", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(WriteFormattedTextFileStep), $"Writing formatted text file for file: {filePath}");

            // Null check context first
            if (context == null)
            {
                 // Cannot use context.Logger if context is null
                Log.ForContext<WriteFormattedTextFileStep>().Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Write formatted PDF text to a file", 0, "WriteFormattedTextFileStep executed with null context.");
                Log.ForContext<WriteFormattedTextFileStep>().Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(WriteFormattedTextFileStep), "Context validation", 0, "WriteFormattedTextFileStep executed with null context.");
                 return false;
            }

            // Corrected logic: Check if data is PRESENT
            if (!IsContextDataPresent(context.Logger, context)) // Handles its own logging, pass logger
            {
                 string errorMsg = $"WriteFormattedTextFileStep cannot proceed due to missing required data (FilePath or PdfText) for File: {filePath}";
                 // Logging is handled within IsContextDataPresent
                 context.AddError(errorMsg); // Add error to context
                 methodStopwatch.Stop(); // Stop stopwatch on validation failure
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Write formatted PDF text to a file", methodStopwatch.ElapsedMilliseconds, "Context data missing.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(WriteFormattedTextFileStep), "Context validation", methodStopwatch.ElapsedMilliseconds, "Context data missing.");
                 return false; // Step fails if required data is missing
            }

            string textFilePath = null; // Initialize outside try block
            try
            {
                context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FilePathCreation", "Creating formatted text file path.", $"FilePath: {filePath}", "");
                textFilePath = CreateFormattedTextFilePath(context.Logger, context); // Renamed for clarity, pass logger
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FilePathCreation", "Target text file path created.", $"TxtFilePath: {textFilePath}", "");

                context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FileWriting", "Writing formatted text to file.", $"TxtFilePath: {textFilePath}, TextLength: {context.PdfText?.Length ?? 0}", "");

                context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"File.WriteAllText to {textFilePath}", "ASYNC_EXPECTED"); // Log before file write
                var fileWriteStopwatch = Stopwatch.StartNew(); // Start stopwatch
                // Wrap synchronous file IO in Task.Run to avoid blocking async pipeline thread
                // Use WriteAllTextAsync for true async operation if targeting .NET Core/.NET 5+
                // For now, using Task.Run with WriteAllText for compatibility.
                await Task.Run(() => File.WriteAllText(textFilePath, context.PdfText.ToString())).ConfigureAwait(false);
                fileWriteStopwatch.Stop(); // Stop stopwatch
                context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"File.WriteAllText to {textFilePath}", fileWriteStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after file write

                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FileWriting", "Successfully wrote formatted text to file.", $"TxtFilePath: {textFilePath}", "");

                context.TextFilePath = textFilePath; // Set TextFilePath in context
                context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "ContextUpdate", "Set context.TextFilePath.", $"TxtFilePath: {textFilePath}", "");

                methodStopwatch.Stop(); // Stop stopwatch on success
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully wrote formatted text file", $"TxtFilePath: {textFilePath}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(WriteFormattedTextFileStep), $"Successfully wrote formatted text file: {textFilePath} for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return true; // Indicate success
            }
            catch (UnauthorizedAccessException uaEx) // Specific exception
            {
                 methodStopwatch.Stop(); // Stop stopwatch on exception
                 string errorMsg = $"Unauthorized access error writing formatted text file: {textFilePath ?? filePath + ".txt"}";
                 context.Logger?.Error(uaEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Write formatted PDF text to a file", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {uaEx.Message}");
                 context.Logger?.Error(uaEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(WriteFormattedTextFileStep), "File writing", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {uaEx.Message}");
                 context.AddError($"{errorMsg}: {uaEx.Message}"); // Add error to context
                 return false;
            }
            catch (DirectoryNotFoundException dnfEx) // Specific exception
            {
                 methodStopwatch.Stop(); // Stop stopwatch on exception
                 string errorMsg = $"Directory not found error writing formatted text file: {textFilePath ?? filePath + ".txt"}";
                 context.Logger?.Error(dnfEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Write formatted PDF text to a file", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {dnfEx.Message}");
                 context.Logger?.Error(dnfEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(WriteFormattedTextFileStep), "File writing", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {dnfEx.Message}");
                 context.AddError($"{errorMsg}: {dnfEx.Message}"); // Add error to context
                 return false;
            }
            catch (IOException ioEx) // Catch specific IO errors
            {
                 methodStopwatch.Stop(); // Stop stopwatch on exception
                 string errorMsg = $"IO Error writing formatted text file: {textFilePath ?? filePath + ".txt"}";
                 context.Logger?.Error(ioEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Write formatted PDF text to a file", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {ioEx.Message}");
                 context.Logger?.Error(ioEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(WriteFormattedTextFileStep), "File writing", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {ioEx.Message}");
                 context.AddError($"{errorMsg}: {ioEx.Message}"); // Add error to context
                 return false;
            }
            catch (Exception ex) // Catch other potential errors
            {
                 methodStopwatch.Stop(); // Stop stopwatch on exception
                 string errorMsg = $"Unexpected error during WriteFormattedTextFileStep for File: {filePath}";
                 context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Write formatted PDF text to a file", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {ex.Message}");
                 context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(WriteFormattedTextFileStep), "Unexpected error", methodStopwatch.ElapsedMilliseconds, $"{errorMsg}: {ex.Message}");
                 context.AddError($"{errorMsg}: {ex.Message}"); // Add error to context
                 return false; // Indicate failure
            }
        }

        // Renamed and corrected logic: Returns true if data is PRESENT
        private static bool IsContextDataPresent(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
             logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsContextDataPresent), "Validation", "Checking if required data is present for writing formatted text file.", "", "");
             // Context null check happens in Execute
             if (string.IsNullOrEmpty(context.FilePath))
             {
                 logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(IsContextDataPresent), "Validation", "Required data missing: FilePath is null or empty.", "", "");
                 return false;
             }
             // FormattedPdfText can be empty, but not null
             // Check if PdfText StringBuilder is null or empty
             if (context.PdfText == null || context.PdfText.Length == 0)
             {
                 logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(IsContextDataPresent), "Validation", "Required data missing: PdfText (StringBuilder) is null or empty.", $"FilePath: {context.FilePath}", "");
                 return false;
             }

             logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsContextDataPresent), "Validation", "Required data (FilePath, PdfText) is present.", $"FilePath: {context.FilePath}", "");
             return true; // All required data is present
        }

        // Renamed for clarity - only creates the path string
        private static string CreateFormattedTextFilePath(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
            // Logic from the original WriteTextFile method
            // FilePath null/empty check happens in IsContextDataPresent
            var txtFile = context.FilePath + ".txt";
             logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(CreateFormattedTextFilePath), "Creation", "Generated text file path.", $"TxtFilePath: {txtFile}", "");
            //if (File.Exists(txtFile)) return; // Original code had this commented out - File.WriteAllText overwrites anyway
            return txtFile;
        }

        // Removed LogFileWriteSuccess as logging is now inline
    }
}