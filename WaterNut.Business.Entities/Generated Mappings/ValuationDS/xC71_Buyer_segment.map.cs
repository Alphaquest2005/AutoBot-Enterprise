namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xC71_Buyer_segmentMap : EntityTypeConfiguration<xC71_Buyer_segment>
    {
        public xC71_Buyer_segmentMap()
        {                        
              this.HasKey(t => t.Identification_segment_Id);        
              this.ToTable("xC71_Buyer_segment");
              this.Property(t => t.Code).HasColumnName("Code").IsRequired().HasMaxLength(255);
              this.Property(t => t.Name).HasColumnName("Name").HasMaxLength(255);
              this.Property(t => t.Address).HasColumnName("Address").HasMaxLength(255);
              this.Property(t => t.Identification_segment_Id).HasColumnName("Identification_segment_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xC71_Identification_segment).WithOptional(t => (xC71_Buyer_segment)t.xC71_Buyer_segment);
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
