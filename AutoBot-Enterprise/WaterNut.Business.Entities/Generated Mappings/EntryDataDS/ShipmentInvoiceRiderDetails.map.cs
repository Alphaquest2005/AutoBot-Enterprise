namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoiceRiderDetailsMap : EntityTypeConfiguration<ShipmentInvoiceRiderDetails>
    {
        public ShipmentInvoiceRiderDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceRiderDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.RiderDetailId).HasColumnName("RiderDetailId");
              this.HasRequired(t => t.ShipmentInvoice).WithMany(t =>(ICollection<ShipmentInvoiceRiderDetails>) t.ShipmentInvoiceRiderDetails).HasForeignKey(d => d.InvoiceId);
              this.HasRequired(t => t.ShipmentRiderDetails).WithMany(t =>(ICollection<ShipmentInvoiceRiderDetails>) t.ShipmentInvoiceRiderDetails).HasForeignKey(d => d.RiderDetailId);
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
