namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentBasicInfoMap : EntityTypeConfiguration<AsycudaDocumentBasicInfo>
    {
        public AsycudaDocumentBasicInfoMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("AsycudaDocumentBasicInfo");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Extended_customs_procedure).HasColumnName("Extended_customs_procedure").IsRequired().HasMaxLength(5);
              this.Property(t => t.National_customs_procedure).HasColumnName("National_customs_procedure").IsRequired().HasMaxLength(5);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.AssessmentDate).HasColumnName("AssessmentDate");
              this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.IsManuallyAssessed).HasColumnName("IsManuallyAssessed");
              this.Property(t => t.Cancelled).HasColumnName("Cancelled");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.ImportComplete).HasColumnName("ImportComplete");
              this.Property(t => t.Customs_ProcedureId).HasColumnName("Customs_ProcedureId");
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").IsRequired().HasMaxLength(11);
              this.Property(t => t.SourceFileName).HasColumnName("SourceFileName").HasMaxLength(500);
              this.Property(t => t.SubmitToCustoms).HasColumnName("SubmitToCustoms");
              this.Property(t => t.IsPaid).HasColumnName("IsPaid");
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
