namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentItemMap : EntityTypeConfiguration<AsycudaDocumentItem>
    {
        public AsycudaDocumentItemMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("AsycudaDocumentItem");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.AsycudaDocumentId).HasColumnName("AsycudaDocumentId");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.IsAssessed).HasColumnName("IsAssessed");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.DoNotEX).HasColumnName("DoNotEX");
              this.Property(t => t.AttributeOnlyAllocation).HasColumnName("AttributeOnlyAllocation");
              this.Property(t => t.Description_of_goods).HasColumnName("Description_of_goods").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Commercial_Description).HasColumnName("Commercial_Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Gross_weight_itm).HasColumnName("Gross_weight_itm");
              this.Property(t => t.Net_weight_itm).HasColumnName("Net_weight_itm");
              this.Property(t => t.Item_price).HasColumnName("Item_price");
              this.Property(t => t.ItemQuantity).HasColumnName("ItemQuantity");
              this.Property(t => t.Suppplementary_unit_code).HasColumnName("Suppplementary_unit_code").HasMaxLength(4);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(20);
              this.Property(t => t.TariffCodeLicenseRequired).HasColumnName("TariffCodeLicenseRequired");
              this.Property(t => t.TariffCategoryLicenseRequired).HasColumnName("TariffCategoryLicenseRequired");
              this.Property(t => t.TariffCodeDescription).HasColumnName("TariffCodeDescription").HasMaxLength(999);
              this.Property(t => t.DutyLiability).HasColumnName("DutyLiability");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.Statistical_value).HasColumnName("Statistical_value");
              this.Property(t => t.DPQtyAllocated).HasColumnName("DPQtyAllocated");
              this.Property(t => t.DFQtyAllocated).HasColumnName("DFQtyAllocated");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.ImportComplete).HasColumnName("ImportComplete");
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.Number_of_packages).HasColumnName("Number_of_packages");
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.PiWeight).HasColumnName("PiWeight");
              this.Property(t => t.Currency_rate).HasColumnName("Currency_rate");
              this.Property(t => t.Currency_code).HasColumnName("Currency_code").HasMaxLength(20);
              this.Property(t => t.InvalidHSCode).HasColumnName("InvalidHSCode");
              this.Property(t => t.WarehouseError).HasColumnName("WarehouseError").HasMaxLength(50);
              this.Property(t => t.Cancelled).HasColumnName("Cancelled");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.ReferenceNumber).HasColumnName("ReferenceNumber").HasMaxLength(30);
              this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceLineNumber).HasColumnName("PreviousInvoiceLineNumber").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceItemNumber).HasColumnName("PreviousInvoiceItemNumber").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasOptional(t => t.AsycudaDocument).WithMany(t =>(ICollection<AsycudaDocumentItem>) t.AsycudaDocumentItems).HasForeignKey(d => d.AsycudaDocumentId);
              this.HasOptional(t => t.InventoryItems).WithMany(t =>(ICollection<AsycudaDocumentItem>) t.AsycudaDocumentItem).HasForeignKey(d => d.ItemNumber);
              this.HasMany(t => t.SubItems).WithRequired(t => (AsycudaDocumentItem)t.AsycudaDocumentItem);
              this.HasMany(t => t.PreviousItems).WithRequired(t => (AsycudaDocumentItem)t.AsycudaDocumentItem);
              this.HasMany(t => t.xcuda_Supplementary_unit).WithRequired(t => (AsycudaDocumentItem)t.AsycudaDocumentItem);
              this.HasMany(t => t.AsycudaDocumentItemEntryDataDetails).WithRequired(t => (AsycudaDocumentItem)t.AsycudaDocumentItem);
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
