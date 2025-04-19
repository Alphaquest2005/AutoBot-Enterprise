namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentBLInvoiceMap : EntityTypeConfiguration<ShipmentBLInvoice>
    {
        public ShipmentBLInvoiceMap()
        {                        
              this.HasKey(t => t.id);        
              this.ToTable("ShipmentBLInvoice");
              this.Property(t => t.id).HasColumnName("id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.BLDetailsLineID).HasColumnName("BLDetailsLineID");
              this.Property(t => t.BLID).HasColumnName("BLID");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").HasMaxLength(50);
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.HasOptional(t => t.ShipmentInvoice).WithMany(t =>(ICollection<ShipmentBLInvoice>) t.ShipmentBLInvoice).HasForeignKey(d => d.InvoiceId);
              this.HasOptional(t => t.ShipmentBLDetails).WithMany(t =>(ICollection<ShipmentBLInvoice>) t.ShipmentBLInvoice).HasForeignKey(d => d.BLDetailsLineID);
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentBLInvoice>) t.ShipmentBLInvoice).HasForeignKey(d => d.BLID);
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
