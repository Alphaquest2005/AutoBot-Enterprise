using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using System.Diagnostics;

using Serilog; // Added

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task<ImapClient> GetImapClientAsync(Client client, CancellationToken cancellationToken, Serilog.ILogger log = null) // Changed ILogger to Serilog.ILogger
    {
        string operationName = nameof(GetImapClientAsync);
        var stopwatch = Stopwatch.StartNew(); // Start stopwatch
        log?.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
            operationName, new { ClientEmail = client?.Email });

        try
        {
            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Creating new ImapClient.", operationName, "CreateImapClient"); // INTERNAL_STEP
            var imapClient = new ImapClient(); // ProtocolLogger can be added here for deep diagnostics
            // Example: imapClient = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));

            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for client email {Email}", // INVOKING_OPERATION
                "GetReadMailSettings", "SYNC_EXPECTED", client?.Email); // This is not async, so SYNC_EXPECTED
            var mailSettings = GetReadMailSettings(client.Email, log); // Pass log to GetReadMailSettings
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) MailSettingsFound: {MailSettingsFound} for client email {Email}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "GetReadMailSettings", "Sync call returned.", mailSettings != null, client?.Email);


            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) to server {Server} on port {Port}", // INVOKING_OPERATION
                "imapClient.ConnectAsync", "ASYNC_EXPECTED", mailSettings?.Server, mailSettings?.Port);
            await imapClient
                .ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken)
                .ConfigureAwait(false);
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) Connected: {IsConnected}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "imapClient.ConnectAsync", "Async call completed (await).", imapClient.IsConnected);


            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for user {Email}", // INVOKING_OPERATION
                "imapClient.AuthenticateAsync", "ASYNC_EXPECTED", client?.Email);
            await imapClient.AuthenticateAsync(client.Email, client.Password, cancellationToken).ConfigureAwait(false);
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) Authenticated: {IsAuthenticated}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "imapClient.AuthenticateAsync", "Async call completed (await).", imapClient.IsAuthenticated);


            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "imapClient.Inbox.OpenAsync", "ASYNC_EXPECTED");
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) InboxOpen: {IsOpen}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "imapClient.Inbox.OpenAsync", "Async call completed (await).", imapClient.Inbox.IsOpen);


            stopwatch.Stop(); // Stop stopwatch
            log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: ImapClient Connected: {IsConnected}", // METHOD_EXIT_SUCCESS
                operationName, stopwatch.ElapsedMilliseconds, imapClient.IsConnected);
            return imapClient;
        }
        catch (Exception e)
        {
            stopwatch.Stop(); // Stop stopwatch on error
            log?.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                operationName, stopwatch.ElapsedMilliseconds, e.Message);
            // Console.WriteLine($"GetImapClientAsync Error for {client.Email}: {e}"); // Remove Console.WriteLine
            return null; // Keep original return null logic on exception
        }
    }
}