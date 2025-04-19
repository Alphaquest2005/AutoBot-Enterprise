namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ItemEntryDataDetailsMap : EntityTypeConfiguration<xcuda_ItemEntryDataDetails>
    {
        public xcuda_ItemEntryDataDetailsMap()
        {                        
              this.HasKey(t => t.ItemEntryDataDetailId);        
              this.ToTable("xcuda_ItemEntryDataDetails");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.ItemEntryDataDetailId).HasColumnName("ItemEntryDataDetailId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasRequired(t => t.xcuda_Item).WithMany(t =>(ICollection<xcuda_ItemEntryDataDetails>) t.EntryDataDetails).HasForeignKey(d => d.Item_Id);
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
