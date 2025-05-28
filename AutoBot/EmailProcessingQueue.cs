using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEntities.Business.Entities;

namespace AutoBot
{
    [Table("EmailProcessingQueue")]
    public class EmailProcessingQueue
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string EmailId { get; set; }
        
        [Required]
        public string Subject { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ProcessingStatus { get; set; } = "Pending";
        
        public int? TotalInvoices { get; set; }
        public int? InvoicesWithIssues { get; set; }
        public double? TotalZeroSum { get; set; }
        
        public int RetryCount { get; set; } = 0;
        public int MaxRetries { get; set; } = 3;
        
        public DateTime? LastProcessedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public string ErrorMessage { get; set; }
        
        [Required]
        public int ApplicationSettingsId { get; set; }
        
        [StringLength(50)]
        public string OriginalImportStatus { get; set; }
        
        [StringLength(50)]
        public string CorrectedImportStatus { get; set; }
        
        public string ProcessingNotes { get; set; }
        
        // Navigation properties
        [ForeignKey("ApplicationSettingsId")]
        public virtual ApplicationSettings ApplicationSettings { get; set; }
    }
    
    public static class ProcessingStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string RequiresCorrection = "RequiresCorrection";
        public const string Correcting = "Correcting";
        public const string CorrectionCompleted = "CorrectionCompleted";
        public const string CorrectionFailed = "CorrectionFailed";
    }
}
