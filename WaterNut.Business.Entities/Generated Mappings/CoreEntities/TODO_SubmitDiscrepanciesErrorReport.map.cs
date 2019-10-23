namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_SubmitDiscrepanciesErrorReportMap : EntityTypeConfiguration<TODO_SubmitDiscrepanciesErrorReport>
    {
        public TODO_SubmitDiscrepanciesErrorReportMap()
        {                        
              this.HasKey(t => new {t.InvoiceNo, t.ItemNumber, t.ItemDescription, t.Cost, t.subject, t.ApplicationSettingsId, t.Quantity});        
              this.ToTable("TODO-SubmitDiscrepanciesErrorReport");
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.PreviousCNumber).HasColumnName("PreviousCNumber").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.comment).HasColumnName("comment").HasMaxLength(255);
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().IsUnicode(false).HasMaxLength(9);
              this.Property(t => t.subject).HasColumnName("subject").IsRequired();
              this.Property(t => t.emailDate).HasColumnName("emailDate");
              this.Property(t => t.emailId).HasColumnName("emailId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Quantity).HasColumnName("Quantity");
             // Tracking Properties
    			this.Ignore(t => t.TrackingState);
    			this.Ignore(t => t.ModifiedProperties);
    
    
             // IIdentifibleEntity
                this.Ignore(t => t.EntityId);
                this.Ignore(t => t.EntityName); 
    
                this.Ignore(t => t.EntityKey);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
