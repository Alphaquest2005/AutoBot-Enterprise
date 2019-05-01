namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xBondAllocationsMap : EntityTypeConfiguration<xBondAllocations>
    {
        public xBondAllocationsMap()
        {                        
              this.HasKey(t => t.xBondAllocationId);        
              this.ToTable("xBondAllocations");
              this.Property(t => t.xEntryItem_Id).HasColumnName("xEntryItem_Id");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId");
              this.Property(t => t.xBondAllocationId).HasColumnName("xBondAllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasRequired(t => t.AsycudaSalesAllocations).WithMany(t =>(ICollection<xBondAllocations>) t.xBondAllocations).HasForeignKey(d => d.AllocationId);
              this.HasRequired(t => t.xcuda_Item).WithMany(t =>(ICollection<xBondAllocations>) t.xBondAllocations).HasForeignKey(d => d.xEntryItem_Id);
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
