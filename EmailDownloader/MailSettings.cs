using MailKit.Security;

namespace EmailDownloader;

public class MailSettings
{
    public string Server { get; set; }
    public int Port { get; set; }
    public SecureSocketOptions Options { get; set; }
    public string Name { get; set; }
}