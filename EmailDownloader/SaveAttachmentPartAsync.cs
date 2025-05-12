using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task SaveAttachmentPartAsync(
               string dataFolder,      // This should be the specific subfolder for the email (e.g., .../SubjectKey/UID/)
               MimeEntity attachmentMimeEntity, // The attachment to save
               List<FileInfo> lst,     // The list to add the FileInfo of the saved attachment to
               CancellationToken cancellationToken)
    {
        Console.WriteLine($"SaveAttachmentPartAsync: Entered. Entity Type: {attachmentMimeEntity?.GetType().Name}, ContentType: {attachmentMimeEntity?.ContentType}");
        if (!(attachmentMimeEntity is MimePart part))
        {
            Console.WriteLine($"SaveAttachmentPartAsync: Provided MimeEntity is not a MimePart. Actual type: {attachmentMimeEntity?.GetType().Name}. Skipping.");
            return;
        }
        Console.WriteLine($"SaveAttachmentPartAsync: MimeEntity is a MimePart. Checking IsAttachment. IsAttachment: {part.IsAttachment}, FileName: '{part.FileName}'");
        if (!part.IsAttachment)
        {
            Console.WriteLine($"SaveAttachmentPartAsync: MimePart.IsAttachment is false. FileName: '{part.FileName}'. Skipping.");
            return;
        }

        // Use part.FileName, but provide a default if it's null or empty
        var originalFileName = part.FileName;
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            // Try to generate a name from ContentType if FileName is missing
            var extension = GetExtensionFromMimeType(part.ContentType) ?? ".dat";
            originalFileName = $"unknown_attachment_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
            Console.WriteLine($"SaveAttachmentPartAsync: Attachment FileName was null/empty. Generated name: {originalFileName}");
        }

        Console.WriteLine($"SaveAttachmentPartAsync: Processing attachment. Original FileName='{originalFileName}', ContentType='{part.ContentType.MimeType}', DataFolder='{dataFolder}'");

        var cleanedFileName = CleanFileName(originalFileName); // Uses your existing CleanFileName method
        var filePath = Path.Combine(dataFolder, cleanedFileName);

        Console.WriteLine($"SaveAttachmentPartAsync: Cleaned FileName='{cleanedFileName}', Initial FilePath='{filePath}'");

        // Handle duplicate file names by appending a number
        filePath = GetNextAvailableFileName(filePath); // Renamed from GetNextFileName for clarity
        if (filePath == null)
        {
            Console.WriteLine($"SaveAttachmentPartAsync: Could not determine a unique available file name for '{cleanedFileName}' in '{dataFolder}'. Skipping save.");
            return; // Max attempts reached or other issue in GetNextAvailableFileName
        }
        Console.WriteLine($"SaveAttachmentPartAsync: Final unique FilePath='{filePath}'");

        try
        {
            // Ensure the directory exists one last time (should have been created by ProcessSingle...)
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Path.GetDirectoryName(filePath) is dataFolder

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                Console.WriteLine($"SaveAttachmentPartAsync: Decoding content to stream for '{filePath}'...");
                await part.Content.DecodeToAsync(stream, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"SaveAttachmentPartAsync: Successfully decoded and saved attachment to '{filePath}'");
            }
            lst.Add(new FileInfo(filePath));
            Console.WriteLine($"SaveAttachmentPartAsync: Added FileInfo for '{filePath}' to list. List count: {lst.Count}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"SaveAttachmentPartAsync: Operation canceled while saving '{filePath}'.");
            throw; // Re-throw cancellation
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SaveAttachmentPartAsync: ERROR saving attachment '{cleanedFileName}' to '{filePath}': {ex.GetType().Name} - {ex.Message}");
            // Optionally log ex.StackTrace for more details
        }
    }

    private static string GetNextAvailableFileName(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return filePath; // Path is available
        }

        string directory = Path.GetDirectoryName(filePath);
        string originalNameOnly = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath); // Includes the dot
        int count = 1;

        string newFileName;
        do
        {
            if (count > 999)
            { // Safety break to prevent infinite loop for some edge case
                Console.WriteLine($"GetNextAvailableFileName: Exceeded 999 attempts for '{filePath}'. Returning null.");
                return null;
            }
            newFileName = $"{originalNameOnly}({count++}){extension}";
            filePath = Path.Combine(directory, newFileName);
        }
        while (File.Exists(filePath));

        return filePath;
    }
    // A small, extendable dictionary for common MIME type to extension mappings
    private static readonly IDictionary<string, string> _commonMimeTypeToExtensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "text/plain", ".txt" },
            { "text/html", ".html" },
            { "text/xml", ".xml" },
            { "application/xml", ".xml" },
            { "application/json", ".json" },
            { "application/pdf", ".pdf" },
            { "application/msword", ".doc" },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
            { "application/vnd.ms-excel", ".xls" },
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
            { "application/vnd.ms-powerpoint", ".ppt" },
            { "application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx" },
            { "application/zip", ".zip" },
            { "application/gzip", ".gz" },
            { "application/octet-stream", ".bin" }, // Generic binary
            { "image/jpeg", ".jpg" },
            { "image/png", ".png" },
            { "image/gif", ".gif" },
            { "image/bmp", ".bmp" },
            { "image/webp", ".webp" },
            { "audio/mpeg", ".mp3" },
            { "audio/wav", ".wav" },
            { "video/mp4", ".mp4" },
            { "video/mpeg", ".mpeg" },
            // Add more common types as needed
        };

    private static string GetExtensionFromMimeType(ContentType contentType)
    {
        if (contentType == null) return ".dat"; // Default fallback

        // MimeKit's ContentType.MediaSubtype can sometimes be helpful if it includes parameters like "; name=file.txt"
        // but part.FileName should be preferred if available.
        // This is for when part.FileName is null.

        string mimeType = contentType.MimeType.ToLowerInvariant();
        if (_commonMimeTypeToExtensionMap.TryGetValue(mimeType, out var extension))
        {
            return extension;
        }

        // Fallback for less common types: try to derive from subtype if it looks like an extension
        // e.g., "application/x-rar-compressed" -> ".rar" (this is a heuristic)
        if (contentType.MediaSubtype.Contains("-") && contentType.MediaSubtype.LastIndexOf('-') < contentType.MediaSubtype.Length - 1)
        {
            string potentialExt = contentType.MediaSubtype.Substring(contentType.MediaSubtype.LastIndexOf('-') + 1);
            if (potentialExt.Length > 1 && potentialExt.Length < 6 && potentialExt.All(char.IsLetterOrDigit))
            {
                return "." + potentialExt;
            }
        }

        // If it's an image, audio, or video, but not in our map, provide a generic extension
        if (contentType.IsMimeType("image", "*")) return ".img";
        if (contentType.IsMimeType("audio", "*")) return ".aud";
        if (contentType.IsMimeType("video", "*")) return ".vid";

        return ".dat"; // Generic fallback
    }

    // Ensure WriteAllTextAsync helper is present in this class or accessible

    // ... (other methods like SaveAttachmentPartAsync, CleanFileName, etc.)
}
