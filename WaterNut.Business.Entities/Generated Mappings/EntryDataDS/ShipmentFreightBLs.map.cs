namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentFreightBLsMap : EntityTypeConfiguration<ShipmentFreightBLs>
    {
        public ShipmentFreightBLsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentFreightBLs");
              this.Property(t => t.FreightDetailId).HasColumnName("FreightDetailId");
              this.Property(t => t.BLDetailId).HasColumnName("BLDetailId");
              this.Property(t => t.FreightId).HasColumnName("FreightId");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.Marks).HasColumnName("Marks").IsRequired().HasMaxLength(50);
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.ShipmentFreightDetails).WithMany(t =>(ICollection<ShipmentFreightBLs>) t.ShipmentFreightBLs).HasForeignKey(d => d.FreightDetailId);
              this.HasRequired(t => t.ShipmentBLDetails).WithMany(t =>(ICollection<ShipmentFreightBLs>) t.ShipmentFreightBLs).HasForeignKey(d => d.BLDetailId);
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
