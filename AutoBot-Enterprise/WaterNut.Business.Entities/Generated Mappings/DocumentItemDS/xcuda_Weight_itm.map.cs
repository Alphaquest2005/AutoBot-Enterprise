namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Weight_itmMap : EntityTypeConfiguration<xcuda_Weight_itm>
    {
        public xcuda_Weight_itmMap()
        {                        
              this.HasKey(t => t.Valuation_item_Id);        
              this.ToTable("xcuda_Weight_itm");
              this.Property(t => t.Gross_weight_itm).HasColumnName("Gross_weight_itm");
              this.Property(t => t.Net_weight_itm).HasColumnName("Net_weight_itm");
              this.Property(t => t.Valuation_item_Id).HasColumnName("Valuation_item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Valuation_item).WithOptional(t => (xcuda_Weight_itm)t.xcuda_Weight_itm);
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
