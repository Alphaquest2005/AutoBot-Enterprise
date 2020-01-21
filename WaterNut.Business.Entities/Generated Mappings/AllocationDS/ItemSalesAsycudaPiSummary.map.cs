namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ItemSalesAsycudaPiSummaryMap : EntityTypeConfiguration<ItemSalesAsycudaPiSummary>
    {
        public ItemSalesAsycudaPiSummaryMap()
        {                        
              this.HasKey(t => new {t.PreviousItem_Id, t.EntryDataDate});        
              this.ToTable("ItemSalesAsycudaPiSummary");
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").HasMaxLength(20);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Type).HasColumnName("Type").IsUnicode(false).HasMaxLength(5);
              this.Property(t => t.pQtyAllocated).HasColumnName("pQtyAllocated");
              this.Property(t => t.MonthYear).HasColumnName("MonthYear").IsUnicode(false).HasMaxLength(7);
              this.Property(t => t.EntryDataDate).HasColumnName("EntryDataDate");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsUnicode(false).HasMaxLength(9);
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
