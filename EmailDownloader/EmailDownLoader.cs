using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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
        public static Dictionary<Tuple<string, Email, string>, List<string>> CheckEmails(Client client, List<string> patterns)
        {
            try
            {
                if(string.IsNullOrEmpty(client.Email)) return new Dictionary<Tuple<string, Email, string>, List<string>>();
                            var imapClient = new ImapClient();
                            imapClient.Connect("ez-brokerage-services.com", 993, SecureSocketOptions.SslOnConnect);
                            imapClient.Authenticate(client.Email, client.Password);
                            var dataFolder = client.DataFolder;
                            imapClient.Inbox.Open(FolderAccess.ReadWrite);
                            var res = DownloadAttachment(imapClient, dataFolder, client.EmailMappings, client, patterns);

                            imapClient.Disconnect(true);
                            return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Dictionary<Tuple<string, Email, string>, List<string>>();
            }
            
        }

        public static void SendEmail(Client client, string directory, string subject, string[] To, string body,
            string[] attachments)
        {
            try
            {
                
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
            foreach (var recipent in to)
            {
               message.To.Add(MailboxAddress.Parse(recipent)); 
            }
            
            message.Subject = subject;

            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.TextBody = body;

            foreach (var attachment in attachments)
            {
               builder.Attachments.Add(attachment);
            }


            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody();
            return message;
        }


        public static Dictionary<Tuple<string, Email, string>, List<string>> DownloadAttachment(ImapClient imapClient, string dataFolder,
            List<EmailMapping> emailMappings, Client client, List<string> patterns)
        {
            try
            {
                var sendNotifications = true;

                var msgFiles = new Dictionary<Tuple<string, Email, string>, List<string>>();
                foreach (var uid in imapClient.Inbox.Search(SearchQuery.NotSeen))
                {
                    var lst = new List<string>();
                    var msg = imapClient.Inbox.GetMessage(uid);

                    if (!emailMappings.Any(x => Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase)))
                    {
                        if (sendNotifications)
                        {
                            imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                            var errTxt = "Hey,\r\n\r\n The System is not configured for this message.\r\n" +
                                         "Check the Subject again or Check Joseph Bartholomew at josephbartholomew@outlook.com to make the necessary changes.\r\n" +
                                         "Thanks\r\n" +
                                         "Ez-Asycuda-Toolkit";
                            SendBackMsg(msg, client, errTxt);
                        }

                        continue;
                    }

                    var subject = GetSubject(msg, uid, emailMappings);

                    if (string.IsNullOrEmpty(subject?.Item1))
                    {
                        if (sendNotifications)
                        {

                            SendEmail(client, null, $"Bug Found",
                                new[] {"Josephbartholomew@outlook.com"},
                                $"Subject not configured for Regex: '{msg.Subject}'", Array.Empty<string>());
                        }

                        imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                        continue;
                    }
                        
                    var desFolder = Path.Combine(dataFolder, subject.Item1, uid.ToString());
                    if(Directory.Exists(desFolder)) Directory.Delete(desFolder, true);
                    Directory.CreateDirectory(desFolder);
                    foreach (var a in msg.Attachments.Where(x => x.ContentType.MediaType != "message"))
                    {
                        if (!a.IsAttachment) continue;
                        SaveAttachmentPart(desFolder, a, lst);
                    }

                    if (lst.Any() && patterns.All(x => !lst.Any(z => Regex.IsMatch(z, x, RegexOptions.IgnoreCase))))
                    {
                        imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                        if (sendNotifications)
                        {
                            var errTxt =
                                "Hey,\r\n\r\n The System is not configured for none of the Attachments in this mail.\r\n" +
                                "Check the file Name of attachments again or Check Joseph Bartholomew at josephbartholomew@outlook.com to make the necessary changes.\r\n" +
                                "Thanks\r\n" +
                                "Ez-Asycuda-Toolkit";
                            SendBackMsg(msg, client, errTxt);
                        }
                    }

                    
                    SaveBodyPart(desFolder, msg, lst);

                    

                 //   imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                    if (msgFiles.ContainsKey(subject))
                    {
                        msgFiles[subject].AddRange(lst);
                    }
                    else
                    {
                        msgFiles.Add(subject, lst);
                    }
                //    imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                }

                return msgFiles;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                    var g = v.Value;
                    subject += " " + g.Trim();
                }

                 
                return new Tuple<string, Email, string>($"{subject.Trim()}", new Email(emailId: Convert.ToInt32(uid.ToString()), subject: msg.Subject, emailDate: msg.Date.DateTime, emailMapping: emailMapping), uid.ToString());

            }

            return null;
        }

        public static bool SendBackMsg(int uID, Client clientDetails, string errtxt)
        {
            try
            {
                var imapClient = new ImapClient();
                imapClient.Connect("ez-brokerage-services.com", 993, SecureSocketOptions.SslOnConnect);
                imapClient.Authenticate(clientDetails.Email, clientDetails.Password);
                var dataFolder = clientDetails.DataFolder;
                imapClient.Inbox.Open(FolderAccess.ReadWrite);
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
            message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            message.To.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            message.Subject = "FWD: " + msg.Subject;

            // now to create our body...
            var builder = new BodyBuilder();
            builder.TextBody = errtxt;
            builder.Attachments.Add(new MessagePart { Message = msg });

            message.Body = builder.ToMessageBody();

            SendEmail(clientDetails, message);
        }

        public static void SendEmail(Client clientDetails, MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Connect("ez-brokerage-services.com", 465, true);
                client.Authenticate(clientDetails.Email, clientDetails.Password);

                client.Send(message);

                client.Disconnect(true);
            }
        }

        private static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z\s]+", rstring);
        }

        private static void SaveAttachmentPart(string dataFolder,  MimeEntity a, List<string> lst)
        {
            var part = (MimePart) a;
            var fileName = CleanFileName(part.FileName);
            var file = Path.Combine(dataFolder, fileName);
            if(File.Exists(file)) File.Delete(file);
            
            using (var stream = File.Create(file))
                part.Content.DecodeTo(stream);
            lst.Add(fileName);
        }

        private static string CleanFileName(string partFileName)
        {
            var newFileName = partFileName.Substring(0,partFileName.LastIndexOf("."));
            var fileExtention = partFileName.Substring(partFileName.LastIndexOf("."));
            var res = ReplaceSpecialChar(newFileName, "-") + fileExtention;
            return res;
        }

        private static void SaveBodyPart(string dataFolder, MimeMessage a, List<string> lst)
        {
            var part = a.BodyParts.OfType<TextPart>().FirstOrDefault();
            var fileName = "Info.txt";

            if (part != null) System.IO.File.WriteAllText(Path.Combine(dataFolder, fileName), part.Text);
            lst.Add(fileName);
        }

        public static bool ForwardMsg(int uID, Client clientDetails, string subject, string body, string[] attachments,
            string[] contacts)
        {
            try
            {
                var imapClient = new ImapClient();
                imapClient.Connect("ez-brokerage-services.com", 993, SecureSocketOptions.SslOnConnect);
                imapClient.Authenticate(clientDetails.Email, clientDetails.Password);
                var dataFolder = clientDetails.DataFolder;
                imapClient.Inbox.Open(FolderAccess.ReadWrite);
                var msg = imapClient.Inbox.GetMessage(new UniqueId(Convert.ToUInt16(uID)));
                imapClient.Disconnect(true);
                if (msg != null)
                {
                    ForwardMsg(msg, clientDetails,subject, body, contacts, attachments);
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

        private static void ForwardMsg(MimeMessage msg, Client clientDetails,string subject, string body, string[] attachments, string[] contacts)
        {
            // construct a new message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
            message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            
            foreach (var recipent in contacts)
            {
                message.To.Add(MailboxAddress.Parse(recipent));
            }
            message.Subject = subject;

            // now to create our body...
            var builder = new BodyBuilder();
            builder.TextBody = body;
            builder.Attachments.Add(new MessagePart { Message = msg });
            

            foreach (var attachment in attachments)
            {
                builder.Attachments.Add(attachment);
            }


            message.Body = builder.ToMessageBody();

            SendEmail(clientDetails, message);
        }
    }
}