using System;
using Serilog;
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
        MimeMessage msg, ILogger log, CancellationToken cancellationToken) // Added ILogger parameter
    {
        string methodName = nameof(CheckFileSizeLimitAsync);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { ClientEmail = client.Email, FileTypeCount = fileTypes.Count, DownloadedFileCount = lst.Count });

        var isGood = true;
        try
        {
            foreach (var fileType in fileTypes)
            {
                var bigFiles = lst.Where(x => Regex.IsMatch(x.Name, fileType.FilePattern, RegexOptions.IgnoreCase))
                    .Where(x => (x.Length / SizeinMB) > (fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize))
                    .ToList();
                if (bigFiles.Any())
                {
                    isGood = false;
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): File size limit exceeded for FileType pattern '{FilePattern}'. Exceeding files: {Files}",
                        methodName, "CheckLimit", fileType.FilePattern, string.Join(", ", bigFiles.Select(f => $"{f.Name} ({f.Length} bytes)")));

                    var errTxt =
                        $"Hey,\r\n\r\n The following files exceed the Max File Size of {fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize}MB...\r\n"
                        + $"{string.Join("\r\n", bigFiles.Select(x => x.Name))}\r\n...\r\nAutoBot";

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        "SendBackMsgAsync (File Size Limit Exceeded)", "ASYNC_EXPECTED");
                    await SendBackMsgAsync(msg, client, errTxt, log, cancellationToken).ConfigureAwait(false); // Pass the logger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "SendBackMsgAsync (File Size Limit Exceeded)", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                }
            }

            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {IsGood}",
                methodName, 0, isGood); // Placeholder for duration
            return isGood;
        }
        catch (Exception ex)
        {
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, 0, ex.Message); // Placeholder for duration
            throw; // Re-throw the exception after logging
        }
    }
}