namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TaxationMap : EntityTypeConfiguration<xcuda_Taxation>
    {
        public xcuda_TaxationMap()
        {                        
              this.HasKey(t => t.Taxation_Id);        
              this.ToTable("xcuda_Taxation");
              this.Property(t => t.Item_taxes_amount).HasColumnName("Item_taxes_amount");
              this.Property(t => t.Item_taxes_guaranted_amount).HasColumnName("Item_taxes_guaranted_amount");
              this.Property(t => t.Counter_of_normal_mode_of_payment).HasColumnName("Counter_of_normal_mode_of_payment").HasMaxLength(20);
              this.Property(t => t.Displayed_item_taxes_amount).HasColumnName("Displayed_item_taxes_amount").HasMaxLength(20);
              this.Property(t => t.Taxation_Id).HasColumnName("Taxation_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.Item_taxes_mode_of_payment).HasColumnName("Item_taxes_mode_of_payment").HasMaxLength(20);
              this.HasOptional(t => t.xcuda_Item).WithMany(t =>(ICollection<xcuda_Taxation>) t.xcuda_Taxation).HasForeignKey(d => d.Item_Id);
              this.HasMany(t => t.xcuda_Taxation_line).WithOptional(t => t.xcuda_Taxation).HasForeignKey(d => d.Taxation_Id);
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
