namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_ERRReport_EntryDataDetailsMap : EntityTypeConfiguration<TODO_ERRReport_EntryDataDetails>
    {
        public TODO_ERRReport_EntryDataDetailsMap()
        {                        
              this.HasKey(t => new {t.InvoiceNo, t.InvoiceDate, t.ItemNumber, t.ItemDescription, t.Error});        
              this.ToTable("TODO-ERRReport-EntryDataDetails");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.Error).HasColumnName("Error").IsRequired().IsUnicode(false).HasMaxLength(21);
              this.Property(t => t.Info).HasColumnName("Info").HasMaxLength(128);
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
