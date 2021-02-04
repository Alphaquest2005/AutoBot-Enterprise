using System;
using System.Collections.Generic;
using CoreEntities.Business.Entities;

namespace EmailDownloader
{
    public class Email
    {
        public int EmailId { get; }
        public string Subject { get; }
        public DateTime EmailDate { get; }

        public Email(int emailId, string subject, DateTime emailDate, EmailMapping emailMapping)
        {
            this.EmailId = emailId;
            this.Subject = subject;
            this.EmailDate = emailDate;
            this.EmailMapping = emailMapping;
          

        }

        public List<FileTypes> FileTypes { get; set; }
        
        public EmailMapping EmailMapping { get; }
    }
}