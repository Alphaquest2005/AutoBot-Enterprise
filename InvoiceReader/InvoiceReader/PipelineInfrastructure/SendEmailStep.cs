// Needed for EmailDownloader.EmailDownloader and Client
using EmailDownloader; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Collections.Generic; // Added for IEnumerable/List

// Needed for file paths

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class SendEmailStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger instance
        // private static readonly ILogger _logger = Log.ForContext<SendEmailStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Send error email notification", $"FilePath: {filePath}");

            // Null check context first
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "SendEmailStep executed with null context.");
            }

            if (IsRequiredDataMissing(context.Logger, context)) // Handles its own logging, pass logger
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "Validation", "SendEmailStep cannot proceed due to missing required data in context.", $"FilePath: {filePath}", "");
                methodStopwatch.Stop(); // Stop stopwatch on failure
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Skipped due to missing data", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return false; // Step fails if required data is missing
            }

            try
            {
                // Logic from the original CreateEmail method for sending the email
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "Processing", "Attempting to send error email.", $"File: {filePath}", "");
                IEnumerable<string> contacts = null;
                try
                {
                    context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        "EmailDownloader.EmailDownloader.GetContacts(\"Developer\")", "SYNC_EXPECTED"); // Log before GetContacts
                    var getContactsStopwatch = Stopwatch.StartNew(); // Start stopwatch
                    contacts = EmailDownloader.EmailDownloader.GetContacts("Developer", context.Logger); // Pass the logger
                    getContactsStopwatch.Stop(); // Stop stopwatch
                    context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "EmailDownloader.EmailDownloader.GetContacts(\"Developer\")", getContactsStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after GetContacts

                    context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(Execute), "Processing", "Retrieved developer contacts.", $"ContactCount: {contacts?.Count() ?? 0}", "");
                }
                catch (Exception contactEx)
                {
                     context.Logger?.Error(contactEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Get developer contacts for email", 0, $"Error getting developer contacts for email. Error: {contactEx.Message}");
                     // Decide if we should proceed without contacts or fail
                     methodStopwatch.Stop(); // Stop stopwatch on failure
                     return false; // Fail step if contacts cannot be retrieved
                }


                // Filter out null or empty file paths before attaching
                var attachments = new[] { context.FilePath, context.TextFilePath }
                                    .Where(f => !string.IsNullOrEmpty(f))
                                    .ToArray();
                context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "Processing", "Email details.", $"Subject: 'Template Template Not found!', Body Length: {context.EmailBody?.Length ?? 0}, Attachments: {string.Join(", ", attachments)}", "");

                // Assuming SendEmail is synchronous or handles its own async internally
                // Running potentially blocking network code in background thread.
               
                context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"EmailDownloader.EmailDownloader.SendEmailAsync for File: {filePath}", "ASYNC_EXPECTED"); // Log before SendEmailAsync
                var sendEmailStopwatch = Stopwatch.StartNew(); // Start stopwatch
                  await EmailDownloader.EmailDownloader.SendEmailAsync(
                        context.Client,
                        null, // Assuming null sender is intended
                        "Template Template Not found!",
                        contacts?.ToArray(), // Convert IEnumerable<string> to string[]
                        context.EmailBody,
                        attachments, // Use filtered list
                     context.Logger).ConfigureAwait(false);
                sendEmailStopwatch.Stop(); // Stop stopwatch
                context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"EmailDownloader.EmailDownloader.SendEmailAsync for File: {filePath}", sendEmailStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after SendEmailAsync

                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "Completion", "Successfully sent error email.", $"File: {filePath}", "");

                methodStopwatch.Stop(); // Stop stopwatch
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Error email sent successfully", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return true; // Indicate success
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on error
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Send error email notification", methodStopwatch.ElapsedMilliseconds, $"Error sending email for File: {filePath}. Error: {ex.Message}");
                return false; // Indicate failure
            }
        }

        private static bool IsRequiredDataMissing(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
             logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsRequiredDataMissing), "Validation", "Checking for missing required data for sending email.", "", "");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (context?.Client == null) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data for SendEmail: Client is null.", "", ""); return true; }
             if (string.IsNullOrEmpty(context.EmailBody)) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data for SendEmail: EmailBody is null or empty.", "", ""); return true; }
             // FilePath and TextFilePath are used as attachments, but maybe sending without them is acceptable?
             // Let's only require Client and EmailBody for now, attachments are optional.
             // if (string.IsNullOrEmpty(context.FilePath)) { logger?.Warning("Missing required data for SendEmail: FilePath is null or empty."); return true; }
             // if (string.IsNullOrEmpty(context.TextFilePath)) { logger?.Warning("Missing required data for SendEmail: TextFilePath is null or empty."); return true; }

             logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsRequiredDataMissing), "Validation", "No missing required data found for sending email (Client, EmailBody).", "", "");
             return false; // All strictly required data is present
        }
    }
}