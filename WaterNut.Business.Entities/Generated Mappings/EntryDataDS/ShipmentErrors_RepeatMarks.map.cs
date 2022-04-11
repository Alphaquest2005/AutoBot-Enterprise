namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentErrors_RepeatMarksMap : EntityTypeConfiguration<ShipmentErrors_RepeatMarks>
    {
        public ShipmentErrors_RepeatMarksMap()
        {                        
              this.HasKey(t => new {t.RiderID, t.BLId, t.BLNumber, t.ETA, t.Marks, t.WarehouseCode});        
              this.ToTable("ShipmentErrors-RepeatMarks");
              this.Property(t => t.RiderID).HasColumnName("RiderID").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.BLId).HasColumnName("BLId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.Marks).HasColumnName("Marks").IsRequired().HasMaxLength(50);
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.count).HasColumnName("count");
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
