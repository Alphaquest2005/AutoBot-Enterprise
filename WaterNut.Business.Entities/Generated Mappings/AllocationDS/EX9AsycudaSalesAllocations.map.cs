namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EX9AsycudaSalesAllocationsMap : EntityTypeConfiguration<EX9AsycudaSalesAllocations>
    {
        public EX9AsycudaSalesAllocationsMap()
        {                        
              this.HasKey(t => t.AllocationId);        
              this.ToTable("EX9AsycudaSalesAllocations");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.TotalValue).HasColumnName("TotalValue");
              this.Property(t => t.AllocatedValue).HasColumnName("AllocatedValue");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.SalesQuantity).HasColumnName("SalesQuantity");
              this.Property(t => t.SalesQtyAllocated).HasColumnName("SalesQtyAllocated");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").HasMaxLength(50);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(255);
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().IsUnicode(false).HasMaxLength(9);
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").HasMaxLength(20);
              this.Property(t => t.pRegistrationDate).HasColumnName("pRegistrationDate");
              this.Property(t => t.pQuantity).HasColumnName("pQuantity");
              this.Property(t => t.pQtyAllocated).HasColumnName("pQtyAllocated");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.pReferenceNumber).HasColumnName("pReferenceNumber").HasMaxLength(30);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.pASYCUDA_Id).HasColumnName("pASYCUDA_Id");
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.DutyLiability).HasColumnName("DutyLiability");
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.pIsAssessed).HasColumnName("pIsAssessed");
              this.Property(t => t.DoNotAllocateSales).HasColumnName("DoNotAllocateSales");
              this.Property(t => t.DoNotAllocatePreviousEntry).HasColumnName("DoNotAllocatePreviousEntry");
              this.Property(t => t.SANumber).HasColumnName("SANumber");
              this.Property(t => t.Commercial_Description).HasColumnName("Commercial_Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.Gross_weight_itm).HasColumnName("Gross_weight_itm");
              this.Property(t => t.Net_weight_itm).HasColumnName("Net_weight_itm");
              this.Property(t => t.pTariffCode).HasColumnName("pTariffCode").HasMaxLength(20);
              this.Property(t => t.pItemNumber).HasColumnName("pItemNumber").HasMaxLength(50);
              this.Property(t => t.pItemCost).HasColumnName("pItemCost");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.Customs_clearance_office_code).HasColumnName("Customs_clearance_office_code").HasMaxLength(20);
              this.Property(t => t.Invalid).HasColumnName("Invalid");
              this.Property(t => t.pExpiryDate).HasColumnName("pExpiryDate");
              this.Property(t => t.xBond_Item_Id).HasColumnName("xBond_Item_Id");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasRequired(t => t.PreviousDocumentItem).WithMany(t =>(ICollection<EX9AsycudaSalesAllocations>) t.EX9AsycudaSalesAllocations).HasForeignKey(d => d.PreviousItem_Id);
              this.HasOptional(t => t.InventoryItemsEx).WithMany(t =>(ICollection<EX9AsycudaSalesAllocations>) t.EX9AsycudaSalesAllocations).HasForeignKey(d => new {d.ItemNumber, d.ApplicationSettingsId});
              this.HasRequired(t => t.AsycudaSalesAllocations).WithOptional(t => (EX9AsycudaSalesAllocations)t.EX9AsycudaSalesAllocations);
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
