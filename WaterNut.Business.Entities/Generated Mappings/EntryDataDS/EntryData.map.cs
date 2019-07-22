namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryDataMap : EntityTypeConfiguration<EntryData>
    {
        public EntryDataMap()
        {                        
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("EntryData");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntryDataDate).HasColumnName("EntryDataDate");
              this.Property(t => t.ImportedTotal).HasColumnName("ImportedTotal");
              this.Property(t => t.ImportedLines).HasColumnName("ImportedLines");
              this.Property(t => t.SupplierId).HasColumnName("SupplierId");
              this.Property(t => t.TotalFreight).HasColumnName("TotalFreight");
              this.Property(t => t.TotalInternalFreight).HasColumnName("TotalInternalFreight");
              this.Property(t => t.TotalWeight).HasColumnName("TotalWeight");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(4);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.HasOptional(t => t.Suppliers).WithMany(t =>(ICollection<EntryData>) t.EntryData).HasForeignKey(d => d.SupplierId);
              this.HasOptional(t => t.FileTypes).WithMany(t =>(ICollection<EntryData>) t.EntryData).HasForeignKey(d => d.FileTypeId);
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (EntryData)t.EntryData);
              this.HasMany(t => t.AsycudaDocuments).WithRequired(t => (EntryData)t.EntryData);
              this.HasMany(t => t.AsycudaDocumentSets).WithRequired(t => (EntryData)t.EntryData);
              this.HasMany(t => t.ContainerEntryData).WithRequired(t => (EntryData)t.EntryData);
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
