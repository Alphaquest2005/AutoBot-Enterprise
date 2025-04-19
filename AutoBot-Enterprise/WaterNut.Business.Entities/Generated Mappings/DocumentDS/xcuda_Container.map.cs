namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ContainerMap : EntityTypeConfiguration<xcuda_Container>
    {
        public xcuda_ContainerMap()
        {                        
              this.HasKey(t => t.Container_Id);        
              this.ToTable("xcuda_Container");
              this.Property(t => t.Item_Number).HasColumnName("Item_Number").HasMaxLength(255);
              this.Property(t => t.Container_identity).HasColumnName("Container_identity").HasMaxLength(255);
              this.Property(t => t.Container_type).HasColumnName("Container_type").HasMaxLength(255);
              this.Property(t => t.Empty_full_indicator).HasColumnName("Empty_full_indicator").HasMaxLength(255);
              this.Property(t => t.Gross_weight).HasColumnName("Gross_weight");
              this.Property(t => t.Goods_description).HasColumnName("Goods_description").HasMaxLength(255);
              this.Property(t => t.Packages_type).HasColumnName("Packages_type").HasMaxLength(255);
              this.Property(t => t.Packages_number).HasColumnName("Packages_number").HasMaxLength(255);
              this.Property(t => t.Packages_weight).HasColumnName("Packages_weight");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Container_Id).HasColumnName("Container_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Container>) t.xcuda_Container).HasForeignKey(d => d.ASYCUDA_Id);
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
