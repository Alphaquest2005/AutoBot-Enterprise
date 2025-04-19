namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderExMap : EntityTypeConfiguration<ShipmentRiderEx>
    {
        public ShipmentRiderExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentRiderEx");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.DocumentDate).HasColumnName("DocumentDate");
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode");
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
              this.Property(t => t.GrossWeightKg).HasColumnName("GrossWeightKg");
              this.Property(t => t.CubicFeet).HasColumnName("CubicFeet");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Code).HasColumnName("Code").HasMaxLength(50);
              this.HasRequired(t => t.ShipmentRider).WithOptional(t => (ShipmentRiderEx)t.ShipmentRiderEx);
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
