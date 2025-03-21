﻿namespace AllocationQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaSalesAndAdjustmentAllocationsExMap : EntityTypeConfiguration<AsycudaSalesAndAdjustmentAllocationsEx>
    {
        public AsycudaSalesAndAdjustmentAllocationsExMap()
        {                        
              this.HasKey(t => new {t.TotalValue, t.AllocatedValue, t.QtyAllocated, t.SANumber, t.InvoiceDate, t.SalesQuantity, t.SalesQtyAllocated, t.InvoiceNo, t.ItemNumber, t.ItemDescription, t.Cost, t.TaxAmount, t.AllocationId, t.EntryDataDetailsId, t.xBond_Item_Id, t.ApplicationSettingsId});        
              this.ToTable("AsycudaSalesAndAdjustmentAllocationsEx");
              this.Property(t => t.TotalValue).HasColumnName("TotalValue");
              this.Property(t => t.AllocatedValue).HasColumnName("AllocatedValue");
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.SANumber).HasColumnName("SANumber").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.CustomerName).HasColumnName("CustomerName").HasMaxLength(255);
              this.Property(t => t.SalesQuantity).HasColumnName("SalesQuantity");
              this.Property(t => t.SalesQtyAllocated).HasColumnName("SalesQtyAllocated");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.SalesLineNumber).HasColumnName("SalesLineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").HasMaxLength(50);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.DutyLiability).HasColumnName("DutyLiability");
              this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.pReferenceNumber).HasColumnName("pReferenceNumber").HasMaxLength(30);
              this.Property(t => t.pRegistrationDate).HasColumnName("pRegistrationDate");
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").HasMaxLength(20);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.pQuantity).HasColumnName("pQuantity");
              this.Property(t => t.pQtyAllocated).HasColumnName("pQtyAllocated");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.pExpiryDate).HasColumnName("pExpiryDate");
              this.Property(t => t.pTariffCode).HasColumnName("pTariffCode").HasMaxLength(20);
              this.Property(t => t.pIsAssessed).HasColumnName("pIsAssessed");
              this.Property(t => t.DoNotAllocateSales).HasColumnName("DoNotAllocateSales");
              this.Property(t => t.DoNotAllocatePreviousEntry).HasColumnName("DoNotAllocatePreviousEntry");
              this.Property(t => t.WarehouseError).HasColumnName("WarehouseError").HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.Invalid).HasColumnName("Invalid");
              this.Property(t => t.pItemNumber).HasColumnName("pItemNumber").HasMaxLength(50);
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.AssessmentDate).HasColumnName("AssessmentDate");
              this.Property(t => t.xStatus).HasColumnName("xStatus").HasMaxLength(255);
              this.Property(t => t.xReferenceNumber).HasColumnName("xReferenceNumber").HasMaxLength(30);
              this.Property(t => t.xCNumber).HasColumnName("xCNumber").HasMaxLength(20);
              this.Property(t => t.xLineNumber).HasColumnName("xLineNumber");
              this.Property(t => t.xRegistrationDate).HasColumnName("xRegistrationDate");
              this.Property(t => t.xQuantity).HasColumnName("xQuantity");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.PreviousItem_Id).HasColumnName("PreviousItem_Id");
              this.Property(t => t.xBond_Item_Id).HasColumnName("xBond_Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.xASYCUDA_Id).HasColumnName("xASYCUDA_Id");
              this.Property(t => t.pASYCUDA_Id).HasColumnName("pASYCUDA_Id");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
