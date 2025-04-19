namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_SubmitIncompleteSupplierInfoMap : EntityTypeConfiguration<TODO_SubmitIncompleteSupplierInfo>
    {
        public TODO_SubmitIncompleteSupplierInfoMap()
        {                        
              this.HasKey(t => new {t.SupplierCode, t.EntryDataId, t.AsycudaDocumentSetId, t.ApplicationSettingsId});        
              this.ToTable("TODO-SubmitIncompleteSupplierInfo");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").IsRequired().HasMaxLength(100);
              this.Property(t => t.CountryCode).HasColumnName("CountryCode").HasMaxLength(3);
              this.Property(t => t.SupplierName).HasColumnName("SupplierName").HasMaxLength(510);
              this.Property(t => t.Street).HasColumnName("Street").HasMaxLength(100);
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
