namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentBLFreightMap : EntityTypeConfiguration<ShipmentBLFreight>
    {
        public ShipmentBLFreightMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentBLFreight");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.FreightInvoiceId).HasColumnName("FreightInvoiceId");
              this.Property(t => t.BLId).HasColumnName("BLId");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentBLFreight>) t.ShipmentBLFreight).HasForeignKey(d => d.FreightInvoiceId);
              this.HasRequired(t => t.ShipmentFreight).WithMany(t =>(ICollection<ShipmentBLFreight>) t.ShipmentBLFreight).HasForeignKey(d => d.FreightInvoiceId);
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
