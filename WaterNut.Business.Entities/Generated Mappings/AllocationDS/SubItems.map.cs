namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SubItemsMap : EntityTypeConfiguration<SubItems>
    {
        public SubItemsMap()
        {                        
              this.HasKey(t => t.SubItem_Id);        
              this.ToTable("SubItems");
              this.Property(t => t.SubItem_Id).HasColumnName("SubItem_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(100);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(255);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.HasRequired(t => t.xcuda_Item).WithMany(t =>(ICollection<SubItems>) t.SubItems).HasForeignKey(d => d.Item_Id);
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
