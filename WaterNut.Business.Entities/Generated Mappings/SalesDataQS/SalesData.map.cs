namespace SalesDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SalesDataMap : EntityTypeConfiguration<SalesData>
    {
        public SalesDataMap()
        {                        
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("SalesData");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntryDataDate).HasColumnName("EntryDataDate");
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().IsUnicode(false).HasMaxLength(5);
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.CustomerName).HasColumnName("CustomerName").HasMaxLength(255);
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.AllocatedTotal).HasColumnName("AllocatedTotal");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.AsycudaDocumentId).HasColumnName("AsycudaDocumentId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasMany(t => t.SalesDataDetails).WithRequired(t => (SalesData)t.SalesData);
              this.HasMany(t => t.AsycudaDocumentSets).WithRequired(t => (SalesData)t.SalesData);
              this.HasMany(t => t.SalesDataAllocations).WithRequired(t => (SalesData)t.SalesData);
              this.HasMany(t => t.AsycudaDocuments).WithRequired(t => (SalesData)t.SalesData);
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
