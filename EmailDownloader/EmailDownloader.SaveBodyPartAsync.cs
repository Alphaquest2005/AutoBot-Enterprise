using Serilog;
namespace EmailDownloader;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MimeKit;

public static partial class EmailDownloader
{
    private static async Task SaveBodyPartAsync(
        string dataFolder, // Specific subfolder for the email (e.g., .../SubjectKey/UID/)
        MimeMessage emailMessage, // The MimeMessage object
        List<FileInfo> lst, // List to add the FileInfo of the saved "Info.txt"
        ILogger log, // Added ILogger parameter
        CancellationToken cancellationToken)
    {
        string methodName = nameof(SaveBodyPartAsync);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { DataFolder = dataFolder, Subject = emailMessage.Subject });

        
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Entered for dataFolder='{DataFolder}'. Message Subject: {Subject}",
                methodName, "Entry", dataFolder, emailMessage.Subject);

            string bodyContent = null;
            string fileExtension = ".txt"; // Default to .txt for plain text
 
            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Entered for dataFolder='{DataFolder}'. Message Subject: {Subject}",
                    methodName, "Entry", dataFolder, emailMessage.Subject);
 
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Prioritizing Plain Text Part.",
                    methodName, "FindBodyPart");
                // 1. Prioritize Plain Text Part
                TextPart textPart = emailMessage.BodyParts.OfType<TextPart>()
                    .FirstOrDefault(p => p.IsPlain); // Explicitly look for text/plain
 
                if (textPart != null)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found Plain Text part.",
                        methodName, "FindBodyPart");
                    bodyContent = textPart.Text;
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No Plain Text part found. Looking for HTML part.",
                        methodName, "FindBodyPart");
                    // 2. Fallback to HTML Part if no plain text part
                    TextPart htmlPart = emailMessage.BodyParts.OfType<TextPart>()
                        .FirstOrDefault(p => p.IsHtml); // Explicitly look for text/html
 
                    if (htmlPart != null)
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found HTML part.",
                            methodName, "FindBodyPart");
                        // Option A: Save raw HTML
                        // bodyContent = htmlPart.Text;
                        // fileExtension = ".html"; // Or keep as .txt and save HTML content
                        // log.Information("SaveBodyPartAsync: Will save raw HTML content.");
 
                        // Option B: Try to convert HTML to plain text (more complex, needs a utility)
                        // For simplicity, let's stick to saving what's available or just the raw HTML text.
                        // If you have an HTML to Text converter:
                        // bodyContent = ConvertHtmlToPlainText(htmlPart.Text);
                        // log.Information("SaveBodyPartAsync: Converted HTML to plain text (conceptual).");
 
                        // For now, let's just take the text content of the HTML part.
                        // This will include HTML tags if not converted.
                        bodyContent = htmlPart.Text;
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Using text content of HTML part (may include tags).",
                            methodName, "FindBodyPart");
                    }
                    else
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No Plain Text or HTML part found in BodyParts. Checking emailMessage.TextBody.",
                            methodName, "FindBodyPart");
                        // 3. Fallback to emailMessage.TextBody (MimeMessage convenience property)
                        // This property tries to give you the best textual representation.
                        if (!string.IsNullOrEmpty(emailMessage.TextBody))
                        {
                            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found content in emailMessage.TextBody.",
                                methodName, "FindBodyPart");
                            bodyContent = emailMessage.TextBody;
                        }
                        else if (!string.IsNullOrEmpty(emailMessage.HtmlBody)) // Last resort: HtmlBody property
                        {
                            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found content in emailMessage.HtmlBody. Using this (may include tags).",
                                methodName, "FindBodyPart");
                            bodyContent = emailMessage.HtmlBody;
                        }
                    }
                }
 
                if (string.IsNullOrEmpty(bodyContent))
                {
                    log.Warning("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: No textual body content found to save. Info.txt will not be created.",
                    methodName, 0); // Placeholder for duration
                    return; // No content to save
                }
 
                var fileName = "Info" + fileExtension; // e.g., "Info.txt" or "Info.html"
                var filePath = Path.Combine(dataFolder, fileName);
 
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Attempting to write body content to: {FilePath}",
                    methodName, "WriteFile", filePath);
                // Log a snippet for debugging, be careful with sensitive info if email bodies contain it
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Body content snippet (first 100 chars): {ContentSnippet}",
                    methodName, "WriteFile", (bodyContent.Length > 100 ? bodyContent.Substring(0, 100) + "..." : bodyContent));
 
                // Ensure the directory exists (should have been created by ProcessSingle...)
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Ensuring directory exists: '{Directory}'",
                    methodName, "EnsureDirectory", Path.GetDirectoryName(filePath));
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
 
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    "WriteAllTextAsync", "ASYNC_EXPECTED");
                // Use your WriteAllTextAsync helper (false for overwrite)
                await WriteAllTextAsync(filePath, bodyContent, false, log, cancellationToken).ConfigureAwait(false); // Pass the logger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "WriteAllTextAsync", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
 
 
                lst.Add(new FileInfo(filePath));
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Successfully wrote body content to {FilePath} and added to list. List count: {ListCount}",
                    methodName, "WriteFile", filePath, lst.Count);
 
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Saved body content to '{SavedFilePath}'",
                    methodName, 0, filePath); // Placeholder for duration
            }
            catch (OperationCanceledException)
            {
                log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Operation canceled while saving body to '{FilePath}'.",
                    methodName, 0, dataFolder + "/Info" + fileExtension); // Placeholder for duration
                throw; // Re-throw cancellation
            }
            catch (Exception ex)
            {
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. ERROR writing body content to '{FilePath}': {ErrorMessage}",
                    methodName, 0, dataFolder + "/Info" + fileExtension, ex.Message); // Placeholder for duration
                // Optionally log ex.StackTrace
            }
        
    }
}