namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Valuation_itemMap : EntityTypeConfiguration<xcuda_Valuation_item>
    {
        public xcuda_Valuation_itemMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Valuation_item");
              this.Property(t => t.Total_cost_itm).HasColumnName("Total_cost_itm");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.Rate_of_adjustement).HasColumnName("Rate_of_adjustement");
              this.Property(t => t.Statistical_value).HasColumnName("Statistical_value");
              this.Property(t => t.Alpha_coeficient_of_apportionment).HasColumnName("Alpha_coeficient_of_apportionment").HasMaxLength(50);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => (xcuda_Valuation_item)t.xcuda_Valuation_item);
              this.HasOptional(t => t.xcuda_Item_Invoice).WithRequired(t => (xcuda_Valuation_item)t.xcuda_Valuation_item);
              this.HasOptional(t => t.xcuda_Weight_itm).WithRequired(t => (xcuda_Valuation_item)t.xcuda_Valuation_item);
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
