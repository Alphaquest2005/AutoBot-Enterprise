namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderMap : EntityTypeConfiguration<ShipmentRider>
    {
        public ShipmentRiderMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentRider");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.DocumentDate).HasColumnName("DocumentDate");
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.HasMany(t => t.ShipmentRiderDetails).WithRequired(t => (ShipmentRider)t.ShipmentRider);
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
