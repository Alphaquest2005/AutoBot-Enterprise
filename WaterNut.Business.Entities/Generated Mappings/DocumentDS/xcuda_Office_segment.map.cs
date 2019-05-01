namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Office_segmentMap : EntityTypeConfiguration<xcuda_Office_segment>
    {
        public xcuda_Office_segmentMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Office_segment");
              this.Property(t => t.Customs_clearance_office_code).HasColumnName("Customs_clearance_office_code").HasMaxLength(20);
              this.Property(t => t.Customs_Clearance_office_name).HasColumnName("Customs_Clearance_office_name").HasMaxLength(255);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Identification).WithOptional(t => (xcuda_Office_segment)t.xcuda_Office_segment);
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
