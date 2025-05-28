using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics;

using Serilog; // Added

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task SendEmailInternalAsync(Client clientDetails, MimeMessage message, // Changed to async Task
        CancellationToken cancellationToken = default, Serilog.ILogger log = null) // Changed ILogger to Serilog.ILogger
    {
        string methodName = nameof(SendEmailInternalAsync);
        string operationName = methodName; // Declared operationName
        var stopwatch = Stopwatch.StartNew(); // Start stopwatch
        log?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]", // METHOD_ENTRY log
            methodName, "Send email using SMTP client", $"ClientEmail: {clientDetails?.Email ?? "null"}, MessageSubject: {message?.Subject ?? "null"}");

        try
        {
            log?.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Checking if client email is null.", methodName, "Validation"); // INTERNAL_STEP
            if (clientDetails?.Email == null)
            {
                log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Client email is null. Returning.", methodName, "ClientEmailNull"); // INTERNAL_STEP (Warning level)
                stopwatch.Stop(); // Stop stopwatch
                log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.", // METHOD_EXIT_SUCCESS
                    methodName, "Skipped due to null client email", "", stopwatch.ElapsedMilliseconds);
                return; // Changed from return null to return
            }

            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "GetSendMailSettings", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
            var mailSettings = GetSendMailSettings(clientDetails.Email, log); // Pass the log parameter
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "GetSendMailSettings", 0, "Sync call returned."); // Placeholder for duration if GetSendMailSettings is sync


            log?.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Creating SmtpClient.", operationName, "CreateSmtpClient"); // INTERNAL_STEP
            using (var smtpClient = new SmtpClient()) // Renamed variable to avoid conflict
            {
                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) to server {Server} on port {Port}", // INVOKING_OPERATION
                    "smtpClient.ConnectAsync", "ASYNC_EXPECTED", mailSettings.Server, mailSettings.Port); // Changed to ConnectAsync
                var connectStopwatch = Stopwatch.StartNew(); // Start stopwatch
                await smtpClient.ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken).ConfigureAwait(false); // Changed to ConnectAsync
                connectStopwatch.Stop(); // Stop stopwatch
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) Connected: {IsConnected}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "smtpClient.ConnectAsync", connectStopwatch.ElapsedMilliseconds, "Async call completed (await).", smtpClient.IsConnected);


                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for user {Email}", // INVOKING_OPERATION
                    "smtpClient.AuthenticateAsync", "ASYNC_EXPECTED", clientDetails.Email); // Changed to AuthenticateAsync
                var authenticateStopwatch = Stopwatch.StartNew(); // Start stopwatch
                await smtpClient.AuthenticateAsync(clientDetails.Email, clientDetails.Password, cancellationToken).ConfigureAwait(false); // Changed to AuthenticateAsync
                authenticateStopwatch.Stop(); // Stop stopwatch
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) Authenticated: {IsAuthenticated}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "smtpClient.AuthenticateAsync", authenticateStopwatch.ElapsedMilliseconds, "Async call completed (await).", smtpClient.IsAuthenticated);


                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for message subject {Subject}", // INVOKING_OPERATION
                    "smtpClient.SendAsync", "ASYNC_EXPECTED", message?.Subject); // Changed to SendAsync
                var sendStopwatch = Stopwatch.StartNew(); // Start stopwatch
                await smtpClient.SendAsync(message, cancellationToken).ConfigureAwait(false); // Changed to SendAsync
                sendStopwatch.Stop(); // Stop stopwatch
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for message subject {Subject}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "smtpClient.SendAsync", sendStopwatch.ElapsedMilliseconds, "Async call completed (await).", message?.Subject);


                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                    "smtpClient.DisconnectAsync", "ASYC_EXPECTED"); // Changed to DisconnectAsync
                var disconnectStopwatch = Stopwatch.StartNew(); // Start stopwatch
                await smtpClient.DisconnectAsync(true, cancellationToken).ConfigureAwait(false); // Changed to DisconnectAsync
                disconnectStopwatch.Stop(); // Stop stopwatch
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) Disconnected: {IsConnected}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "smtpClient.DisconnectAsync", disconnectStopwatch.ElapsedMilliseconds, "Async call completed (await).", smtpClient.IsConnected);
            }

            stopwatch.Stop(); // Stop stopwatch
            log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.", // METHOD_EXIT_SUCCESS
                methodName, "Email sent successfully", "", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            stopwatch.Stop(); // Stop stopwatch on error
            log?.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                methodName, "Send email using SMTP client", stopwatch.ElapsedMilliseconds, e.Message);
            // Console.WriteLine(e); // Remove Console.WriteLine
            throw; // Re-throw the exception
        }
    }
}