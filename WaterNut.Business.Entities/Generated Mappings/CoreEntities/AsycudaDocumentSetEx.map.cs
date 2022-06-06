namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetExMap : EntityTypeConfiguration<AsycudaDocumentSetEx>
    {
        public AsycudaDocumentSetExMap()
        {                        
              this.HasKey(t => t.AsycudaDocumentSetId);        
              this.ToTable("AsycudaDocumentSetEx");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.Exchange_Rate).HasColumnName("Exchange_Rate");
              this.Property(t => t.Customs_ProcedureId).HasColumnName("Customs_ProcedureId");
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").HasMaxLength(3);
              this.Property(t => t.Currency_Code).HasColumnName("Currency_Code").IsRequired().HasMaxLength(3);
              this.Property(t => t.Document_TypeId).HasColumnName("Document_TypeId");
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(255);
              this.Property(t => t.Manifest_Number).HasColumnName("Manifest_Number").HasMaxLength(50);
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").HasMaxLength(50);
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.StartingFileCount).HasColumnName("StartingFileCount");
              this.Property(t => t.DocumentsCount).HasColumnName("DocumentsCount");
              this.Property(t => t.ApportionMethod).HasColumnName("ApportionMethod").HasMaxLength(50);
              this.Property(t => t.TotalCIF).HasColumnName("TotalCIF");
              this.Property(t => t.TotalFreight).HasColumnName("TotalFreight");
              this.Property(t => t.TotalWeight).HasColumnName("TotalWeight");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.TotalPackages).HasColumnName("TotalPackages");
              this.Property(t => t.LastFileNumber).HasColumnName("LastFileNumber");
              this.Property(t => t.TotalInvoices).HasColumnName("TotalInvoices");
              this.Property(t => t.ImportedInvoices).HasColumnName("ImportedInvoices");
              this.Property(t => t.ClassifiedLines).HasColumnName("ClassifiedLines");
              this.Property(t => t.TotalLines).HasColumnName("TotalLines");
              this.Property(t => t.MaxLines).HasColumnName("MaxLines");
              this.Property(t => t.LocationOfGoods).HasColumnName("LocationOfGoods").HasMaxLength(50);
              this.Property(t => t.LicenseLines).HasColumnName("LicenseLines");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.FreightCurrencyCode).HasColumnName("FreightCurrencyCode").IsRequired().HasMaxLength(3);
              this.Property(t => t.QtyLicensesRequired).HasColumnName("QtyLicensesRequired");
              this.Property(t => t.EntryPackages).HasColumnName("EntryPackages");
              this.Property(t => t.CurrencyRate).HasColumnName("CurrencyRate");
              this.Property(t => t.FreightCurrencyRate).HasColumnName("FreightCurrencyRate");
              this.Property(t => t.ExpectedEntries).HasColumnName("ExpectedEntries");
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<AsycudaDocumentSetEx>) t.AsycudaDocumentSetEx).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.AsycudaDocuments).WithOptional(t => t.AsycudaDocumentSetEx).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasMany(t => t.LicenceSummary).WithRequired(t => (AsycudaDocumentSetEx)t.AsycudaDocumentSetEx);
              this.HasMany(t => t.AsycudaDocumentSet_Attachments).WithRequired(t => (AsycudaDocumentSetEx)t.AsycudaDocumentSetEx);
              this.HasMany(t => t.AsycudaDocumentSetEntryDataEx).WithRequired(t => (AsycudaDocumentSetEx)t.AsycudaDocumentSetEx);
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
