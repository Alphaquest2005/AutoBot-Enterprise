using System.Collections.Generic;

namespace EmailDownloader
{
    public class Client
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DataFolder { get; set; }
        public List<string> EmailMappings { get; set; }
        public string CompanyName { get; set; }
    }
}