namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TradersMap : EntityTypeConfiguration<xcuda_Traders>
    {
        public xcuda_TradersMap()
        {                        
              this.HasKey(t => t.Traders_Id);        
              this.ToTable("xcuda_Traders");
              this.Property(t => t.Traders_Id).HasColumnName("Traders_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_Traders)t.xcuda_Traders);
              this.HasOptional(t => t.xcuda_Consignee).WithRequired(t => (xcuda_Traders)t.xcuda_Traders);
              this.HasOptional(t => t.xcuda_Exporter).WithRequired(t => (xcuda_Traders)t.xcuda_Traders);
              this.HasOptional(t => t.xcuda_Traders_Financial).WithRequired(t => (xcuda_Traders)t.xcuda_Traders);
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
