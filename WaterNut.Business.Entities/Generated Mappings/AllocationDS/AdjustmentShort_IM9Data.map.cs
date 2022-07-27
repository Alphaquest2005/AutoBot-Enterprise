namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AdjustmentShort_IM9DataMap : EntityTypeConfiguration<AdjustmentShort_IM9Data>
    {
        public AdjustmentShort_IM9DataMap()
        {                        
              this.HasKey(t => t.AllocationId);        
              this.ToTable("AdjustmentShort-IM9Data");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.pItemDescription).HasColumnName("pItemDescription").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().IsUnicode(false).HasMaxLength(9);
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").HasMaxLength(20);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.pItemNumber).HasColumnName("pItemNumber").HasMaxLength(50);
              this.Property(t => t.pItemCost).HasColumnName("pItemCost");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.SalesQtyAllocated).HasColumnName("SalesQtyAllocated");
              this.Property(t => t.SalesQuantity).HasColumnName("SalesQuantity");
              this.Property(t => t.pPrecision1).HasColumnName("pPrecision1").HasMaxLength(255);
              this.Property(t => t.DFQtyAllocated).HasColumnName("DFQtyAllocated");
              this.Property(t => t.DPQtyAllocated).HasColumnName("DPQtyAllocated");
              this.Property(t => t.pTariffCode).HasColumnName("pTariffCode").IsRequired().HasMaxLength(20);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(255);
              this.Property(t => t.Customs_clearance_office_code).HasColumnName("Customs_clearance_office_code").HasMaxLength(10);
              this.Property(t => t.pQuantity).HasColumnName("pQuantity");
              this.Property(t => t.pRegistrationDate).HasColumnName("pRegistrationDate");
              this.Property(t => t.pAssessmentDate).HasColumnName("pAssessmentDate");
              this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.Property(t => t.xBond_Item_Id).HasColumnName("xBond_Item_Id");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Net_weight_itm).HasColumnName("Net_weight_itm");
              this.HasOptional(t => t.PreviousDocumentItem).WithMany(t =>(ICollection<AdjustmentShort_IM9Data>) t.AdjustmentShort_IM9Data).HasForeignKey(d => d.PreviousItem_Id);
              this.HasOptional(t => t.EntryDataDetails).WithMany(t =>(ICollection<AdjustmentShort_IM9Data>) t.AdjustmentShort_IM9Data).HasForeignKey(d => d.EntryDataDetailsId);
              this.HasMany(t => t.AsycudaSalesAllocationsPIData).WithRequired(t => (AdjustmentShort_IM9Data)t.AdjustmentShort_IM9Data);
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
