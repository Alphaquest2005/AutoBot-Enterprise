using Serilog;
namespace EmailDownloader;
 
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
 
public static partial class EmailDownloader
{
    private static async Task WriteAllTextAsync(
        string path,
        string contents,
        bool append,
        ILogger logger, // Added ILogger parameter
        CancellationToken cancellationToken)
    {
        string methodName = nameof(WriteAllTextAsync);
        logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Writing text to file: {FilePath}. Append: {Append}. Content Length: {ContentLength}",
            methodName, "StartWrite", path, append, contents?.Length ?? 0);
 
        try
        {
            byte[] encodedText = System.Text.Encoding.UTF8.GetBytes(contents);
            FileMode mode = append ? FileMode.Append : FileMode.Create;
            using (var stream = new FileStream(
                       path,
                       mode,
                       FileAccess.Write,
                       FileShare.None,
                       bufferSize: 4096,
                       useAsync: true))
            {
                await stream.WriteAsync(encodedText, 0, encodedText.Length, cancellationToken).ConfigureAwait(false);
                if (append && !contents.EndsWith(Environment.NewLine)
                           && contents.Length > 0) // Add newline if appending and content doesn't end with one
                {
                    byte[] newlineBytes = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine);
                    await stream.WriteAsync(newlineBytes, 0, newlineBytes.Length, cancellationToken).ConfigureAwait(false);
                }
            }
 
            logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Successfully wrote text to file: {FilePath}",
                methodName, "WriteComplete", path);
        }
        catch (OperationCanceledException)
        {
            logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Operation canceled while writing to file: {FilePath}",
                methodName, "WriteCanceled", path);
            throw; // Re-throw cancellation
        }
        catch (Exception ex)
        {
            logger.Error(ex, "INTERNAL_STEP ({MethodName} - {Stage}): Error writing text to file: {FilePath}",
                methodName, "WriteError", path);
            throw; // Re-throw other exceptions
        }
    }
}