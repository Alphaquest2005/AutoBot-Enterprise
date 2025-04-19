namespace AdjustmentQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AdjustmentOversAllocationMap : EntityTypeConfiguration<AdjustmentOversAllocation>
    {
        public AdjustmentOversAllocationMap()
        {                        
              this.HasKey(t => t.AllocationId);        
              this.ToTable("AdjustmentOversAllocations");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.Asycuda_Id).HasColumnName("Asycuda_Id");
              this.HasRequired(t => t.EntryDataDetail).WithMany(t =>(ICollection<AdjustmentOversAllocation>) t.AdjustmentOversAllocations).HasForeignKey(d => d.EntryDataDetailsId);
              this.HasOptional(t => t.xcuda_Item).WithMany(t =>(ICollection<AdjustmentOversAllocation>) t.AdjustmentOversAllocations).HasForeignKey(d => d.PreviousItem_Id);
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
