namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_DiscrepanciesToSubmitMap : EntityTypeConfiguration<TODO_DiscrepanciesToSubmit>
    {
        public TODO_DiscrepanciesToSubmitMap()
        {                        
              this.HasKey(t => new {t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.InvoiceNo, t.InvoiceDate, t.ItemNumber});        
              this.ToTable("TODO-DiscrepanciesToSubmit");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.IsClassified).HasColumnName("IsClassified");
              this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType").HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(50);
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.Comment).HasColumnName("Comment").IsUnicode(false).HasMaxLength(24);
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
