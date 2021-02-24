namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoicePOsMap : EntityTypeConfiguration<ShipmentInvoicePOs>
    {
        public ShipmentInvoicePOsMap()
        {                        
              this.HasKey(t => new {t.InvoiceId, t.EntryData_Id});        
              this.ToTable("ShipmentInvoicePOs");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.PurchaseOrders).WithMany(t =>(ICollection<ShipmentInvoicePOs>) t.ShipmentInvoicePOs).HasForeignKey(d => d.EntryData_Id);
              this.HasRequired(t => t.ShipmentInvoice).WithMany(t =>(ICollection<ShipmentInvoicePOs>) t.ShipmentInvoicePOs).HasForeignKey(d => d.InvoiceId);
              this.HasMany(t => t.POMISMatches).WithOptional(t => t.ShipmentInvoicePOs).HasForeignKey(d => new {d.INVId, d.POId});
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
