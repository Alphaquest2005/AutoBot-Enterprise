namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_LicenseToXMLMap : EntityTypeConfiguration<TODO_LicenseToXML>
    {
        public TODO_LicenseToXMLMap()
        {                        
              this.HasKey(t => new {t.AsycudaDocumentSetId, t.ApplicationSettingsId, t.EntryDataId, t.ItemNumber, t.ItemDescription});        
              this.ToTable("TODO-LicenseToXML");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(52);
              this.Property(t => t.TariffCategoryCode).HasColumnName("TariffCategoryCode").HasMaxLength(50);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.LicenseDescription).HasColumnName("LicenseDescription").HasMaxLength(50);
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.Country_of_origin_code).HasColumnName("Country_of_origin_code").HasMaxLength(3);
              this.Property(t => t.UOM).HasColumnName("UOM").HasMaxLength(50);
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").IsRequired().HasMaxLength(255);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.VolumeLiters).HasColumnName("VolumeLiters");
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
