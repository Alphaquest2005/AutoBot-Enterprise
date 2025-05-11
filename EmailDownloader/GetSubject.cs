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
        // Original GetSubject logic - remains synchronous
        foreach (var emailMapping in emailMappings)
        {
            var mat = Regex.Match(msg.Subject, emailMapping.Pattern,
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!mat.Success || mat.Groups.Count == 0) continue;
            var subject = "";
            for (int i = 1; i < mat.Groups.Count; i++)
            {
                var v = mat.Groups[i];
                if (string.IsNullOrEmpty(v.Value) || subject.Contains(v.Value)) continue;
                var g = string.IsNullOrEmpty(emailMapping.ReplacementValue) ? v.Value : emailMapping.ReplacementValue;
                subject += " " + g.Trim();
            }

            foreach (var regEx in emailMapping.EmailMappingRexExs)
            {
                subject = Regex.Replace(subject, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            }

            return new Tuple<string, Email, string>(
                $"{subject.Trim().Replace("'", "")}",
                new Email(emailUniqueId: Convert.ToInt32(uid.ToString()), subject: msg.Subject.Replace("'", ""),
                    emailDate: msg.Date.DateTime, emailMapping: emailMapping),
                uid.ToString());
        }

        return null;
    }
}