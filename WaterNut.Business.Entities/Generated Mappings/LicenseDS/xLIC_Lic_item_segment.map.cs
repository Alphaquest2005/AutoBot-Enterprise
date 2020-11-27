namespace LicenseDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xLIC_Lic_item_segmentMap : EntityTypeConfiguration<xLIC_Lic_item_segment>
    {
        public xLIC_Lic_item_segmentMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("xLIC_Lic_item_segment");
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(255);
              this.Property(t => t.Commodity_code).HasColumnName("Commodity_code").IsRequired().HasMaxLength(255);
              this.Property(t => t.Quantity_requested).HasColumnName("Quantity_requested");
              this.Property(t => t.Origin).HasColumnName("Origin").HasMaxLength(255);
              this.Property(t => t.Unit_of_measurement).HasColumnName("Unit_of_measurement").HasMaxLength(255);
              this.Property(t => t.Quantity_to_approve).HasColumnName("Quantity_to_approve");
              this.Property(t => t.LicenseId).HasColumnName("LicenseId");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasRequired(t => t.xLIC_License).WithMany(t =>(ICollection<xLIC_Lic_item_segment>) t.xLIC_Lic_item_segment).HasForeignKey(d => d.LicenseId);
              this.HasOptional(t => t.TODO_LicenceAvailableQty).WithRequired(t => (xLIC_Lic_item_segment)t.xLIC_Lic_item_segment);
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
