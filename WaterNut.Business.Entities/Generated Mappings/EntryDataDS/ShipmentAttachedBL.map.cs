namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentAttachedBLMap : EntityTypeConfiguration<ShipmentAttachedBL>
    {
        public ShipmentAttachedBLMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentAttachedBL");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ShipmentId).HasColumnName("ShipmentId");
              this.Property(t => t.BlId).HasColumnName("BlId");
              this.HasRequired(t => t.Shipment).WithMany(t =>(ICollection<ShipmentAttachedBL>) t.ShipmentAttachedBL).HasForeignKey(d => d.ShipmentId);
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentAttachedBL>) t.ShipmentAttachedBL).HasForeignKey(d => d.BlId);
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
