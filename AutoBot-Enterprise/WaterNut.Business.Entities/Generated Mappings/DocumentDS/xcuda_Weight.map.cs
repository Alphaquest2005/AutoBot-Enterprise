namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_WeightMap : EntityTypeConfiguration<xcuda_Weight>
    {
        public xcuda_WeightMap()
        {                        
              this.HasKey(t => t.Valuation_Id);        
              this.ToTable("xcuda_Weight");
              this.Property(t => t.Gross_weight).HasColumnName("Gross_weight");
              this.Property(t => t.Valuation_Id).HasColumnName("Valuation_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Valuation).WithOptional(t => (xcuda_Weight)t.xcuda_Weight);
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
