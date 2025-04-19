namespace LicenseDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xLIC_General_segmentMap : EntityTypeConfiguration<xLIC_General_segment>
    {
        public xLIC_General_segmentMap()
        {                        
              this.HasKey(t => t.General_segment_Id);        
              this.ToTable("xLIC_General_segment");
              this.Property(t => t.Arrival_date).HasColumnName("Arrival_date").HasMaxLength(255);
              this.Property(t => t.Application_date).HasColumnName("Application_date").HasMaxLength(255);
              this.Property(t => t.Expiry_date).HasColumnName("Expiry_date").HasMaxLength(255);
              this.Property(t => t.Importation_date).HasColumnName("Importation_date").HasMaxLength(255);
              this.Property(t => t.General_segment_Id).HasColumnName("General_segment_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Importer_cellphone).HasColumnName("Importer_cellphone").HasMaxLength(255);
              this.Property(t => t.Exporter_address).HasColumnName("Exporter_address").HasMaxLength(255);
              this.Property(t => t.Exporter_country_code).HasColumnName("Exporter_country_code").HasMaxLength(255);
              this.Property(t => t.Importer_code).HasColumnName("Importer_code").HasMaxLength(255);
              this.Property(t => t.Owner_code).HasColumnName("Owner_code").HasMaxLength(255);
              this.Property(t => t.Exporter_email).HasColumnName("Exporter_email").HasMaxLength(255);
              this.Property(t => t.Importer_name).HasColumnName("Importer_name").HasMaxLength(255);
              this.Property(t => t.Importer_contact).HasColumnName("Importer_contact").HasMaxLength(255);
              this.Property(t => t.Exporter_name).HasColumnName("Exporter_name").HasMaxLength(255);
              this.Property(t => t.Exporter_telephone).HasColumnName("Exporter_telephone").HasMaxLength(255);
              this.Property(t => t.Importer_telephone).HasColumnName("Importer_telephone").HasMaxLength(255);
              this.Property(t => t.Exporter_country_name).HasColumnName("Exporter_country_name").HasMaxLength(255);
              this.Property(t => t.Exporter_cellphone).HasColumnName("Exporter_cellphone").HasMaxLength(255);
              this.Property(t => t.Importer_email).HasColumnName("Importer_email").HasMaxLength(255);
              this.HasRequired(t => t.xLIC_License).WithOptional(t => (xLIC_General_segment)t.xLIC_General_segment);
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
