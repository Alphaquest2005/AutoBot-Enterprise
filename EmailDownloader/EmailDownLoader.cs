using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

namespace EmailDownloader
{
    public static partial class EmailDownloader
    {
        private const int SizeinMB = 1048576;
        private const int AsycudaMaxFileSize = 4;


        public static string[] GetContacts(string role) => new CoreEntitiesContext().Contacts.Where(x => x.Role == role).Select(x => x.EmailAddress).ToArray();

        public static bool ReturnOnlyUnknownMails { get; set; } = false;
        public static Dictionary<Tuple<string, Email, string>, List<FileInfo>> CheckEmails(Client client)
        {
            var res = new Dictionary<Tuple<string, Email, string>, List<FileInfo>>();
            try
            {
                if(string.IsNullOrEmpty(client.Email)) return new Dictionary<Tuple<string, Email, string>, List<FileInfo>>();
                            var imapClient = GetImapClient(client);

                            DownloadAttachment(imapClient, client.DataFolder, client.EmailMappings, client,ref res);

                            imapClient.Disconnect(true);
                            return res;
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
                return res;
            }
            
        }

        private static ImapClient GetImapClient(Client client)
        {
            try
            {
                
                var imapClient = new ImapClient();
                var mailSettings = GetReadMailSettings(client.Email);
                imapClient.Connect(mailSettings.Server, mailSettings.Port, mailSettings.Options);
                imapClient.Authenticate(client.Email, client.Password);
                imapClient.Inbox.Open(FolderAccess.ReadWrite);
                return imapClient;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void SendEmail(Client client, string directory, string subject, string[] To, string body,
            string[] attachments)
        {
            try
            {
                if (client.Email == null) return;
                MimeMessage msg = CreateMessage(client, subject, To, body, attachments);
                SendEmail(client, msg);
                if(directory != null)
                File.AppendAllLines(Path.Combine(directory, "EmailResults.txt"), attachments);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static MimeMessage CreateMessage(Client client, string subject, string[] to, string body, string[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{client.CompanyName}-AutoBot", client.Email));
           // if (!client.DevMode)
            //{
                foreach (var recipent in to)
                {
                    message.To.Add(MailboxAddress.Parse(recipent));
                }
               // message.Cc.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
            //}
            //else
           // {
               // message.To.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
           // }


            message.Subject = subject;

            var builder = new BodyBuilder
            {
                // Set the plain-text version of the message text
                TextBody = body
            };

            foreach (var attachment in attachments)
            {
                try
                {
                    if (File.Exists(attachment)) builder.Attachments.Add(attachment);
                }
                catch (Exception)
                {

                }

            }


            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody();
            return message;
        }


        public static void DownloadAttachment(ImapClient imapClient,
            string dataFolder,
            List<EmailMapping> emailMappings, Client client,
            ref Dictionary<Tuple<string, Email, string>, List<FileInfo>> msgFiles)
        {
            try
            {
                var sendNotifications = client.NotifyUnknownMessages ;

                //var msgFiles = new Dictionary<Tuple<string, Email, string>, List<string>>();

                var uniqueIds = imapClient.Inbox.Search(SearchQuery.NotSeen).ToList();
                var existingEmails = new List<Emails>();
                if (ReturnOnlyUnknownMails)
                {
                    existingEmails = new CoreEntitiesContext().Emails
                        .Where(x => x.ApplicationSettingsId == client.ApplicationSettingsId).ToList();
                }

                foreach (var uid in uniqueIds)
                {
                    var lst = new List<FileInfo>();
                    var msg = imapClient.Inbox.GetMessage(uid);

                    if (ReturnOnlyUnknownMails)
                    {
                        if(existingEmails.FirstOrDefault(x => x.EmailDate == msg.Date.DateTime && x.Subject == msg.Subject) != null) continue;
                    }

                    var emailsFound =
                        emailMappings
                            .Where(x => Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            .OrderByDescending(x => x.Pattern.Length)
                            .ToList();

                    if (!emailsFound.Any())
                    {
                        if (sendNotifications)
                        {
                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            var errTxt = "Hey,\r\n\r\n The System is not configured for this message.\r\n" +
                                         "Check the Subject again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                         "Thanks\r\n" +
                                         "Ez-Asycuda-Toolkit";
                            SendBackMsg(msg, client, errTxt);
                        }

                        continue;
                    }

                    foreach (var emailMapping in emailsFound.Take(1))// taking first one because i think there should only be one real match but mutiple matches possible
                    {
                        var subject = GetSubject(msg, uid, new List<EmailMapping>() {emailMapping});

                        if (string.IsNullOrEmpty(subject?.Item1))
                        {
                            if (sendNotifications)
                            {

                                SendEmail(client, null, $"Bug Found",
                                    GetContacts("Developer"),
                                    $"Subject not configured for Regex: '{msg.Subject}'", Array.Empty<string>());
                            }

                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            continue;
                        }

                        var desFolder = Path.Combine(dataFolder, subject.Item1, uid.ToString());
                        if (Directory.Exists(desFolder)) Directory.Delete(desFolder, true);
                        Directory.CreateDirectory(desFolder);
                        foreach (var a in msg.Attachments.Where(x => x.ContentType.MediaType != "message"))
                        {
                            if (!a.IsAttachment) continue;

                           
                            SaveAttachmentPart(desFolder, a, lst);
                        }
                        SaveBodyPart(desFolder, msg, lst);

                        var fileTypes = emailMapping.EmailFileTypes.Select(x => x.FileTypes)
                            .Where(x => lst.Any(z => Regex.IsMatch(z.Name, x.FilePattern, RegexOptions.IgnoreCase)))
                            .Where(x => x.FileImporterInfos != null)
                            .ToList();

                        if (lst.Any(x => x.Name != "Info.txt") && fileTypes.All(x => x.FileImporterInfos.EntryType == "Info"))//TODO: take this out
                        {
                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            if (sendNotifications)
                            {
                                var errTxt =
                                    "Hey,\r\n\r\n The System is not configured for none of the Attachments in this mail.\r\n" +
                                    "Check the file Name of attachments again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                    "Thanks\r\n" +
                                    "AutoBot";
                                SendBackMsg(msg, client, errTxt);
                            }
                        }

                        if (!CheckFileSizeLimit(client, fileTypes, lst, msg))
                        {
                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            continue;
                        }

                        subject.Item2.FileTypes = fileTypes;


                        imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                      
                       
                        msgFiles.Add(subject, lst);
                     

                        imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                    }

                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool CheckFileSizeLimit(Client client, List<FileTypes> fileTypes, List<FileInfo> lst,
            MimeMessage msg)
        {
            var isGood = true;
            foreach (var fileType in fileTypes)
            {
                var bigFiles = lst.Where(x =>
                        Regex.IsMatch(x.Name, fileType.FilePattern, RegexOptions.IgnoreCase))
                    .Where(x => (x.Length / SizeinMB) > (fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize))
                    .ToList();
                if (bigFiles.Any())
                {
                    isGood = false;
                    var errTxt =
                        $"Hey,\r\n\r\n The following files exceed the Max File Size of {fileType.MaxFileSizeInMB ?? AsycudaMaxFileSize}MB the Attachments in this mail.\r\n" +
                        $"{bigFiles.Select(x => x.Name).Aggregate((o, n) => $"{o}\r\n{n}\r\n")}\r\n" +
                        "Check the file Name of attachments again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                        "Thanks\r\n" +
                        "AutoBot";

                    SendBackMsg(msg, client, errTxt);
                }
            }

            return isGood;
        }

        private static Tuple<string, Email, string> GetSubject(MimeMessage msg, UniqueId uid, List<EmailMapping> emailMappings)
        {
            //var patterns = new string[]
            //{
            //    @"(\b\d{1,2}\D{0,3})?\b(?<Month>Jan(?:uary)? |Feb(?:ruary)? |Mar(?:ch)? |Apr(?:il)? |May |Jun(?:e)? |Jul(?:y)? |Aug(?:ust)? |Sep(?:tember)? |Oct(?:ober)? |(Nov |Dec)(?:ember)? )[a-zA-Z\s]*(?<Year>(19[7-9]\d|20\d{2})|\d{2})?(?<![Discrepancy])",
            //    @"Shipment:\s(?<Subject>.+)",
            //    @"Fw: (?<Subject>[A-Z][a-z]+).*(?<=Discrepancy)|Fw: (?<Subject>[A-Z][A-Z]+).*(?<=Discrepancy)|(?![Fw: ])(?<Subject>^[A-Z][a-z]+).*(?<=Discrepancy)|.*(?<=Warranty).*\-\s(?<Subject>[A-Z][A-Z]+)|.*(?<Subject>[0-9][A-Z]+).*(?<=Discrepancy)",
            //    @".*(?<Subject>[0-9][A-Z]+).*(?<=Discrepancy)",
            //    @".*Error:\s?(?<Subject>.+)",

            //};

            foreach (var emailMapping in emailMappings)
            {
                var mat = Regex.Match(msg.Subject,
                    emailMapping.Pattern,
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success || mat.Groups.Count == 0) continue;
                var subject = "";
                for (int i = 1; i < mat.Groups.Count; i++)
                {
                    var v = mat.Groups[i];
                    if(string.IsNullOrEmpty(v.Value) || subject.Contains(v.Value)) continue;
                    var g = string.IsNullOrEmpty(emailMapping.ReplacementValue)
                            ? v.Value
                            : emailMapping.ReplacementValue;
                    subject += " " + g.Trim();
                }

                foreach (var regEx in emailMapping.EmailMappingRexExs)
                {

                    subject = Regex.Replace(subject, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                }


                return new Tuple<string, Email, string>($"{subject.Trim().Replace("'", "")}", new Email(emailUniqueId: Convert.ToInt32(uid.ToString()), subject: msg.Subject.Replace("'", ""), emailDate: msg.Date.DateTime, emailMapping: emailMapping), uid.ToString());

            }

            return null;
        }

        public static bool SendBackMsg(string emailId, Client clientDetails, string errtxt)
        {
            try
            {
                if(clientDetails.Email == null) return false;
                var imapClient = GetImapClient(clientDetails);

                var uID = new CoreEntitiesContext().Emails.FirstOrDefault(x => x.EmailId == emailId && x.MachineName == Environment.MachineName)?.EmailUniqueId;
                if (uID == null) return false;
                var msg = imapClient.Inbox.GetMessage(new UniqueId(Convert.ToUInt16(uID)));
                imapClient.Disconnect(true);
                if (msg != null)
                {
                    SendBackMsg(msg, clientDetails, errtxt);
                }
                else
                {
                    // msg not found
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private static void SendBackMsg(MimeMessage msg, Client clientDetails, string errtxt)
        {
            // construct a new message
            if (clientDetails.Email == null) return;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
            if (!clientDetails.DevMode)
            {
                message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name,
                    msg.From.Mailboxes.FirstOrDefault().Address));

                message.To.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
               // message.Cc.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
            }
            else
            {
               // message.To.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
            }

            message.Subject = "FWD: " + msg.Subject;

            // now to create our body...
            var builder = new BodyBuilder
            {
                TextBody = errtxt
            };
            builder.Attachments.Add(new MessagePart { Message = msg });

            message.Body = builder.ToMessageBody();

            SendEmail(clientDetails, message);
        }

        public static void SendEmail(Client clientDetails, MimeMessage message)
        {
            try
            {
                if (clientDetails.Email == null) return ;
                var mailSettings = GetSendMailSettings(clientDetails.Email);
                using (var client = new SmtpClient())
                {
                    client.Connect(mailSettings.Server, mailSettings.Port, mailSettings.Options);
                    client.Authenticate(clientDetails.Email, clientDetails.Password);

                    client.Send(message);

                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }

        }

        private static List<MailSettings> _sendEmailSettings = new List<MailSettings>()
            {
                new MailSettings(){Name = "auto-brokerage.com",Server = "smtppro.zoho.com", Port = 465, Options = SecureSocketOptions.SslOnConnect},
                new MailSettings(){Name = "outlook.com", Server = @"smtp-mail.outlook.com", Port = 587, Options = SecureSocketOptions.StartTls}
            };

        private static List<MailSettings> _readEmailSettings = new List<MailSettings>()
        {
            new MailSettings(){Name = "auto-brokerage.com",Server = "imappro.zoho.com", Port = 993, Options = SecureSocketOptions.SslOnConnect},
            new MailSettings(){Name = "outlook.com", Server = @"outlook.office365.com", Port = 993, Options = SecureSocketOptions.Auto}
        };

        private static MailSettings GetSendMailSettings(string email) => _sendEmailSettings.First(x => email.ToUpper().Contains(x.Name.ToUpper()));
        private static MailSettings GetReadMailSettings(string email) => _readEmailSettings.First(x => email.ToUpper().Contains(x.Name.ToUpper()));
        

        private static void SaveAttachmentPart(string dataFolder,  MimeEntity a, List<FileInfo> lst)
        {
            var part = (MimePart) a;
            var fileName = CleanFileName(part.FileName);
            var file = Path.Combine(dataFolder, fileName);
            if(File.Exists(file)) file = GetNextFileName(file);
            
            using (var stream = File.Create(file))
                part.Content.DecodeTo(stream);
            lst.Add( new FileInfo(file));
        }

        private static string GetNextFileName(string file)
        {
            var fileinfo = new FileInfo(file);
            for (int i = 1; i < 1000; i++)
            {
                var nfileName = Path.Combine(fileinfo.DirectoryName,  $"{fileinfo.Name.Replace(fileinfo.Extension,"")}({i}){fileinfo.Extension.ToLower()}" );
                if (!File.Exists(nfileName)) return nfileName;
            }

            return null;
        }

        private static string CleanFileName(string partFileName)
        {
            var newFileName = partFileName.Substring(0,partFileName.LastIndexOf("."));
            var fileExtention = partFileName.Substring(partFileName.LastIndexOf("."));
            var res = newFileName.ReplaceSpecialChar("-") + fileExtention;
            return res;
        }

        private static void SaveBodyPart(string dataFolder, MimeMessage a, List<FileInfo> lst)
        {
            var part = a.BodyParts.OfType<TextPart>().FirstOrDefault();
            var fileName = "Info.txt";
            var file = Path.Combine(dataFolder, fileName);
            if (part != null)
            {
                
                System.IO.File.WriteAllText(file, part.Text);
            }

            lst.Add(new FileInfo(file));
        }

        public static bool ForwardMsg(string emailId, Client clientDetails, string subject, string body,string[] contacts ,string[] attachments
            )
        {
            try
            {
                if (clientDetails.Email == null) return false;
                var uID = new CoreEntitiesContext().Emails.FirstOrDefault(x => x.EmailId == emailId && x.MachineName == Environment.MachineName)?.EmailUniqueId??0;
                if (uID == 0)
                {
                    //throw new ApplicationException($"Email not found for {emailId}");
                    SendEmail(clientDetails,null, subject, contacts, body, attachments  );
                    return true;
                }
                var msg = GetMsg(uID, clientDetails);
                
                ForwardMsg(msg, clientDetails,subject, body, contacts ,attachments);
               
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private static MimeMessage GetMsg(int uID, Client clientDetails)
        {
            var imapClient = GetImapClient(clientDetails);
            var msg = GetMessageOrDefault(uID, imapClient);
            imapClient.Disconnect(true);
            return msg;
        }

        private static MimeMessage GetMessageOrDefault(int uID, ImapClient imapClient)
        {
            MimeMessage msg = new MimeMessage();
            try
            {
                msg = imapClient.Inbox.GetMessage(new UniqueId(Convert.ToUInt16(uID)));
            }
            catch (Exception)
            {
            }
            return msg;
                
               
        }

        private static void ForwardMsg(MimeMessage msg, Client clientDetails, string subject, string body,
            string[] contacts, string[] attachments)
        {
            if (clientDetails.Email == null) return ;
            // construct a new message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
            if (!clientDetails.DevMode)
            {
                message.ReplyTo.Add(new MailboxAddress(msg.From?.FirstOrDefault()?.Name ?? "No Sender Found",
                    msg.From?.Mailboxes?.FirstOrDefault()?.Address ?? GetContacts("Developer").FirstOrDefault()));

                foreach (var recipent in contacts.Distinct())
                {
                    message.To.Add(MailboxAddress.Parse(recipent));
                }

               // message.Cc.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
            }
            else
            {
               // message.To.Add(new MailboxAddress("Joseph Bartholomew", "Joseph@auto-brokerage.com"));
            }

            message.Subject = subject;

            // now to create our body...
            var builder = new BodyBuilder
            {
                TextBody = body
            };
            if (msg != new MimeMessage()) builder.Attachments.Add(new MessagePart { Message = msg });
            

            foreach (var attachment in attachments)
            {
                if(File.Exists(attachment))
                    builder.Attachments.Add(attachment);
            }


            message.Body = builder.ToMessageBody();

            SendEmail(clientDetails, message);
        }

        public static bool ForwardMsgToSender(int uID, Client client, string subject, string body, string[] attachments)
        {
            try
            {
                if (client.Email == null) return false;
                var msg = GetMsg(uID, client);
                if (msg != null)
                {
                    var contacts = msg.From.Mailboxes.Select(x => x.Address).ToArray();
                    ForwardMsg(msg, client, subject, body, contacts, attachments);
                }
                else
                {
                    // msg not found
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    internal class MailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public SecureSocketOptions Options { get; set; }
        public string Name { get; set; }
    }
}