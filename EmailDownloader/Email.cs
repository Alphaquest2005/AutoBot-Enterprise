using System;
using System.Collections.Generic;
using CoreEntities.Business.Entities;

namespace EmailDownloader
{
    public class Email
    {
        public int EmailUniqueId { get; }
        public string Subject { get; }
        public DateTime EmailDate { get; }

        public Email(int emailUniqueId, string subject, DateTime emailDate, EmailMapping emailMapping)
        {
            this.EmailUniqueId = emailUniqueId;
            this.Subject = subject;
            this.EmailDate = emailDate;
            this.EmailMapping = emailMapping;
            this.FileTypes = new List<CoreEntities.Business.Entities.FileTypes>(); // Initialize the list
        }

        public string EmailId => Subject + "--" + EmailDate.ToString("yyyy-MM-dd-HH:mm:ss");

        public List<FileTypes> FileTypes { get; set; }
        
        public EmailMapping EmailMapping { get; }
    }
}