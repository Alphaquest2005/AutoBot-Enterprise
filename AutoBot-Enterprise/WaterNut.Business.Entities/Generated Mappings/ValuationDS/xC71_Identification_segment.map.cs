namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xC71_Identification_segmentMap : EntityTypeConfiguration<xC71_Identification_segment>
    {
        public xC71_Identification_segmentMap()
        {                        
              this.HasKey(t => t.Identification_segment_Id);        
              this.ToTable("xC71_Identification_segment");
              this.Property(t => t.Contract_Date).HasColumnName("Contract_Date").HasMaxLength(50);
              this.Property(t => t.Customs_Decision_Date).HasColumnName("Customs_Decision_Date").HasMaxLength(50);
              this.Property(t => t.Yes_7A).HasColumnName("Yes_7A");
              this.Property(t => t.No_7A).HasColumnName("No_7A");
              this.Property(t => t.Yes_7B).HasColumnName("Yes_7B");
              this.Property(t => t.No_7B).HasColumnName("No_7B");
              this.Property(t => t.Yes_7C).HasColumnName("Yes_7C");
              this.Property(t => t.No_7C).HasColumnName("No_7C");
              this.Property(t => t.Yes_8A).HasColumnName("Yes_8A");
              this.Property(t => t.No_8A).HasColumnName("No_8A");
              this.Property(t => t.Yes_8B).HasColumnName("Yes_8B");
              this.Property(t => t.No_8B).HasColumnName("No_8B");
              this.Property(t => t.Yes_9A).HasColumnName("Yes_9A");
              this.Property(t => t.No_9A).HasColumnName("No_9A");
              this.Property(t => t.Yes_9B).HasColumnName("Yes_9B");
              this.Property(t => t.No_9B).HasColumnName("No_9B");
              this.Property(t => t.Identification_segment_Id).HasColumnName("Identification_segment_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Customs_Decision_Number).HasColumnName("Customs_Decision_Number").HasMaxLength(255);
              this.Property(t => t.Box7_Details).HasColumnName("Box7_Details").HasMaxLength(255);
              this.Property(t => t.Box8_Details).HasColumnName("Box8_Details").HasMaxLength(255);
              this.Property(t => t.Box9_Details).HasColumnName("Box9_Details").HasMaxLength(255);
              this.Property(t => t.Contract_Number).HasColumnName("Contract_Number").HasMaxLength(255);
              this.HasRequired(t => t.xC71_Value_declaration_form).WithOptional(t => (xC71_Identification_segment)t.xC71_Identification_segment);
              this.HasOptional(t => t.xC71_Buyer_segment).WithRequired(t => (xC71_Identification_segment)t.xC71_Identification_segment);
              this.HasOptional(t => t.xC71_Declarant_segment).WithRequired(t => (xC71_Identification_segment)t.xC71_Identification_segment);
              this.HasOptional(t => t.xC71_Seller_segment).WithRequired(t => (xC71_Identification_segment)t.xC71_Identification_segment);
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
