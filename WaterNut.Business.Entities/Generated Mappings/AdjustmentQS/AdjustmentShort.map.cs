namespace AdjustmentQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AdjustmentShortMap : EntityTypeConfiguration<AdjustmentShort>
    {
        public AdjustmentShortMap()
        {                        
              this.HasKey(t => t.EntryDataDetailsId);        
              this.ToTable("AdjustmentShorts");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(15);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.UnitWeight).HasColumnName("UnitWeight");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.CLineNumber).HasColumnName("CLineNumber");
              this.Property(t => t.Downloaded).HasColumnName("Downloaded");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.PreviousCNumber).HasColumnName("PreviousCNumber").HasMaxLength(50);
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(255);
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(4);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.Subject).HasColumnName("Subject");
              this.Property(t => t.EmailDate).HasColumnName("EmailDate");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
              this.Property(t => t.IsReconciled).HasColumnName("IsReconciled");
              this.HasRequired(t => t.AdjustmentEx).WithMany(t =>(ICollection<AdjustmentShort>) t.AdjustmentShorts).HasForeignKey(d => d.EntryData_Id);
              this.HasRequired(t => t.AdjustmentDetail).WithOptional(t => (AdjustmentShort)t.AdjustmentShorts);
              this.HasMany(t => t.ShortAllocations).WithRequired(t => (AdjustmentShort)t.AdjustmentShort);
              this.HasMany(t => t.AsycudaDocumentItemEntryDataDetails).WithRequired(t => (AdjustmentShort)t.AdjustmentShort);
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
