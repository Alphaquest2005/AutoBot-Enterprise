namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryDataFilesMap : EntityTypeConfiguration<EntryDataFiles>
    {
        public EntryDataFilesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EntryDataFiles");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.SourceRow).HasColumnName("SourceRow").IsRequired();
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").HasMaxLength(50);
              this.Property(t => t.EntryDataDate).HasColumnName("EntryDataDate");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(20);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(15);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(255);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.TotalCost).HasColumnName("TotalCost");
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.FileType).HasColumnName("FileType").IsRequired().HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
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
