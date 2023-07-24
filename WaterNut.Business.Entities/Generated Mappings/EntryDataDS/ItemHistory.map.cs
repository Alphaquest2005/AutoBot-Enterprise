namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ItemHistoryMap : EntityTypeConfiguration<ItemHistory>
    {
        public ItemHistoryMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ItemHistory");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").HasMaxLength(500);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.TransactionId).HasColumnName("TransactionId").HasMaxLength(255);
              this.Property(t => t.TransactionType).HasColumnName("TransactionType").HasMaxLength(50);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(50);
              this.Property(t => t.Date).HasColumnName("Date");
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.Comments).HasColumnName("Comments").HasMaxLength(1000);
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
