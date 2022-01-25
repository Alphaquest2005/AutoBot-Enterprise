namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_SubmitPOInfoMap : EntityTypeConfiguration<TODO_SubmitPOInfo>
    {
        public TODO_SubmitPOInfoMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("TODO-SubmitPOInfo");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Number).HasColumnName("Number").HasMaxLength(20);
              this.Property(t => t.Date).HasColumnName("Date").HasMaxLength(10);
              this.Property(t => t.SupplierInvoiceNo).HasColumnName("SupplierInvoiceNo").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.Status).HasColumnName("Status").IsRequired().IsUnicode(false).HasMaxLength(11);
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasMaxLength(11);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.Totals_taxes).HasColumnName("Totals_taxes");
              this.Property(t => t.Total_CIF).HasColumnName("Total_CIF");
              this.Property(t => t.WarehouseNo).HasColumnName("WarehouseNo").HasMaxLength(50);
              this.Property(t => t.BillingLine).HasColumnName("BillingLine").HasMaxLength(113);
              this.Property(t => t.IsSubmitted).HasColumnName("IsSubmitted");
              this.Property(t => t.PONumber).HasColumnName("PONumber").HasMaxLength(50);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Marks2_of_packages).HasColumnName("Marks2_of_packages").HasMaxLength(40);
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
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
