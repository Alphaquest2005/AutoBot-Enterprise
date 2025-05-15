using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using MailKit;
using MimeKit;
using System.Diagnostics;

using Serilog; // Added

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<bool> SendBackMsgAsync(string emailId, Client clientDetails, string errtxt, Serilog.ILogger log,
        CancellationToken cancellationToken = default) // Changed ILogger to Serilog.ILogger, Added log parameter
    {
        string operationName = nameof(SendBackMsgAsync);
        var stopwatch = Stopwatch.StartNew(); // Start stopwatch
        // Use log?.Information to handle cases where log might be null if not passed
        log?.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
            operationName, new { EmailId = emailId, ClientEmail = clientDetails?.Email });

        try
        {
            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if client email is null.", operationName, "CheckClientEmail"); // INTERNAL_STEP
            if (clientDetails?.Email == null)
            {
                log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Client email is null. Returning false.", operationName, "ClientEmailNull"); // INTERNAL_STEP (Warning level)
                stopwatch.Stop(); // Stop stopwatch
                log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: False (Client Email Null).", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds);
                return false;
            }

            uint? uID = null;
            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Retrieving email entity from database for EmailId {EmailId}.", operationName, "GetEmailEntity", emailId); // INTERNAL_STEP
            using (var ctx = new CoreEntitiesContext()) // Assuming EF6 with async support
            {
                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for EmailId {EmailId}", // INVOKING_OPERATION
                    "ctx.Emails.FirstOrDefaultAsync", "ASYNC_EXPECTED", emailId);
                var emailEntity = await ctx.Emails
                    .FirstOrDefaultAsync(
                        x => x.EmailId == emailId && x.MachineName == Environment.MachineName /*, cancellationToken */)
                    .ConfigureAwait(false);
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) EmailEntityFound: {EmailEntityFound} for EmailId {EmailId}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "ctx.Emails.FirstOrDefaultAsync", "Async call completed (await).", emailEntity != null, emailId);

                if (emailEntity?.EmailUniqueId != null &&
                    uint.TryParse(emailEntity.EmailUniqueId.ToString(), out uint parsedUid))
                {
                    uID = parsedUid;
                    log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Parsed EmailUniqueId to UID: {UID}", operationName, "ParseUID", uID); // INTERNAL_STEP
                }
                else
                {
                    log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Could not retrieve or parse EmailUniqueId for EmailId {EmailId}. EmailEntityFound: {EmailEntityFound}", operationName, "ParseUIDFailed", emailId, emailEntity != null); // INTERNAL_STEP (Warning level)
                }
            }

            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if UID is null.", operationName, "CheckUID"); // INTERNAL_STEP
            if (uID == null)
            {
                log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): UID is null. Returning false.", operationName, "UIDNull"); // INTERNAL_STEP (Warning level)
                stopwatch.Stop(); // Stop stopwatch
                log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: False (UID Null).", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds);
                return false;
            }

            MimeMessage msg;
            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "GetImapClientAsync", "ASYNC_EXPECTED");
            using (var imapClient = await GetImapClientAsync(clientDetails, cancellationToken).ConfigureAwait(false))
            {
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ImapClientConnected: {ImapClientConnected}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "GetImapClientAsync", "Async call completed (await).", imapClient != null && imapClient.IsConnected);

                log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if IMAP client is null.", operationName, "CheckImapClient"); // INTERNAL_STEP
                if (imapClient == null)
                {
                    log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): IMAP client is null. Returning false.", operationName, "ImapClientNull"); // INTERNAL_STEP (Warning level)
                    stopwatch.Stop(); // Stop stopwatch
                    log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: False (IMAP Client Null).", // METHOD_EXIT_SUCCESS
                        operationName, stopwatch.ElapsedMilliseconds);
                    return false;
                }

                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for UID {UID}", // INVOKING_OPERATION
                    "imapClient.Inbox.GetMessageAsync", "ASYNC_EXPECTED", uID);
                msg = await imapClient.Inbox.GetMessageAsync(new UniqueId(uID.Value), cancellationToken)
                    .ConfigureAwait(false);
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) MessageFound: {MessageFound} for UID {UID}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "imapClient.Inbox.GetMessageAsync", "Async call completed (await).", msg != null, uID);
            }

            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if message is null.", operationName, "CheckMessage"); // INTERNAL_STEP
            if (msg != null)
            {
                log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for message subject {Subject}", // INVOKING_OPERATION
                    "SendBackMsgAsync (MimeMessage overload)", "ASYNC_EXPECTED", msg.Subject);
                await SendBackMsgAsync(msg, clientDetails, errtxt, log, cancellationToken) // Pass log to overloaded method
                    .ConfigureAwait(false); // Call the MimeMessage overload
                log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for message subject {Subject}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "SendBackMsgAsync (MimeMessage overload)", "Async call completed (await).", msg.Subject);
            }
            else
            {
                log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Message is null for UID {UID}. Skipping sending back message.", operationName, "MessageNull", uID); // INTERNAL_STEP (Warning level)
            }

            stopwatch.Stop(); // Stop stopwatch
            log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: True.", // METHOD_EXIT_SUCCESS
                operationName, stopwatch.ElapsedMilliseconds);
            return true;
        }
        catch (Exception e)
        {
            stopwatch.Stop(); // Stop stopwatch on error
            log?.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                operationName, stopwatch.ElapsedMilliseconds, e.Message);
            // Console.WriteLine(e); // Remove Console.WriteLine
            // Consider rethrowing or returning false based on policy
            return false; // Keep original return false logic on exception
        }
    }

    public static async Task SendBackMsgAsync(MimeMessage msg, Client clientDetails, string errtxt, Serilog.ILogger log,
        CancellationToken cancellationToken = default) // Changed ILogger to Serilog.ILogger, Changed access modifier to public
    {
        string operationName = nameof(SendBackMsgAsync) + "_MimeMessage"; // Differentiate overload
        var stopwatch = Stopwatch.StartNew(); // Start stopwatch
        log?.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
            operationName, new { MessageSubject = msg?.Subject, ClientEmail = clientDetails?.Email });

        try
        {
            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if client email is null.", operationName, "CheckClientEmail"); // INTERNAL_STEP
            if (clientDetails?.Email == null)
            {
                log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Client email is null. Returning.", operationName, "ClientEmailNull"); // INTERNAL_STEP (Warning level)
                stopwatch.Stop(); // Stop stopwatch
                log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: Void (Client Email Null).", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds);
                return;
            }

            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Creating new MimeMessage.", operationName, "CreateMimeMessage"); // INTERNAL_STEP
            var message = new MimeMessage();

            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "message.From.Add", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
            message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "message.From.Add", "Sync call returned.");


            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking DevMode and adding ReplyTo/To recipients.", operationName, "AddRecipients"); // INTERNAL_STEP
            if (!clientDetails.DevMode)
            {
                if (msg?.From.Mailboxes.Any() == true)
                {
                    log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                        "message.ReplyTo.Add", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
                    message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name,
                        msg.From.Mailboxes.FirstOrDefault().Address));
                    log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                        "message.ReplyTo.Add", "Sync call returned.");

                    log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                        "message.To.Add", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
                    message.To.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
                    log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                        "message.To.Add", "Sync call returned.");
                }
                else
                {
                    log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Original message From mailbox is empty. Cannot add ReplyTo/To recipients.", operationName, "OriginalFromEmpty"); // INTERNAL_STEP (Warning level)
                }
            }
            else
            {
                log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): DevMode is enabled. Skipping adding ReplyTo/To recipients from original message.", operationName, "DevModeSkipRecipients"); // INTERNAL_STEP
            }


            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Setting email subject.", operationName, "SetSubject"); // INTERNAL_STEP
            message.Subject = "FWD: " + msg?.Subject;

            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Creating BodyBuilder and adding text body.", operationName, "CreateBodyBuilder"); // INTERNAL_STEP
            var builder = new BodyBuilder { TextBody = errtxt };

            log?.Information("INTERNAL_STEP ({MethodName} - {Stage}): Adding original message as attachment.", operationName, "AddAttachment"); // INTERNAL_STEP
            if (msg != null)
            {
                 log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for message subject {Subject}", // INVOKING_OPERATION
                    "builder.Attachments.Add (MessagePart)", "SYNC_EXPECTED", msg.Subject); // This is not async, so SYNC_EXPECTED
                builder.Attachments.Add(new MessagePart { Message = msg });
                 log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for message subject {Subject}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "builder.Attachments.Add (MessagePart)", "Sync call returned.", msg.Subject);
            }
            else
            {
                 log?.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Original message is null. Cannot add as attachment.", operationName, "OriginalMessageNullForAttachment"); // INTERNAL_STEP (Warning level)
            }


            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "builder.ToMessageBody", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
            message.Body = builder.ToMessageBody();
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "builder.ToMessageBody", "Sync call returned.");


            log?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                "SendEmailInternalAsync", "ASYNC_EXPECTED");
            await SendEmailInternalAsync(clientDetails, message, cancellationToken, log) // Use internal async version and pass log
                .ConfigureAwait(false); // Use internal async version
            log?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                "SendEmailInternalAsync", "Async call completed (await).");


            stopwatch.Stop(); // Stop stopwatch
            log?.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", // METHOD_EXIT_SUCCESS
                operationName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            stopwatch.Stop(); // Stop stopwatch on error
            log?.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                operationName, stopwatch.ElapsedMilliseconds, e.Message);
            // Console.WriteLine(e); // Remove Console.WriteLine
            throw; // Re-throw the exception
        }
    }
}