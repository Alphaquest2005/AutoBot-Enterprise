namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaSalesAllocationsMap : EntityTypeConfiguration<AsycudaSalesAllocations>
    {
        public AsycudaSalesAllocationsMap()
        {                        
              this.HasKey(t => t.AllocationId);        
              this.ToTable("AsycudaSalesAllocations");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.EANumber).HasColumnName("EANumber");
              this.Property(t => t.SANumber).HasColumnName("SANumber");
              this.Property(t => t.xEntryItem_Id).HasColumnName("xEntryItem_Id");
              this.HasOptional(t => t.EntryDataDetails).WithMany(t =>(ICollection<AsycudaSalesAllocations>) t.AsycudaSalesAllocations).HasForeignKey(d => d.EntryDataDetailsId);
              this.HasOptional(t => t.PreviousDocumentItem).WithMany(t =>(ICollection<AsycudaSalesAllocations>) t.AsycudaSalesAllocations).HasForeignKey(d => d.PreviousItem_Id);
              this.HasMany(t => t.xBondAllocations).WithRequired(t => (AsycudaSalesAllocations)t.AsycudaSalesAllocations);
              this.HasOptional(t => t.EX9AsycudaSalesAllocations).WithRequired(t => (AsycudaSalesAllocations) t.AsycudaSalesAllocations);
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
