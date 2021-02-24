namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoiceDetailsItemAliasMap : EntityTypeConfiguration<ShipmentInvoiceDetailsItemAlias>
    {
        public ShipmentInvoiceDetailsItemAliasMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceDetailsItemAlias");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.INVItemCode).HasColumnName("INVItemCode").HasMaxLength(20);
              this.Property(t => t.POItemCode).HasColumnName("POItemCode").IsRequired().HasMaxLength(20);
              this.Property(t => t.POItemDescription).HasColumnName("POItemDescription").IsRequired().HasMaxLength(255);
              this.HasRequired(t => t.InvoiceDetails).WithOptional(t => (ShipmentInvoiceDetailsItemAlias)t.ItemAlias);
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
