namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentFreightDetailsMap : EntityTypeConfiguration<ShipmentFreightDetails>
    {
        public ShipmentFreightDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentFreightDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FreightId).HasColumnName("FreightId");
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Rate).HasColumnName("Rate");
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").HasMaxLength(50);
              this.HasRequired(t => t.ShipmentFreight).WithMany(t =>(ICollection<ShipmentFreightDetails>) t.ShipmentFreightDetails).HasForeignKey(d => d.FreightId);
              this.HasMany(t => t.ShipmentFreightBLs).WithRequired(t => (ShipmentFreightDetails)t.ShipmentFreightDetails);
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
