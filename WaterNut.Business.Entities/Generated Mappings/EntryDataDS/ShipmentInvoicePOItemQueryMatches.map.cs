namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoicePOItemQueryMatchesMap : EntityTypeConfiguration<ShipmentInvoicePOItemQueryMatches>
    {
        public ShipmentInvoicePOItemQueryMatchesMap()
        {                        
              this.HasKey(t => new {t.PODetailsId, t.INVDetailsId, t.CategoryTariffCode});        
              this.ToTable("ShipmentInvoicePOItemQueryMatches");
              this.Property(t => t.PODetailsId).HasColumnName("PODetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.INVDetailsId).HasColumnName("INVDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.POId).HasColumnName("POId");
              this.Property(t => t.INVId).HasColumnName("INVId");
              this.Property(t => t.PONumber).HasColumnName("PONumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.POItemCode).HasColumnName("POItemCode").IsRequired().HasMaxLength(20);
              this.Property(t => t.INVItemCode).HasColumnName("INVItemCode").HasMaxLength(20);
              this.Property(t => t.PODescription).HasColumnName("PODescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.INVDescription).HasColumnName("INVDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.POCost).HasColumnName("POCost");
              this.Property(t => t.INVCost).HasColumnName("INVCost");
              this.Property(t => t.POQuantity).HasColumnName("POQuantity");
              this.Property(t => t.INVQuantity).HasColumnName("INVQuantity");
              this.Property(t => t.POTotalCost).HasColumnName("POTotalCost");
              this.Property(t => t.INVTotalCost).HasColumnName("INVTotalCost");
              this.Property(t => t.INVInventoryItemId).HasColumnName("INVInventoryItemId");
              this.Property(t => t.POInventoryItemId).HasColumnName("POInventoryItemId");
              this.Property(t => t.Gallons).HasColumnName("Gallons");
              this.Property(t => t.RankNo).HasColumnName("RankNo");
              this.Property(t => t.rn).HasColumnName("rn");
              this.Property(t => t.CategoryTariffCode).HasColumnName("CategoryTariffCode").IsRequired().HasMaxLength(12);
              this.Property(t => t.Category).HasColumnName("category").HasMaxLength(50);
              this.HasRequired(t => t.InvoiceDetails).WithMany(t =>(ICollection<ShipmentInvoicePOItemQueryMatches>) t.POItems).HasForeignKey(d => d.INVDetailsId);
              this.HasRequired(t => t.EntryDataDetails).WithMany(t =>(ICollection<ShipmentInvoicePOItemQueryMatches>) t.INVItems).HasForeignKey(d => d.PODetailsId);
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
