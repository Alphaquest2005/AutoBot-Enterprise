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
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"SaveBodyPartAsync: Entered for dataFolder='{dataFolder}'. Message Subject: {emailMessage.Subject}");

        string bodyContent = null;
        string fileExtension = ".txt"; // Default to .txt for plain text

        // 1. Prioritize Plain Text Part
        TextPart textPart = emailMessage.BodyParts.OfType<TextPart>()
            .FirstOrDefault(p => p.IsPlain); // Explicitly look for text/plain

        if (textPart != null)
        {
            Console.WriteLine("SaveBodyPartAsync: Found Plain Text part.");
            bodyContent = textPart.Text;
        }
        else
        {
            Console.WriteLine("SaveBodyPartAsync: No Plain Text part found. Looking for HTML part.");
            // 2. Fallback to HTML Part if no plain text part
            TextPart htmlPart = emailMessage.BodyParts.OfType<TextPart>()
                .FirstOrDefault(p => p.IsHtml); // Explicitly look for text/html

            if (htmlPart != null)
            {
                Console.WriteLine("SaveBodyPartAsync: Found HTML part.");
                // Option A: Save raw HTML
                // bodyContent = htmlPart.Text;
                // fileExtension = ".html"; // Or keep as .txt and save HTML content
                // Console.WriteLine("SaveBodyPartAsync: Will save raw HTML content.");

                // Option B: Try to convert HTML to plain text (more complex, needs a utility)
                // For simplicity, let's stick to saving what's available or just the raw HTML text.
                // If you have an HTML to Text converter:
                // bodyContent = ConvertHtmlToPlainText(htmlPart.Text); 
                // Console.WriteLine("SaveBodyPartAsync: Converted HTML to plain text (conceptual).");

                // For now, let's just take the text content of the HTML part.
                // This will include HTML tags if not converted.
                bodyContent = htmlPart.Text;
                Console.WriteLine("SaveBodyPartAsync: Using text content of HTML part (may include tags).");
            }
            else
            {
                Console.WriteLine(
                    "SaveBodyPartAsync: No Plain Text or HTML part found in BodyParts. Checking emailMessage.TextBody.");
                // 3. Fallback to emailMessage.TextBody (MimeMessage convenience property)
                // This property tries to give you the best textual representation.
                if (!string.IsNullOrEmpty(emailMessage.TextBody))
                {
                    Console.WriteLine("SaveBodyPartAsync: Found content in emailMessage.TextBody.");
                    bodyContent = emailMessage.TextBody;
                }
                else if (!string.IsNullOrEmpty(emailMessage.HtmlBody)) // Last resort: HtmlBody property
                {
                    Console.WriteLine(
                        "SaveBodyPartAsync: Found content in emailMessage.HtmlBody. Using this (may include tags).");
                    bodyContent = emailMessage.HtmlBody;
                }
            }
        }

        if (string.IsNullOrEmpty(bodyContent))
        {
            Console.WriteLine(
                "SaveBodyPartAsync: No textual body content found to save. Info.txt will not be created.");
            return; // No content to save
        }

        var fileName = "Info" + fileExtension; // e.g., "Info.txt" or "Info.html"
        var filePath = Path.Combine(dataFolder, fileName);

        Console.WriteLine($"SaveBodyPartAsync: Attempting to write body content to: {filePath}");
        // Log a snippet for debugging, be careful with sensitive info if email bodies contain it
        Console.WriteLine(
            $"SaveBodyPartAsync: Body content snippet (first 100 chars): {(bodyContent.Length > 100 ? bodyContent.Substring(0, 100) + "..." : bodyContent)}");

        try
        {
            // Ensure the directory exists (should have been created by ProcessSingle...)
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Use your WriteAllTextAsync helper (false for overwrite)
            await WriteAllTextAsync(filePath, bodyContent, false, cancellationToken).ConfigureAwait(false);

            lst.Add(new FileInfo(filePath));
            Console.WriteLine(
                $"SaveBodyPartAsync: Successfully wrote body content to {filePath} and added to list. List count: {lst.Count}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"SaveBodyPartAsync: Operation canceled while saving body to '{filePath}'.");
            throw; // Re-throw cancellation
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"SaveBodyPartAsync: ERROR writing body content to '{filePath}': {ex.GetType().Name} - {ex.Message}");
            // Optionally log ex.StackTrace
        }
    }
}