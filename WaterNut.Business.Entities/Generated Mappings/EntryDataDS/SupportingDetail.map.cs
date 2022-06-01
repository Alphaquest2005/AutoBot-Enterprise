namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SupportingDetailMap : EntityTypeConfiguration<SupportingDetail>
    {
        public SupportingDetailMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EntryDataDetails-SupportingDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.CLineNumber).HasColumnName("CLineNumber");
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(50);
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.FileLineNumber).HasColumnName("FileLineNumber");
              this.Property(t => t.EntryDataDetailsKey).HasColumnName("EntryDataDetailsKey").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed)).HasMaxLength(4000);
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
              this.Property(t => t.PreviousEntryDataDetailsId).HasColumnName("PreviousEntryDataDetailsId");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(255);
              this.HasRequired(t => t.EntryDataDetails).WithMany(t =>(ICollection<SupportingDetail>) t.SupportingDetails).HasForeignKey(d => d.EntryDataDetailsId);
              this.HasRequired(t => t.PreviousDataDetails).WithMany(t =>(ICollection<SupportingDetail>) t.PreviousSupportingDetails).HasForeignKey(d => d.PreviousEntryDataDetailsId);
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
