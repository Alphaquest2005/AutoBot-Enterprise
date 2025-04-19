namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class VolumeUnitsMap : EntityTypeConfiguration<VolumeUnits>
    {
        public VolumeUnitsMap()
        {                        
              this.HasKey(t => t.Name);        
              this.ToTable("VolumeUnits");
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.Property(t => t.RateToLiters).HasColumnName("RateToLiters");
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
