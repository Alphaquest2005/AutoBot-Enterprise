﻿namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Suppliers_documentsMap : EntityTypeConfiguration<xcuda_Suppliers_documents>
    {
        public xcuda_Suppliers_documentsMap()
        {                        
              this.HasKey(t => t.Suppliers_documents_Id);        
              this.ToTable("xcuda_Suppliers_documents");
              this.Property(t => t.Suppliers_document_date).HasColumnName("Suppliers_document_date");
              this.Property(t => t.Suppliers_documents_Id).HasColumnName("Suppliers_documents_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Suppliers_document_itmlink).HasColumnName("Suppliers_document_itmlink");
              this.Property(t => t.Suppliers_document_code).HasColumnName("Suppliers_document_code");
              this.Property(t => t.Suppliers_document_name).HasColumnName("Suppliers_document_name");
              this.Property(t => t.Suppliers_document_country).HasColumnName("Suppliers_document_country");
              this.Property(t => t.Suppliers_document_city).HasColumnName("Suppliers_document_city");
              this.Property(t => t.Suppliers_document_street).HasColumnName("Suppliers_document_street");
              this.Property(t => t.Suppliers_document_telephone).HasColumnName("Suppliers_document_telephone");
              this.Property(t => t.Suppliers_document_fax).HasColumnName("Suppliers_document_fax");
              this.Property(t => t.Suppliers_document_zip_code).HasColumnName("Suppliers_document_zip_code");
              this.Property(t => t.Suppliers_document_invoice_nbr).HasColumnName("Suppliers_document_invoice_nbr");
              this.Property(t => t.Suppliers_document_invoice_amt).HasColumnName("Suppliers_document_invoice_amt");
              this.Property(t => t.Suppliers_document_type_code).HasColumnName("Suppliers_document_type_code");
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Suppliers_documents>) t.xcuda_Suppliers_documents).HasForeignKey(d => d.ASYCUDA_Id);
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
