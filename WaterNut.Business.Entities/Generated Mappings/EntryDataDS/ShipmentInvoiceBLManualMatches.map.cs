namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoiceBLManualMatchesMap : EntityTypeConfiguration<ShipmentInvoiceBLManualMatches>
    {
        public ShipmentInvoiceBLManualMatchesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceBLManualMatches");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.BLInvoiceNumber).HasColumnName("BLInvoiceNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.Packages).HasColumnName("Packages");
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
