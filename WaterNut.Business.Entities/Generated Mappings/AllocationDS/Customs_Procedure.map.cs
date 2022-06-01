namespace AllocationDS.Business.Entities.Mapping
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
              this.Property(t => t.Extended_customs_procedure).HasColumnName("Extended_customs_procedure").HasMaxLength(5);
              this.Property(t => t.National_customs_procedure).HasColumnName("National_customs_procedure").HasMaxLength(5);
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed)).HasMaxLength(11);
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
              this.HasRequired(t => t.CustomsOperations).WithMany(t =>(ICollection<Customs_Procedure>) t.Customs_Procedure).HasForeignKey(d => d.CustomsOperationId);
              this.HasMany(t => t.AsycudaDocument).WithOptional(t => t.Customs_Procedure).HasForeignKey(d => d.Customs_ProcedureId);
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
