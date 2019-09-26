namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EDDocumentTypesMap : EntityTypeConfiguration<EDDocumentTypes>
    {
        public EDDocumentTypesMap()
        {                        
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("EntryData_DocumentType");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(50);
              this.HasRequired(t => t.EntryData).WithOptional(t => (EDDocumentTypes)t.DocumentType);
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
