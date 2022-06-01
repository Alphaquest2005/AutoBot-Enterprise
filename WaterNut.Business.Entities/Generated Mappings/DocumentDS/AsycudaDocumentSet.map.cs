namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetMap : EntityTypeConfiguration<AsycudaDocumentSet>
    {
        public AsycudaDocumentSetMap()
        {                        
              this.HasKey(t => t.AsycudaDocumentSetId);        
              this.ToTable("AsycudaDocumentSet");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
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
              this.Property(t => t.ApportionMethod).HasColumnName("ApportionMethod").HasMaxLength(50);
              this.Property(t => t.TotalWeight).HasColumnName("TotalWeight");
              this.Property(t => t.TotalFreight).HasColumnName("TotalFreight");
              this.Property(t => t.TotalPackages).HasColumnName("TotalPackages");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.LastFileNumber).HasColumnName("LastFileNumber");
              this.Property(t => t.TotalInvoices).HasColumnName("TotalInvoices");
              this.Property(t => t.MaxLines).HasColumnName("MaxLines");
              this.Property(t => t.LocationOfGoods).HasColumnName("LocationOfGoods").HasMaxLength(50);
              this.Property(t => t.FreightCurrencyCode).HasColumnName("FreightCurrencyCode").IsRequired().HasMaxLength(3);
              this.Property(t => t.Office).HasColumnName("Office").HasMaxLength(50);
              this.Property(t => t.UpgradeKey).HasColumnName("UpgradeKey");
              this.Property(t => t.ExpectedEntries).HasColumnName("ExpectedEntries");
              this.HasOptional(t => t.Customs_Procedure).WithMany(t =>(ICollection<AsycudaDocumentSet>) t.AsycudaDocumentSets).HasForeignKey(d => d.Customs_ProcedureId);
              this.HasOptional(t => t.Document_Type).WithMany(t =>(ICollection<AsycudaDocumentSet>) t.AsycudaDocumentSets).HasForeignKey(d => d.Document_TypeId);
              this.HasMany(t => t.AsycudaDocumentSetEntryDatas).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
              this.HasMany(t => t.xcuda_ASYCUDA_ExtendedProperties).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
              this.HasMany(t => t.AsycudaDocumentSet_Attachments).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
              this.HasOptional(t => t.Container).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
              this.HasOptional(t => t.SystemDocumentSet).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
              this.HasMany(t => t.FileTypes).WithRequired(t => (AsycudaDocumentSet)t.AsycudaDocumentSet);
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
