namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ManualAllocationsMap : EntityTypeConfiguration<ManualAllocations>
    {
        public ManualAllocationsMap()
        {                        
              this.HasKey(t => t.EntryDataDetailsId);        
              this.ToTable("EntryDataDetails_Allocations");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.HasRequired(t => t.EntryDataDetails).WithOptional(t => (ManualAllocations)t.ManualAllocations);
              this.HasRequired(t => t.xcuda_Item).WithMany(t =>(ICollection<ManualAllocations>) t.ManualAllocations).HasForeignKey(d => d.Item_Id);
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
