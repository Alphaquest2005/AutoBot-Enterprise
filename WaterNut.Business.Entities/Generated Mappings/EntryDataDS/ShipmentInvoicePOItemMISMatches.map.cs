namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoicePOItemMISMatchesMap : EntityTypeConfiguration<ShipmentInvoicePOItemMISMatches>
    {
        public ShipmentInvoicePOItemMISMatchesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoicePOItemMISMatches");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.PODetailsId).HasColumnName("PODetailsId");
              this.Property(t => t.INVDetailsId).HasColumnName("INVDetailsId");
              this.Property(t => t.POId).HasColumnName("POId");
              this.Property(t => t.INVId).HasColumnName("INVId");
              this.Property(t => t.PONumber).HasColumnName("PONumber").HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").HasMaxLength(50);
              this.Property(t => t.POItemCode).HasColumnName("POItemCode").HasMaxLength(20);
              this.Property(t => t.INVItemCode).HasColumnName("INVItemCode").HasMaxLength(20);
              this.Property(t => t.PODescription).HasColumnName("PODescription").HasMaxLength(255);
              this.Property(t => t.INVDescription).HasColumnName("INVDescription").HasMaxLength(255);
              this.Property(t => t.POCost).HasColumnName("POCost");
              this.Property(t => t.INVCost).HasColumnName("INVCost");
              this.Property(t => t.POQuantity).HasColumnName("POQuantity");
              this.Property(t => t.INVQuantity).HasColumnName("INVQuantity");
              this.Property(t => t.POTotalCost).HasColumnName("POTotalCost");
              this.Property(t => t.INVTotalCost).HasColumnName("INVTotalCost");
              this.HasOptional(t => t.ShipmentInvoicePOs).WithMany(t =>(ICollection<ShipmentInvoicePOItemMISMatches>) t.POMISMatches).HasForeignKey(d => new {d.INVId, d.POId});
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
