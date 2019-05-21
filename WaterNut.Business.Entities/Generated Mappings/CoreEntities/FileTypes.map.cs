namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypesMap : EntityTypeConfiguration<FileTypes>
    {
        public FileTypesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypes");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.FilePattern).HasColumnName("FilePattern").IsRequired().HasMaxLength(255);
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().HasMaxLength(50);
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.CreateDocumentSet).HasColumnName("CreateDocumentSet");
              this.Property(t => t.DocumentSpecific).HasColumnName("DocumentSpecific");
              this.Property(t => t.DocumentCode).HasColumnName("DocumentCode").HasMaxLength(50);
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<FileTypes>) t.FileTypes).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasRequired(t => t.AsycudaDocumentSetEx).WithMany(t =>(ICollection<FileTypes>) t.FileTypes).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasMany(t => t.FileTypeMappings).WithRequired(t => (FileTypes)t.FileTypes);
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
