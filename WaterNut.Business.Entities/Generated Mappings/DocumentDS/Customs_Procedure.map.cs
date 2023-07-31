namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class Customs_ProcedureMap : EntityTypeConfiguration<Customs_Procedure>
    {
        public Customs_ProcedureMap()
        {                        
              this.HasKey(t => t.Customs_ProcedureId);        
              this.ToTable("Customs_Procedure");
              this.Property(t => t.Document_TypeId).HasColumnName("Document_TypeId");
              this.Property(t => t.Customs_ProcedureId).HasColumnName("Customs_ProcedureId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Extended_customs_procedure).HasColumnName("Extended_customs_procedure").IsRequired().HasMaxLength(5);
              this.Property(t => t.National_customs_procedure).HasColumnName("National_customs_procedure").IsRequired().HasMaxLength(5);
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed)).IsRequired().HasMaxLength(11);
              this.Property(t => t.IsObsolete).HasColumnName("IsObsolete");
              this.Property(t => t.IsPaid).HasColumnName("IsPaid");
              this.Property(t => t.BondTypeId).HasColumnName("BondTypeId");
              this.Property(t => t.Stock).HasColumnName("Stock");
              this.Property(t => t.Discrepancy).HasColumnName("Discrepancy");
              this.Property(t => t.Adjustment).HasColumnName("Adjustment");
              this.Property(t => t.Sales).HasColumnName("Sales");
              this.Property(t => t.CustomsOperationId).HasColumnName("CustomsOperationId");
              this.Property(t => t.SubmitToCustoms).HasColumnName("SubmitToCustoms");
              this.Property(t => t.IsDefault).HasColumnName("IsDefault");
              this.Property(t => t.ExportSupportingEntryData).HasColumnName("ExportSupportingEntryData");
              this.HasRequired(t => t.CustomsOperation).WithMany(t =>(ICollection<Customs_Procedure>) t.Customs_Procedure).HasForeignKey(d => d.CustomsOperationId);
              this.HasRequired(t => t.Document_Type).WithMany(t =>(ICollection<Customs_Procedure>) t.Customs_Procedure).HasForeignKey(d => d.Document_TypeId);
              this.HasMany(t => t.AsycudaDocumentSets).WithOptional(t => t.Customs_Procedure).HasForeignKey(d => d.Customs_ProcedureId);
              this.HasMany(t => t.xcuda_ASYCUDA_ExtendedProperties).WithOptional(t => t.Customs_Procedure).HasForeignKey(d => d.Customs_ProcedureId);
              this.HasMany(t => t.InCustomsProcedure).WithRequired(t => (Customs_Procedure)t.InCustomsProcedure);
              this.HasMany(t => t.OutCustomsProcedure).WithRequired(t => (Customs_Procedure)t.OutCustomsProcedure);
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
