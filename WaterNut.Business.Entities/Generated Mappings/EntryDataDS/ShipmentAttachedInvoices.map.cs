namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentAttachedInvoicesMap : EntityTypeConfiguration<ShipmentAttachedInvoices>
    {
        public ShipmentAttachedInvoicesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentAttachedInvoices");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ShipmentId).HasColumnName("ShipmentId");
              this.Property(t => t.ShipmentInvoiceId).HasColumnName("ShipmentInvoiceId");
              this.HasRequired(t => t.ShipmentInvoice).WithMany(t =>(ICollection<ShipmentAttachedInvoices>) t.ShipmentAttachedInvoices).HasForeignKey(d => d.ShipmentInvoiceId);
              this.HasRequired(t => t.Shipment).WithMany(t =>(ICollection<ShipmentAttachedInvoices>) t.ShipmentAttachedInvoices).HasForeignKey(d => d.ShipmentId);
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
