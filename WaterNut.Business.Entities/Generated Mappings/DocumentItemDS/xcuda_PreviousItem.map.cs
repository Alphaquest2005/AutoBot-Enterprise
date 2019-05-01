namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_PreviousItemMap : EntityTypeConfiguration<xcuda_PreviousItem>
    {
        public xcuda_PreviousItemMap()
        {                        
              this.HasKey(t => t.PreviousItem_Id);        
              this.ToTable("xcuda_PreviousItem");
              this.Property(t => t.Packages_number).HasColumnName("Packages_number").HasMaxLength(20);
              this.Property(t => t.Previous_Packages_number).HasColumnName("Previous_Packages_number").HasMaxLength(20);
              this.Property(t => t.Hs_code).HasColumnName("Hs_code").HasMaxLength(20);
              this.Property(t => t.Commodity_code).HasColumnName("Commodity_code").HasMaxLength(20);
              this.Property(t => t.Previous_item_number).HasColumnName("Previous_item_number").HasMaxLength(20);
              this.Property(t => t.Goods_origin).HasColumnName("Goods_origin").HasMaxLength(20);
              this.Property(t => t.Net_weight).HasColumnName("Net_weight");
              this.Property(t => t.Prev_net_weight).HasColumnName("Prev_net_weight");
              this.Property(t => t.Prev_reg_ser).HasColumnName("Prev_reg_ser").HasMaxLength(20);
              this.Property(t => t.Prev_reg_nbr).HasColumnName("Prev_reg_nbr").HasMaxLength(20);
              this.Property(t => t.Prev_reg_dat).HasColumnName("Prev_reg_dat").HasMaxLength(20);
              this.Property(t => t.Prev_reg_cuo).HasColumnName("Prev_reg_cuo").HasMaxLength(20);
              this.Property(t => t.Suplementary_Quantity).HasColumnName("Suplementary_Quantity");
              this.Property(t => t.Preveious_suplementary_quantity).HasColumnName("Preveious_suplementary_quantity");
              this.Property(t => t.Current_value).HasColumnName("Current_value");
              this.Property(t => t.Previous_value).HasColumnName("Previous_value");
              this.Property(t => t.Current_item_number).HasColumnName("Current_item_number").HasMaxLength(20);
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.Prev_decl_HS_spec).HasColumnName("Prev_decl_HS_spec").HasMaxLength(20);
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => (xcuda_PreviousItem)t.xcuda_PreviousItem);
              this.HasOptional(t => t.Ex).WithRequired(t => (xcuda_PreviousItem) t.xcuda_PreviousItem);
              this.HasMany(t => t.xcuda_Items).WithRequired(t => (xcuda_PreviousItem)t.xcuda_PreviousItem);
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
