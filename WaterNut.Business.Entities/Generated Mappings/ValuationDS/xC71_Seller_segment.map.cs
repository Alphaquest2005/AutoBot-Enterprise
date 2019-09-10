namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xC71_Seller_segmentMap : EntityTypeConfiguration<xC71_Seller_segment>
    {
        public xC71_Seller_segmentMap()
        {                        
              this.HasKey(t => t.Identification_segment_Id);        
              this.ToTable("xC71_Seller_segment");
              this.Property(t => t.Name).HasColumnName("Name").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Address).HasColumnName("Address").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Identification_segment_Id).HasColumnName("Identification_segment_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xC71_Identification_segment).WithOptional(t => (xC71_Seller_segment)t.xC71_Seller_segment);
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
