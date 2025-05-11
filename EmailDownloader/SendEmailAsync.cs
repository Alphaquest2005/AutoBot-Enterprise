using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task SendEmailAsync(Client client, string directory, string subject, string[] To, string body,
        string[] attachments, CancellationToken cancellationToken = default) // Added CancellationToken
    {
        try
        {
            if (client.Email == null) return;
            MimeMessage msg = CreateMessage(client, subject, To, body, attachments); // CreateMessage is CPU-bound
            await SendEmailInternalAsync(client, msg, cancellationToken)
                .ConfigureAwait(false); // Use internal async version
            if (directory != null && attachments.Any())
            {
                // For .NET 4.8, File.AppendAllLinesAsync might not exist or have the desired overload.
                // Using a helper if needed, or Task.Run for simplicity if AppendAllLines is sync.
                string content = string.Join(Environment.NewLine, attachments) + Environment.NewLine;
                await WriteAllTextAsync(Path.Combine(directory, "EmailResults.txt"), content, true, cancellationToken)
                    .ConfigureAwait(false); // true for append
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}