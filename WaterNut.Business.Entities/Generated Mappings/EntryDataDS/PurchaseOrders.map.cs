namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PurchaseOrdersMap : EntityTypeConfiguration<PurchaseOrders>
    {
        public PurchaseOrdersMap()
        {                        
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryData_PurchaseOrders");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.PONumber).HasColumnName("PONumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.SupplierInvoiceNo).HasColumnName("SupplierInvoiceNo").HasMaxLength(50);
              this.HasMany(t => t.WarehouseInfo).WithRequired(t => (PurchaseOrders)t.EntryData_PurchaseOrders);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
