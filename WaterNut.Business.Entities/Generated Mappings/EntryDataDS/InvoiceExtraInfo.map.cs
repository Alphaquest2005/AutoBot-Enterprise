namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InvoiceExtraInfoMap : EntityTypeConfiguration<InvoiceExtraInfo>
    {
        public InvoiceExtraInfoMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceExtraInfo");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Info).HasColumnName("Info").IsRequired().HasMaxLength(50);
              this.Property(t => t.Value).HasColumnName("Value").IsRequired().HasMaxLength(255);
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.HasRequired(t => t.Invoice).WithMany(t =>(ICollection<InvoiceExtraInfo>) t.InvoiceExtraInfo).HasForeignKey(d => d.InvoiceId);
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
