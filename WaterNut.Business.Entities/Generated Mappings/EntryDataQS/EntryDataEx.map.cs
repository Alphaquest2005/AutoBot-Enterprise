namespace EntryDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryDataExMap : EntityTypeConfiguration<EntryDataEx>
    {
        public EntryDataExMap()
        {                        
              this.HasKey(t => t.InvoiceNo);        
              this.ToTable("EntryDataEx");
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.ImportedLines).HasColumnName("ImportedLines");
              this.Property(t => t.TotalLines).HasColumnName("TotalLines");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(4);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().IsUnicode(false).HasMaxLength(9);
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").HasMaxLength(100);
              this.Property(t => t.ImportedTotal).HasColumnName("ImportedTotal");
              this.Property(t => t.ExpectedTotal).HasColumnName("ExpectedTotal");
              this.HasMany(t => t.AsycudaDocumentSets).WithRequired(t => (EntryDataEx)t.EntryDataEx);
              this.HasMany(t => t.AsycudaDocuments).WithRequired(t => (EntryDataEx)t.EntryDataEx);
              this.HasMany(t => t.EntryDataDetailsExs).WithRequired(t => (EntryDataEx)t.EntryDataEx);
              this.HasMany(t => t.ContainerEntryDatas).WithRequired(t => (EntryDataEx)t.EntryDataEx);
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
