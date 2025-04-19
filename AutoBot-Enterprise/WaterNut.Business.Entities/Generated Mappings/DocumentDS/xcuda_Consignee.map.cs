namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ConsigneeMap : EntityTypeConfiguration<xcuda_Consignee>
    {
        public xcuda_ConsigneeMap()
        {                        
              this.HasKey(t => t.Traders_Id);        
              this.ToTable("xcuda_Consignee");
              this.Property(t => t.Traders_Id).HasColumnName("Traders_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Consignee_code).HasColumnName("Consignee_code").HasMaxLength(20);
              this.Property(t => t.Consignee_name).HasColumnName("Consignee_name").HasMaxLength(255);
              this.HasRequired(t => t.xcuda_Traders).WithOptional(t => (xcuda_Consignee)t.xcuda_Consignee);
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
