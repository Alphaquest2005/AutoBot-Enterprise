namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryPreviousItemsMap : EntityTypeConfiguration<EntryPreviousItems>
    {
        public EntryPreviousItemsMap()
        {                        
              this.HasKey(t => t.EntryPreviousItemId);        
              this.ToTable("EntryPreviousItems");
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.EntryPreviousItemId).HasColumnName("EntryPreviousItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasRequired(t => t.xcuda_Item).WithMany(t =>(ICollection<EntryPreviousItems>) t.EntryPreviousItems).HasForeignKey(d => d.Item_Id);
              this.HasRequired(t => t.xcuda_PreviousItem).WithMany(t =>(ICollection<EntryPreviousItems>) t.EntryPreviousItems).HasForeignKey(d => d.PreviousItem_Id);
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
