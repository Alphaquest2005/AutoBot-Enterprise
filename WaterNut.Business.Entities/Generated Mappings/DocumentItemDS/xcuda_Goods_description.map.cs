namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Goods_descriptionMap : EntityTypeConfiguration<xcuda_Goods_description>
    {
        public xcuda_Goods_descriptionMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Goods_description");
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.Description_of_goods).HasColumnName("Description_of_goods").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Commercial_Description).HasColumnName("Commercial_Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => (xcuda_Goods_description)t.xcuda_Goods_description);
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
