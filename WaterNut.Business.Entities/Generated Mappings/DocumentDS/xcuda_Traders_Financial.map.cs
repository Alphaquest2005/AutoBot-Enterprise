namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Traders_FinancialMap : EntityTypeConfiguration<xcuda_Traders_Financial>
    {
        public xcuda_Traders_FinancialMap()
        {                        
              this.HasKey(t => t.Traders_Id);        
              this.ToTable("xcuda_Traders_Financial");
              this.Property(t => t.Traders_Id).HasColumnName("Traders_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Financial_code).HasColumnName("Financial_code");
              this.Property(t => t.Financial_name).HasColumnName("Financial_name");
              this.HasRequired(t => t.xcuda_Traders).WithOptional(t => (xcuda_Traders_Financial)t.xcuda_Traders_Financial);
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
