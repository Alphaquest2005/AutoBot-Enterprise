namespace LicenseDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xLIC_LicenseMap : EntityTypeConfiguration<xLIC_License>
    {
        public xLIC_LicenseMap()
        {                        
              this.HasKey(t => t.LicenseId);        
              this.ToTable("xLIC_License");
              this.Property(t => t.LicenseId).HasColumnName("LicenseId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasOptional(t => t.xLIC_General_segment).WithRequired(t => (xLIC_License)t.xLIC_License);
              this.HasMany(t => t.xLIC_Lic_item_segment).WithRequired(t => (xLIC_License)t.xLIC_License);
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
