namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCRFileTypesMap : EntityTypeConfiguration<OCRFileTypes>
    {
        public OCRFileTypesMap()
        {                        
              this.HasKey(t => new {t.OCRInvoiceId, t.FileTypeId});        
              this.ToTable("OCRInvoicesFileTypes");
              this.Property(t => t.OCRInvoiceId).HasColumnName("OCRInvoiceId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.Invoices).WithMany(t =>(ICollection<OCRFileTypes>) t.FileTypes).HasForeignKey(d => d.OCRInvoiceId);
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
