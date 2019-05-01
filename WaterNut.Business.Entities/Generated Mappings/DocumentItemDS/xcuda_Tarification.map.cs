namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TarificationMap : EntityTypeConfiguration<xcuda_Tarification>
    {
        public xcuda_TarificationMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Tarification");
              this.Property(t => t.Extended_customs_procedure).HasColumnName("Extended_customs_procedure").HasMaxLength(20);
              this.Property(t => t.National_customs_procedure).HasColumnName("National_customs_procedure").HasMaxLength(20);
              this.Property(t => t.Item_price).HasColumnName("Item_price");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Value_item).HasColumnName("Value_item").HasMaxLength(20);
              this.Property(t => t.Attached_doc_item).HasColumnName("Attached_doc_item").HasMaxLength(20);
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => (xcuda_Tarification)t.xcuda_Tarification);
              this.HasOptional(t => t.xcuda_HScode).WithRequired(t => (xcuda_Tarification)t.xcuda_Tarification);
              this.HasMany(t => t.Unordered_xcuda_Supplementary_unit).WithRequired(t => (xcuda_Tarification)t.xcuda_Tarification);
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
