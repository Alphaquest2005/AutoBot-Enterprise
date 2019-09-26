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
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("EntryData_PurchaseOrders");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.PONumber).HasColumnName("PONumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.SupplierInvoiceNo).HasColumnName("SupplierInvoiceNo").HasMaxLength(50);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
