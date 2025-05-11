using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task SaveAttachmentPartAsync(string dataFolder, MimeEntity a, List<FileInfo> lst,
        CancellationToken cancellationToken) // Made async
    {
        var part = (MimePart)a;
        var fileName = CleanFileName(part.FileName);
        var file = Path.Combine(dataFolder, fileName);
        if (File.Exists(file)) file = GetNextFileName(file);
        if (file == null) return; // Could not get a unique name

        using (var stream =
               new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await part.Content.DecodeToAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        lst.Add(new FileInfo(file));
    }
}