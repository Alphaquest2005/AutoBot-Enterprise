﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_LICToAssessMap : EntityTypeConfiguration<TODO_LICToAssess>
    {
        public TODO_LICToAssessMap()
        {                        
              this.HasKey(t => new {t.AsycudaDocumentSetId, t.ApplicationSettingsId, t.Currency_Code});        
              this.ToTable("TODO-LICToAssess");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").HasMaxLength(3);
              this.Property(t => t.Currency_Code).HasColumnName("Currency_Code").IsRequired().HasMaxLength(3);
              this.Property(t => t.Manifest_Number).HasColumnName("Manifest_Number").HasMaxLength(50);
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").HasMaxLength(50);
              this.Property(t => t.Type_of_declaration).HasColumnName("Type_of_declaration").HasMaxLength(10);
              this.Property(t => t.Declaration_gen_procedure_code).HasColumnName("Declaration_gen_procedure_code").HasMaxLength(10);
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.TotalInvoices).HasColumnName("TotalInvoices");
              this.Property(t => t.DocumentsCount).HasColumnName("DocumentsCount");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.LicenseLines).HasColumnName("LicenseLines");
              this.Property(t => t.TotalCIF).HasColumnName("TotalCIF");
              this.Property(t => t.QtyLicensesRequired).HasColumnName("QtyLicensesRequired");
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
