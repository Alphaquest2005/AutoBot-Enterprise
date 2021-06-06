namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_Error_DuplicateEntryMap : EntityTypeConfiguration<TODO_Error_DuplicateEntry>
    {
        public TODO_Error_DuplicateEntryMap()
        {                        
              this.HasKey(t => new {t.ASYCUDA_Id, t.Type, t.ApplicationSettingsId});        
              this.ToTable("TODO-Error-DuplicateEntry");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().IsUnicode(false).HasMaxLength(14);
              this.Property(t => t.Lines).HasColumnName("Lines");
              this.Property(t => t.id).HasColumnName("id").HasMaxLength(10);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Extended_customs_procedure).HasColumnName("Extended_customs_procedure").HasMaxLength(5);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
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
