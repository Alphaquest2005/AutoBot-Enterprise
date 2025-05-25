namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InvoiceDetailsMap : EntityTypeConfiguration<InvoiceDetails>
    {
        public InvoiceDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(20);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(15);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.TotalCost).HasColumnName("TotalCost");
              this.Property(t => t.FileLineNumber).HasColumnName("FileLineNumber");
              this.Property(t => t.ShipmentInvoiceId).HasColumnName("ShipmentInvoiceId");
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.Discount).HasColumnName("Discount");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(12);
              this.Property(t => t.Category).HasColumnName("Category").HasMaxLength(50);
              this.Property(t => t.CategoryTariffCode).HasColumnName("CategoryTariffCode").HasMaxLength(12);
              this.HasRequired(t => t.Invoice).WithMany(t =>(ICollection<InvoiceDetails>) t.InvoiceDetails).HasForeignKey(d => d.ShipmentInvoiceId);
              this.HasMany(t => t.POItems).WithRequired(t => (InvoiceDetails)t.InvoiceDetails);
              this.HasOptional(t => t.ItemAlias).WithRequired(t => (InvoiceDetails)t.InvoiceDetails);
              this.HasOptional(t => t.Volume).WithRequired(t => (InvoiceDetails)t.ShipmentInvoiceDetails);
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
