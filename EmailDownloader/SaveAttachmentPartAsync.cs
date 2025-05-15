using Serilog;
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
               ILogger log, // Added ILogger parameter
               CancellationToken cancellationToken)
    {
        string methodName = nameof(SaveAttachmentPartAsync);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { DataFolder = dataFolder, EntityType = attachmentMimeEntity?.GetType().Name, ContentType = attachmentMimeEntity?.ContentType });

        string cleanedFileName = null; // Declare outside try block
        string filePath = null; // Declare outside try block

        try
        {
            if (!(attachmentMimeEntity is MimePart part))
            {
                log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Provided MimeEntity is not a MimePart. Actual type: {ActualType}. Skipping.",
                    methodName, 0, attachmentMimeEntity?.GetType().Name); // Placeholder for duration
                return;
            }
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): MimeEntity is a MimePart. Checking IsAttachment. IsAttachment: {IsAttachment}, FileName: '{FileName}'",
                methodName, "CheckPart", part.IsAttachment, part.FileName);

            if (!part.IsAttachment)
            {
                log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: MimePart.IsAttachment is false. FileName: '{FileName}'. Skipping.",
                    methodName, 0, part.FileName); // Placeholder for duration
                return;
            }

            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Handling file name.",
                methodName, "HandleFileName");
            // Use part.FileName, but provide a default if it's null or empty
            var originalFileName = part.FileName;
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                // Try to generate a name from ContentType if FileName is missing
                var extension = GetExtensionFromMimeType(part.ContentType, log) ?? ".dat"; // Pass the logger
                originalFileName = $"unknown_attachment_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Attachment FileName was null/empty. Generated name: {GeneratedName}",
                    methodName, "HandleFileName", originalFileName);
            }

            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing attachment. Original FileName='{OriginalFileName}', ContentType='{ContentType}', DataFolder='{DataFolder}'",
                methodName, "Process", originalFileName, part.ContentType.MimeType, dataFolder);

            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "CleanFileName", "SYNC_EXPECTED");
            cleanedFileName = CleanFileName(originalFileName, log); // Uses your existing CleanFileName method, Pass the logger
            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "CleanFileName", 0, "Sync call returned"); // Placeholder for duration
            filePath = Path.Combine(dataFolder, cleanedFileName);

            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Cleaned FileName='{CleanedFileName}', Initial FilePath='{InitialFilePath}'",
                methodName, "Process", cleanedFileName, filePath);

            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "GetNextAvailableFileName", "SYNC_EXPECTED");
            // Handle duplicate file names by appending a number
            filePath = GetNextAvailableFileName(filePath, log); // Renamed from GetNextFileName for clarity, Pass the logger
            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "GetNextAvailableFileName", 0, "Sync call returned"); // Placeholder for duration

            if (filePath == null)
            {
                log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Could not determine a unique available file name for '{CleanedFileName}' in '{DataFolder}'. Skipping save.",
                    methodName, 0, cleanedFileName, dataFolder); // Placeholder for duration
                return; // Max attempts reached or other issue in GetNextAvailableFileName
            }
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Final unique FilePath='{FinalFilePath}'",
                methodName, "Process", filePath);

            // Ensure the directory exists one last time (should have been created by ProcessSingle...)
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Ensuring directory exists: '{Directory}'",
                methodName, "EnsureDirectory", Path.GetDirectoryName(filePath));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Path.GetDirectoryName(filePath) is dataFolder

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Decoding content to stream for '{FilePath}'...",
                    methodName, "DecodeContent", filePath);
                await part.Content.DecodeToAsync(stream, cancellationToken).ConfigureAwait(false);
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Successfully decoded and saved attachment to '{FilePath}'",
                    methodName, "DecodeContent", filePath);
            }
            lst.Add(new FileInfo(filePath));
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Added FileInfo for '{FilePath}' to list. List count: {ListCount}",
                methodName, "AddToList", filePath, lst.Count);

            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Saved attachment to '{SavedFilePath}'",
                methodName, 0, filePath); // Placeholder for duration
        }
        catch (OperationCanceledException)
        {
            log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Operation canceled while saving '{FilePath}'.",
                methodName, 0, filePath); // Placeholder for duration
            throw; // Re-throw cancellation
        }
        catch (Exception ex)
        {
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error saving attachment '{CleanedFileName}' to '{FilePath}': {ErrorMessage}",
                methodName, 0, cleanedFileName, filePath, ex.Message); // Placeholder for duration
            // Optionally log ex.StackTrace for more details
        }
    }

    private static string GetNextAvailableFileName(string filePath, ILogger log) // Added ILogger parameter
    {
        string methodName = nameof(GetNextAvailableFileName);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { InitialFilePath = filePath });

        if (!File.Exists(filePath))
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): File path '{FilePath}' is available.",
                methodName, "CheckAvailability", filePath);
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Available File Path: '{AvailableFilePath}'",
                methodName, 0, filePath); // Placeholder for duration
            return filePath; // Path is available
        }

        log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): File path '{FilePath}' already exists. Finding next available name.",
            methodName, "CheckAvailability", filePath);

        string directory = Path.GetDirectoryName(filePath);
        string originalNameOnly = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath); // Includes the dot
        int count = 1;

        string newFileName;
        do
        {
            if (count > 999)
            { // Safety break to prevent infinite loop for some edge case
                log.Error("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Exceeded 999 attempts for '{FilePath}'. Returning null.",
                    methodName, 0, filePath); // Placeholder for duration
                return null;
            }
            newFileName = $"{originalNameOnly}({count++}){extension}";
            filePath = Path.Combine(directory, newFileName);
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Trying file name: '{NewFileName}'",
                methodName, "FindNext", newFileName);
        }
        while (File.Exists(filePath));

        log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Found next available file path: '{AvailableFilePath}'",
            methodName, 0, filePath); // Placeholder for duration
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

    private static string GetExtensionFromMimeType(ContentType contentType, ILogger log) // Added ILogger parameter
    {
        string methodName = nameof(GetExtensionFromMimeType);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { ContentType = contentType?.MimeType });

        if (contentType == null)
        {
            log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: ContentType is null. Returning '.dat'.",
                methodName, 0); // Placeholder for duration
            return ".dat"; // Default fallback
        }

        // MimeKit's ContentType.MediaSubtype can sometimes be helpful if it includes parameters like "; name=file.txt"
        // but part.FileName should be preferred if available.
        // This is for when part.FileName is null.

        string mimeType = contentType.MimeType.ToLowerInvariant();
        if (_commonMimeTypeToExtensionMap.TryGetValue(mimeType, out var extension))
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found extension '{Extension}' for MIME type '{MimeType}' in map.",
                methodName, "MapLookup", extension, mimeType);
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Found Extension: '{FoundExtension}'",
                methodName, 0, extension); // Placeholder for duration
            return extension;
        }

        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): MIME type '{MimeType}' not in map. Attempting to derive from subtype.",
            methodName, "DeriveFromSubtype", mimeType);
        // Fallback for less common types: try to derive from subtype if it looks like an extension
        // e.g., "application/x-rar-compressed" -> ".rar" (this is a heuristic)
        if (contentType.MediaSubtype.Contains("-") && contentType.MediaSubtype.LastIndexOf('-') < contentType.MediaSubtype.Length - 1)
        {
            string potentialExt = contentType.MediaSubtype.Substring(contentType.MediaSubtype.LastIndexOf('-') + 1);
            if (potentialExt.Length > 1 && potentialExt.Length < 6 && potentialExt.All(char.IsLetterOrDigit))
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Derived potential extension '{PotentialExtension}' from subtype.",
                    methodName, "DeriveFromSubtype", potentialExt);
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Derived Extension: '.{DerivedExtension}'",
                    methodName, 0, potentialExt); // Placeholder for duration
                return "." + potentialExt;
            }
        }

        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Could not derive from subtype. Checking generic types.",
            methodName, "CheckGenericTypes");
        // If it's an image, audio, or video, but not in our map, provide a generic extension
        if (contentType.IsMimeType("image", "*"))
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Is generic image type. Returning '.img'.",
                methodName, "CheckGenericTypes");
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Generic Extension: '.img'",
                methodName, 0); // Placeholder for duration
            return ".img";
        }
        if (contentType.IsMimeType("audio", "*"))
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Is generic audio type. Returning '.aud'.",
                methodName, "CheckGenericTypes");
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Generic Extension: '.aud'",
                methodName, 0); // Placeholder for duration
            return ".aud";
        }
        if (contentType.IsMimeType("video", "*"))
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Is generic video type. Returning '.vid'.",
                methodName, "CheckGenericTypes");
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Generic Extension: '.vid'",
                methodName, 0); // Placeholder for duration
            return ".vid";
        }

        log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: Could not determine extension. Returning '.dat'.",
            methodName, 0); // Placeholder for duration
        return ".dat"; // Generic fallback
    }

    // Ensure WriteAllTextAsync helper is present in this class or accessible

    // ... (other methods like SaveAttachmentPartAsync, CleanFileName, etc.)
}
