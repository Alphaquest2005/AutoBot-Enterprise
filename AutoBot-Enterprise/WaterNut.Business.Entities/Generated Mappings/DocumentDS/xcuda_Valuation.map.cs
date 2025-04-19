namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ValuationMap : EntityTypeConfiguration<xcuda_Valuation>
    {
        public xcuda_ValuationMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Valuation");
              this.Property(t => t.Calculation_working_mode).HasColumnName("Calculation_working_mode").HasMaxLength(20);
              this.Property(t => t.Total_cost).HasColumnName("Total_cost");
              this.Property(t => t.Total_CIF).HasColumnName("Total_CIF");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_deduction).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_insurance).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_internal_freight).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_other_cost).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Total).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Weight).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_Invoice).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
              this.HasOptional(t => t.xcuda_Gs_external_freight).WithRequired(t => (xcuda_Valuation)t.xcuda_Valuation);
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
