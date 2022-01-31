using System.Collections.Generic;
using CoreEntities.Business.Entities;

namespace EmailDownloader
{
    public class Client
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DataFolder { get; set; }
        public List<EmailMapping> EmailMappings { get; set; }
        public string CompanyName { get; set; }
        public int ApplicationSettingsId { get; set; }
    }
}