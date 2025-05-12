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
        CancellationToken cancellationToken)
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
    }
}