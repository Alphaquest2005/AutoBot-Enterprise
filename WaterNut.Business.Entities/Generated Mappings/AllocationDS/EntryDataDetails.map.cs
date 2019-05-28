namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryDataDetailsMap : EntityTypeConfiguration<EntryDataDetails>
    {
        public EntryDataDetailsMap()
        {                        
              this.HasKey(t => t.EntryDataDetailsId);        
              this.ToTable("EntryDataDetails");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(15);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.UnitWeight).HasColumnName("UnitWeight");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.Weight).HasColumnName("Weight");
              this.Property(t => t.InternalFreight).HasColumnName("InternalFreight");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(50);
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(50);
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(255);
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.IsReconciled).HasColumnName("IsReconciled");
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.LastCost).HasColumnName("LastCost");
              this.HasRequired(t => t.Sales).WithMany(t =>(ICollection<EntryDataDetails>) t.EntryDataDetails).HasForeignKey(d => d.EntryDataId);
              this.HasRequired(t => t.Adjustments).WithMany(t =>(ICollection<EntryDataDetails>) t.EntryDataDetails).HasForeignKey(d => d.EntryDataId);
              this.HasMany(t => t.AsycudaSalesAllocations).WithOptional(t => t.EntryDataDetails).HasForeignKey(d => d.EntryDataDetailsId);
              this.HasOptional(t => t.EntryDataDetailsEx).WithRequired(t => (EntryDataDetails) t.EntryDataDetails);
              this.HasMany(t => t.AdjustmentShortAllocations).WithRequired(t => (EntryDataDetails)t.EntryDataDetails);
              this.HasMany(t => t.AsycudaDocumentItemEntryDataDetails).WithRequired(t => (EntryDataDetails)t.EntryDataDetails);
              this.HasMany(t => t.EX9AsycudaSalesAllocations).WithOptional(t => t.EntryDataDetails).HasForeignKey(d => d.EntryDataDetailsId);
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
