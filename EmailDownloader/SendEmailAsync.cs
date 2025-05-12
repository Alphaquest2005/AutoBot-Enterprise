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
                
                File.AppendAllLines(Path.Combine(directory, "EmailResults.txt"), attachments); 
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}