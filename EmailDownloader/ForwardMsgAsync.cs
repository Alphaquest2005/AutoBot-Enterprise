using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<bool> ForwardMsgAsync(string emailId, Client clientDetails, string subject, string body,
        string[] contacts, string[] attachments, CancellationToken cancellationToken = default) // Made async
    {
        try
        {
            if (clientDetails.Email == null) return false;
            uint? uID = 0;
            using (var ctx = new CoreEntitiesContext())
            {
                var emailEntity = await ctx.Emails
                    .FirstOrDefaultAsync(x => x.EmailId == emailId && x.MachineName == Environment.MachineName)
                    .ConfigureAwait(false);
                if (emailEntity?.EmailUniqueId != null &&
                    uint.TryParse(emailEntity.EmailUniqueId.ToString(), out uint parsedUid))
                {
                    uID = parsedUid;
                }
            }

            if (uID == 0 || uID == null)
            {
                await SendEmailAsync(clientDetails, null, subject, contacts, body, attachments, cancellationToken)
                    .ConfigureAwait(false);
                return true;
            }

            var msg = await GetMsgAsync(uID.Value, clientDetails, cancellationToken).ConfigureAwait(false);
            await ForwardMsgInternalAsync(msg, clientDetails, subject, body, contacts, attachments, cancellationToken)
                .ConfigureAwait(false);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}