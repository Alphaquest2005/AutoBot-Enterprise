namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TotalMap : EntityTypeConfiguration<xcuda_Total>
    {
        public xcuda_TotalMap()
        {                        
              this.HasKey(t => t.Valuation_Id);        
              this.ToTable("xcuda_Total");
              this.Property(t => t.Total_invoice).HasColumnName("Total_invoice");
              this.Property(t => t.Total_weight).HasColumnName("Total_weight");
              this.Property(t => t.Valuation_Id).HasColumnName("Valuation_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Valuation).WithOptional(t => (xcuda_Total)t.xcuda_Total);
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
