namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class XSales_UnAllocatedMap : EntityTypeConfiguration<XSales_UnAllocated>
    {
        public XSales_UnAllocatedMap()
        {                        
              this.HasKey(t => new {t.pItemId, t.xItemId, t.EntryDataDetailsId, t.InventoryItemId, t.PreviousDocumentInventoryItemId});        
              this.ToTable("XSales-UnAllocated");
              this.Property(t => t.Line).HasColumnName("Line");
              this.Property(t => t.Date).HasColumnName("Date");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.CustomerName).HasColumnName("CustomerName").HasMaxLength(50);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.SalesQuantity).HasColumnName("SalesQuantity");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.xQuantity).HasColumnName("xQuantity");
              this.Property(t => t.Price).HasColumnName("Price");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().HasMaxLength(50);
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.pRegDate).HasColumnName("pRegDate");
              this.Property(t => t.CIFValue).HasColumnName("CIFValue");
              this.Property(t => t.DutyLiablity).HasColumnName("DutyLiablity");
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.SalesLineNumber).HasColumnName("SalesLineNumber");
              this.Property(t => t.xItemId).HasColumnName("xItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.pItemId).HasColumnName("pItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.PreviousDocumentInventoryItemId).HasColumnName("PreviousDocumentInventoryItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
