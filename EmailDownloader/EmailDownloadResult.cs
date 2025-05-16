// In EmailDownloader namespace or a shared types namespace
// This class structure is based on what was returned by the old CheckEmails
// and what the new streaming approach will yield.

using System;
using System.Collections.Generic;

namespace EmailDownloader;

public class EmailProcessingResult
{
    public (string SubjectIdentifier, Email EmailMessage, string UidString) EmailKey { get; }
    public List<System.IO.FileInfo> AttachedFiles { get; }

    // Added properties for sender and recipient information
    public string FromAddress { get; }
    public string FromName { get; }
    public string ToAddress { get; }
    public string ToName { get; }

    public EmailProcessingResult((string SubjectIdentifier, Email EmailMessage, string UidString) emailKey, List<System.IO.FileInfo> attachedFiles, string fromAddress, string fromName, string toAddress, string toName)
    {
        EmailKey = emailKey;
        AttachedFiles = attachedFiles ?? new List<System.IO.FileInfo>(); // Added null check for safety, consistent with previous attempt
    }
}