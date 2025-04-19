namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_SubmitInadequatePackagesMap : EntityTypeConfiguration<TODO_SubmitInadequatePackages>
    {
        public TODO_SubmitInadequatePackagesMap()
        {                        
              this.HasKey(t => new {t.ApplicationSettingsId, t.AsycudaDocumentSetId});        
              this.ToTable("TODO-SubmitInadequatePackages");
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.MaxEntryLines).HasColumnName("MaxEntryLines");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.TotalPackages).HasColumnName("TotalPackages");
              this.Property(t => t.TotalLines).HasColumnName("TotalLines");
              this.Property(t => t.RequiredPackages).HasColumnName("RequiredPackages");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
