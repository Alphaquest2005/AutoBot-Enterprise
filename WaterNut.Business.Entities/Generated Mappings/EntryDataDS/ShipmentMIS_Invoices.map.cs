namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentMIS_InvoicesMap : EntityTypeConfiguration<ShipmentMIS_Invoices>
    {
        public ShipmentMIS_InvoicesMap()
        {                        
              this.HasKey(t => new {t.Id, t.SourceFile});        
              this.ToTable("ShipmentMIS-Invoices");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.SubTotal).HasColumnName("SubTotal");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.ImportedLines).HasColumnName("ImportedLines");
              this.Property(t => t.RiderID).HasColumnName("RiderID");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").HasMaxLength(100);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
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
