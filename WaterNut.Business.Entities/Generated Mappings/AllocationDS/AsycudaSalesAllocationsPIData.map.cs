namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaSalesAllocationsPIDataMap : EntityTypeConfiguration<AsycudaSalesAllocationsPIData>
    {
        public AsycudaSalesAllocationsPIDataMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaSalesAllocationsPIData");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId");
              this.Property(t => t.xLineNumber).HasColumnName("xLineNumber");
              this.Property(t => t.xBond_Item_Id).HasColumnName("xBond_Item_Id");
              this.Property(t => t.xCNumber).HasColumnName("xCNumber").HasMaxLength(20);
              this.Property(t => t.xRegistrationDate).HasColumnName("xRegistrationDate");
              this.Property(t => t.xQuantity).HasColumnName("xQuantity");
              this.Property(t => t.xASYCUDA_Id).HasColumnName("xASYCUDA_Id");
              this.Property(t => t.xReferenceNumber).HasColumnName("xReferenceNumber").HasMaxLength(30);
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.EX9AsycudaSalesAllocations).WithMany(t =>(ICollection<AsycudaSalesAllocationsPIData>) t.AsycudaSalesAllocationsPIData).HasForeignKey(d => d.AllocationId);
              this.HasRequired(t => t.AdjustmentShortAllocations).WithMany(t =>(ICollection<AsycudaSalesAllocationsPIData>) t.AsycudaSalesAllocationsPIData).HasForeignKey(d => d.AllocationId);
              this.HasRequired(t => t.AsycudaSalesAllocations).WithMany(t =>(ICollection<AsycudaSalesAllocationsPIData>) t.PIData).HasForeignKey(d => d.AllocationId);
              this.HasRequired(t => t.AdjustmentShort_IM9Data).WithMany(t =>(ICollection<AsycudaSalesAllocationsPIData>) t.AsycudaSalesAllocationsPIData).HasForeignKey(d => d.AllocationId);
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
