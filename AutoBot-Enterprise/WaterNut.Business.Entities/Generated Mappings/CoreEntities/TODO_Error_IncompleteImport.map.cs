namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_Error_IncompleteImportMap : EntityTypeConfiguration<TODO_Error_IncompleteImport>
    {
        public TODO_Error_IncompleteImportMap()
        {                        
              this.HasKey(t => new {t.ASYCUDA_Id, t.Type});        
              this.ToTable("TODO-Error-IncompleteImport");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ImportComplete).HasColumnName("ImportComplete");
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().IsUnicode(false).HasMaxLength(16);
              this.Property(t => t.ExpectedLines).HasColumnName("ExpectedLines").HasMaxLength(20);
              this.Property(t => t.ImportedLines).HasColumnName("ImportedLines");
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(50);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.ReferenceNumber).HasColumnName("ReferenceNumber").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(20);
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
