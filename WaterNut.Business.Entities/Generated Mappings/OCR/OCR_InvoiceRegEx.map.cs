namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_InvoiceRegExMap : EntityTypeConfiguration<OCR_InvoiceRegEx>
    {
        public OCR_InvoiceRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-InvoiceRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.ReplacementRegExId).HasColumnName("ReplacementRegExId");
              this.HasRequired(t => t.OCR_Invoices).WithMany(t =>(ICollection<OCR_InvoiceRegEx>) t.OCR_InvoiceRegEx).HasForeignKey(d => d.InvoiceId);
              this.HasRequired(t => t.RegEx).WithMany(t =>(ICollection<OCR_InvoiceRegEx>) t.OCR_InvoiceRegEx).HasForeignKey(d => d.RegExId);
              this.HasRequired(t => t.ReplacementRegEx).WithMany(t =>(ICollection<OCR_InvoiceRegEx>) t.OCR_InvoiceRegEx1).HasForeignKey(d => d.ReplacementRegExId);
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
