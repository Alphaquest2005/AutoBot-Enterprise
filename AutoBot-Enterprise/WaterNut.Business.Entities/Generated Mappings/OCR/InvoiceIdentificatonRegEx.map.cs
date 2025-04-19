namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InvoiceIdentificatonRegExMap : EntityTypeConfiguration<InvoiceIdentificatonRegEx>
    {
        public InvoiceIdentificatonRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-InvoiceIdentificatonRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.HasRequired(t => t.OCR_Invoices).WithMany(t =>(ICollection<InvoiceIdentificatonRegEx>) t.InvoiceIdentificatonRegEx).HasForeignKey(d => d.InvoiceId);
              this.HasRequired(t => t.OCR_RegularExpressions).WithMany(t =>(ICollection<InvoiceIdentificatonRegEx>) t.InvoiceIdentificatonRegEx).HasForeignKey(d => d.RegExId);
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
