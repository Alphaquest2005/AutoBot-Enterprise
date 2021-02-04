namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderInvoiceMap : EntityTypeConfiguration<ShipmentRiderInvoice>
    {
        public ShipmentRiderInvoiceMap()
        {                        
              this.HasKey(t => t.id);        
              this.ToTable("ShipmentRiderInvoice");
              this.Property(t => t.id).HasColumnName("id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RiderLineID).HasColumnName("RiderLineID");
              this.Property(t => t.RiderID).HasColumnName("RiderID");
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").HasMaxLength(255);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").HasMaxLength(50);
              this.HasRequired(t => t.ShipmentRider).WithMany(t =>(ICollection<ShipmentRiderInvoice>) t.ShipmentRiderInvoice).HasForeignKey(d => d.RiderID);
              this.HasRequired(t => t.ShipmentInvoice).WithMany(t =>(ICollection<ShipmentRiderInvoice>) t.ShipmentRiderInvoice).HasForeignKey(d => d.InvoiceId);
              this.HasRequired(t => t.ShipmentRiderDetails).WithMany(t =>(ICollection<ShipmentRiderInvoice>) t.ShipmentRiderInvoice).HasForeignKey(d => d.RiderLineID);
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
