namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_DiscrepancyPreExecutionReportMap : EntityTypeConfiguration<TODO_DiscrepancyPreExecutionReport>
    {
        public TODO_DiscrepancyPreExecutionReportMap()
        {                        
              this.HasKey(t => new {t.EntryDataDetailsId, t.ApplicationSettingsId});        
              this.ToTable("TODO-DiscrepancyPreExecutionReport");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(15);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.Cost).HasColumnName("Cost");
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.UnitWeight).HasColumnName("UnitWeight");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.CLineNumber).HasColumnName("CLineNumber");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.InvoiceQty).HasColumnName("InvoiceQty");
              this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(255);
              this.Property(t => t.PreviousCNumber).HasColumnName("PreviousCNumber").HasMaxLength(255);
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(255);
              this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(50);
              this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(4);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.Subject).HasColumnName("Subject");
              this.Property(t => t.EmailDate).HasColumnName("EmailDate");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").HasMaxLength(50);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
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
