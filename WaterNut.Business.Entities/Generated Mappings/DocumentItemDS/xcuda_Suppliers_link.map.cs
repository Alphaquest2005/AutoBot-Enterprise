namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Suppliers_linkMap : EntityTypeConfiguration<xcuda_Suppliers_link>
    {
        public xcuda_Suppliers_linkMap()
        {                        
              this.HasKey(t => t.Suppliers_link_Id);        
              this.ToTable("xcuda_Suppliers_link");
              this.Property(t => t.Suppliers_link_code).HasColumnName("Suppliers_link_code").HasMaxLength(100);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.Suppliers_link_Id).HasColumnName("Suppliers_link_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasOptional(t => t.xcuda_Item).WithMany(t =>(ICollection<xcuda_Suppliers_link>) t.xcuda_Suppliers_link).HasForeignKey(d => d.Item_Id);
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
