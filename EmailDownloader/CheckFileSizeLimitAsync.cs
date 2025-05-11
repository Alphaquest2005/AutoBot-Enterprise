using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task<bool> CheckFileSizeLimitAsync(Client client, List<FileTypes> fileTypes,
        List<FileInfo> lst,
        MimeMessage msg, CancellationToken cancellationToken)
    {
        var isGood = true;
        foreach (var fileType in fileTypes)
        {
            var bigFiles = lst.Where(x => Regex.IsMatch(x.Name, fileType.FilePattern, RegexOptions.IgnoreCase))
                .Where(x => (x.Length / SizeinMB) > (fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize))
                .ToList();
            if (bigFiles.Any())
            {
                isGood = false;
                var errTxt =
                    $"Hey,\r\n\r\n The following files exceed the Max File Size of {fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize}MB...\r\n"
                    + $"{string.Join("\r\n", bigFiles.Select(x => x.Name))}\r\n...\r\nAutoBot";
                await SendBackMsgAsync(msg, client, errTxt, cancellationToken).ConfigureAwait(false);
            }
        }

        return isGood;
    }
}