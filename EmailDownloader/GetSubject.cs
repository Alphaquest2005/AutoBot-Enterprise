using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using MailKit;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static Tuple<string, Email, string> GetSubject(MimeMessage msg, UniqueId uid,
        List<EmailMapping> emailMappings)
    {
           Console.WriteLine($"GetSubject: Processing subject '{msg.Subject}' with UID {uid}. Received {emailMappings.Count} mapping(s).");
        // Original GetSubject logic - remains synchronous
        foreach (var emailMapping in emailMappings)
        {
               Console.WriteLine($"GetSubject: Using mapping with Pattern '{emailMapping.Pattern}', ReplacementValue '{emailMapping.ReplacementValue}'.");
            var mat = Regex.Match(msg.Subject, emailMapping.Pattern,
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!mat.Success || mat.Groups.Count == 0)
            {
                Console.WriteLine($"GetSubject: Pattern '{emailMapping.Pattern}' did NOT match or had no groups for subject '{msg.Subject}'. Match success: {mat.Success}, Group count: {mat.Groups.Count}");
                continue;
            }
            var subject = "";
            for (int i = 1; i < mat.Groups.Count; i++)
            {
                var v = mat.Groups[i];
                Console.WriteLine($"GetSubject: Group {i} value: '{v.Value}'");
                if (string.IsNullOrEmpty(v.Value) || subject.Contains(v.Value)) continue;
                var g = string.IsNullOrEmpty(emailMapping.ReplacementValue) ? v.Value : emailMapping.ReplacementValue;
                subject += " " + g.Trim();
                Console.WriteLine($"GetSubject: Intermediate subject after group {i}: '{subject}'");
            }

            foreach (var regEx in emailMapping.EmailMappingRexExs)
            {
                Console.WriteLine($"GetSubject: Applying inner Regex: Pattern='{regEx.ReplacementRegex}', Value='{regEx.ReplacementValue}' to subject '{subject}'");
                subject = Regex.Replace(subject, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                Console.WriteLine($"GetSubject: Subject after inner Regex: '{subject}'");
            }
               Console.WriteLine($"GetSubject: Final extracted subject before returning: '{subject.Trim().Replace("'", "")}'");

            return new Tuple<string, Email, string>(
                $"{subject.Trim().Replace("'", "")}",
                new Email(emailUniqueId: Convert.ToInt32(uid.ToString()), subject: msg.Subject.Replace("'", ""),
                    emailDate: msg.Date.DateTime, emailMapping: emailMapping),
                uid.ToString());
        }
           Console.WriteLine($"GetSubject: No mapping processed successfully for subject '{msg.Subject}'. Returning null.");

        return null;
    }
}