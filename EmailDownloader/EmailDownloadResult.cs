// In EmailDownloader namespace or a shared types namespace
// This class structure is based on what was returned by the old CheckEmails
// and what the new streaming approach will yield.

using System;
using System.Collections.Generic;

namespace EmailDownloader;

public class EmailProcessingResult
{
    public Tuple<string, Email, string> EmailKey { get; }
    public List<System.IO.FileInfo> AttachedFiles { get; }

    public EmailProcessingResult(Tuple<string, Email, string> emailKey, List<System.IO.FileInfo> attachedFiles)
    {
        EmailKey = emailKey;
        AttachedFiles = attachedFiles;
    }
}