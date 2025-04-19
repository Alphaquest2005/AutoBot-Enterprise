namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_DiscrepanciesExecutionReportMap : EntityTypeConfiguration<TODO_DiscrepanciesExecutionReport>
    {
        public TODO_DiscrepanciesExecutionReportMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("TODO-DiscrepanciesExecutionReport");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.IsClassified).HasColumnName("IsClassified");
              this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType").HasMaxLength(50);
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(255);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(255);
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.xCNumber).HasColumnName("xCNumber").HasMaxLength(20);
              this.Property(t => t.xLineNumber).HasColumnName("xLineNumber");
              this.Property(t => t.xRegistrationDate).HasColumnName("xRegistrationDate");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasMaxLength(11);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
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
