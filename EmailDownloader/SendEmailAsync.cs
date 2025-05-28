using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using Serilog; // Added
using System.Diagnostics; // Added System.Diagnostics for Stopwatch

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task SendEmailAsync(Client client, string directory, string subject, string[] To, string body,
        string[] attachments, ILogger log, CancellationToken cancellationToken = default) // Added ILogger parameter
    {
        var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
        string methodName = nameof(SendEmailAsync);
        log.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
            methodName, "Send email asynchronously", $"ClientEmail: {client?.Email ?? "null"}, Subject: {subject}, ToRecipients: {To?.Length ?? 0}");

        try
        {
            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Checking if client email is null.", methodName, "Validation");
            if (client?.Email == null)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                log.Warning("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, "Send email asynchronously", methodStopwatch.ElapsedMilliseconds, "Client email is null. Skipping email send.");
                return;
            }

            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Creating MimeMessage.", methodName, "MessageCreation");
            MimeMessage msg = CreateMessage(client, subject, To, body, attachments, log); // Pass the log parameter
            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): MimeMessage created.", methodName, "MessageCreation");

            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "SendEmailInternalAsync", "ASYNC_EXPECTED");
            var internalSendStopwatch = Stopwatch.StartNew(); // Start stopwatch for internal send
            await SendEmailInternalAsync(client, msg, cancellationToken, log) // Use internal async version and pass log
                .ConfigureAwait(false); // Use internal async version
            internalSendStopwatch.Stop(); // Stop stopwatch
            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "SendEmailInternalAsync", internalSendStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Checking if directory and attachments are present for logging results.", methodName, "LogResults");
            if (directory != null && attachments != null && attachments.Any())
            {
                string resultsFilePath = Path.Combine(directory, "EmailResults.txt");
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Appending email results to file: '{FilePath}'",
                    methodName, "LogResults", resultsFilePath);
                // For .NET 4.8, File.AppendAllLinesAsync might not exist or have the desired overload.
                // Using a helper if needed, or Task.Run for simplicity if AppendAllLines is sync.

                try
                {
                    // Assuming File.AppendAllLines is synchronous and acceptable here
                    File.AppendAllLines(resultsFilePath, attachments);
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Successfully appended email results to file.", methodName, "LogResults");
                }
                catch (Exception fileEx)
                {
                    log.Error(fileEx, "INTERNAL_STEP ({OperationName} - {Stage}): Error appending email results to file: {ErrorMessage}",
                        methodName, "LogResults", fileEx.Message);
                    // Decide if this error should fail the whole email send operation
                }
            }
            else
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Directory or attachments not present. Skipping logging email results to file.", methodName, "LogResults");
            }

            methodStopwatch.Stop(); // Stop stopwatch
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                methodName, "Email sent successfully", "", methodStopwatch.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            methodStopwatch.Stop(); // Stop stopwatch on error
            log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, "Send email asynchronously", methodStopwatch.ElapsedMilliseconds, e.Message);
            throw;
        }
    }
}