using System;

namespace EmailDownloader
{
    public class Email
    {
        public int EmailId { get; }
        public string Subject { get; }
        public DateTime EmailDate { get; }

        public Email(int EmailId, string Subject, DateTime EmailDate)
        {
            this.EmailId = EmailId;
            this.Subject = Subject;
            this.EmailDate = EmailDate;
            
        }
    }
}