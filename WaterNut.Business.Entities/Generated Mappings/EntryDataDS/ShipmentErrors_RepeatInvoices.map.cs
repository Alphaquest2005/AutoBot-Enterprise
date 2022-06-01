namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentErrors_RepeatInvoicesMap : EntityTypeConfiguration<ShipmentErrors_RepeatInvoices>
    {
        public ShipmentErrors_RepeatInvoicesMap()
        {                        
              this.HasKey(t => new {t.RiderID, t.WarehouseCode});        
              this.ToTable("ShipmentErrors-RepeatInvoices");
              this.Property(t => t.id).HasColumnName("id");
              this.Property(t => t.RiderLineID).HasColumnName("RiderLineID");
              this.Property(t => t.RiderID).HasColumnName("RiderID").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").HasMaxLength(255);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").HasMaxLength(50);
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.Property(t => t.rowNumber).HasColumnName("rowNumber");
              this.Property(t => t.RowNumber2).HasColumnName("RowNumber2");
              this.Property(t => t.count).HasColumnName("count");
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
