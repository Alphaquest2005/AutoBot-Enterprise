namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoiceDetailPOSSuggestionsMap : EntityTypeConfiguration<ShipmentInvoiceDetailPOSSuggestions>
    {
        public ShipmentInvoiceDetailPOSSuggestionsMap()
        {                        
              this.HasKey(t => t.id);        
              this.ToTable("ShipmentInvoiceDetailPOSSuggestions");
              this.Property(t => t.id).HasColumnName("id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").HasMaxLength(20);
              this.Property(t => t.POSCode).HasColumnName("POSCode").IsRequired().HasMaxLength(20);
              this.Property(t => t.SupplierDescription).HasColumnName("SupplierDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.Property(t => t.InvoiceDetailId).HasColumnName("InvoiceDetailId");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.HasRequired(t => t.InvoiceDetails).WithMany(t =>(ICollection<ShipmentInvoiceDetailPOSSuggestions>) t.ShipmentInvoiceDetailPOSSuggestions).HasForeignKey(d => d.InvoiceDetailId);
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
