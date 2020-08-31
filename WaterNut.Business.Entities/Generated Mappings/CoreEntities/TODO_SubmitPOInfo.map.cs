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
              this.HasKey(t => new {t.Id, t.EntryDataId, t.AsycudaDocumentSetId, t.NewAsycuda_Id, t.AssessedAsycuda_Id, t.Status, t.Total_CIF, t.WarehouseNo});        
              this.ToTable("TODO-SubmitPOInfo");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Number).HasColumnName("Number").HasMaxLength(20);
              this.Property(t => t.Date).HasColumnName("Date").HasMaxLength(10);
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.SupplierInvoiceNo).HasColumnName("SupplierInvoiceNo").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.NewAsycuda_Id).HasColumnName("NewAsycuda_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.AssessedAsycuda_Id).HasColumnName("AssessedAsycuda_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Status).HasColumnName("Status").IsRequired().IsUnicode(false).HasMaxLength(11);
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasMaxLength(11);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.Totals_taxes).HasColumnName("Totals_taxes");
              this.Property(t => t.Total_CIF).HasColumnName("Total_CIF");
              this.Property(t => t.WarehouseNo).HasColumnName("WarehouseNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.BillingLine).HasColumnName("BillingLine").HasMaxLength(113);
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
