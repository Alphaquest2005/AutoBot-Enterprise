using Serilog;
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
        List<EmailMapping> emailMappings, ILogger log) // Added ILogger parameter
    {
        string methodName = nameof(GetSubject);
        log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
            methodName, new { Uid = uid, Subject = msg.Subject, MappingCount = emailMappings.Count });

        // Original GetSubject logic - remains synchronous
        foreach (var emailMapping in emailMappings)
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Using mapping with Pattern '{Pattern}', ReplacementValue '{ReplacementValue}'.",
                methodName, "ProcessMapping", emailMapping.Pattern, emailMapping.ReplacementValue);
            var mat = Regex.Match(msg.Subject, emailMapping.Pattern,
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!mat.Success || mat.Groups.Count == 0)
            {
                log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Pattern '{Pattern}' did NOT match or had no groups for subject '{Subject}'. Match success: {MatchSuccess}, Group count: {GroupCount}",
                    methodName, "ProcessMapping", emailMapping.Pattern, msg.Subject, mat.Success, mat.Groups.Count);
                continue;
            }
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Pattern '{Pattern}' matched subject '{Subject}'. Found {GroupCount} groups.",
                methodName, "ProcessMapping", emailMapping.Pattern, msg.Subject, mat.Groups.Count);

            var subject = "";
            for (int i = 1; i < mat.Groups.Count; i++)
            {
                var v = mat.Groups[i];
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Group {GroupIndex} value: '{GroupValue}'",
                    methodName, "ProcessMapping", i, v.Value);
                if (string.IsNullOrEmpty(v.Value) || subject.Contains(v.Value))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Skipping group {GroupIndex} due to empty value or already contained in subject.",
                        methodName, "ProcessMapping", i);
                    continue;
                }
                var g = string.IsNullOrEmpty(emailMapping.ReplacementValue) ? v.Value : emailMapping.ReplacementValue;
                subject += " " + g.Trim();
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Intermediate subject after group {GroupIndex}: '{Subject}'",
                    methodName, "ProcessMapping", i, subject);
            }

            foreach (var regEx in emailMapping.EmailMappingRexExs)
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Applying inner Regex: Pattern='{Pattern}', Value='{Value}' to subject '{Subject}'",
                    methodName, "ApplyInnerRegex", regEx.ReplacementRegex, regEx.ReplacementValue, subject);
                subject = Regex.Replace(subject, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Subject after inner Regex: '{Subject}'",
                    methodName, "ApplyInnerRegex", subject);
            }
            var finalSubject = subject.Trim().Replace("'", "");
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Final extracted subject before returning: '{Subject}'",
                methodName, "Return", finalSubject);

            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Extracted Subject: '{ExtractedSubject}'",
                methodName, 0, finalSubject); // Placeholder for duration
            return new Tuple<string, Email, string>(
                finalSubject,
                new Email(emailUniqueId: Convert.ToInt32(uid.ToString()), subject: msg.Subject.Replace("'", ""),
                    emailDate: msg.Date.DateTime, emailMapping: emailMapping),
                uid.ToString());
        }
        log.Warning("METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Reason: No mapping processed successfully for subject '{Subject}'.",
            methodName, 0, msg.Subject); // Placeholder for duration

        return null;
    }
}