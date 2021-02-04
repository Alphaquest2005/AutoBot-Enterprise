namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ApplicationSettingsMap : EntityTypeConfiguration<ApplicationSettings>
    {
        public ApplicationSettingsMap()
        {                        
              this.HasKey(t => t.ApplicationSettingsId);        
              this.ToTable("ApplicationSettings");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Description).HasColumnName("Description");
              this.Property(t => t.MaxEntryLines).HasColumnName("MaxEntryLines");
              this.Property(t => t.SoftwareName).HasColumnName("SoftwareName");
              this.Property(t => t.AllowCounterPoint).HasColumnName("AllowCounterPoint");
              this.Property(t => t.GroupEX9).HasColumnName("GroupEX9");
              this.Property(t => t.InvoicePerEntry).HasColumnName("InvoicePerEntry");
              this.Property(t => t.AllowTariffCodes).HasColumnName("AllowTariffCodes");
              this.Property(t => t.AllowWareHouse).HasColumnName("AllowWareHouse");
              this.Property(t => t.AllowXBond).HasColumnName("AllowXBond");
              this.Property(t => t.AllowAsycudaManager).HasColumnName("AllowAsycudaManager");
              this.Property(t => t.AllowQuickBooks).HasColumnName("AllowQuickBooks");
              this.Property(t => t.ItemDescriptionContainsAsycudaAttribute).HasColumnName("ItemDescriptionContainsAsycudaAttribute");
              this.Property(t => t.AllowExportToExcel).HasColumnName("AllowExportToExcel");
              this.Property(t => t.AllowAutoWeightCalculation).HasColumnName("AllowAutoWeightCalculation").HasMaxLength(50);
              this.Property(t => t.AllowEntryPerIM7).HasColumnName("AllowEntryPerIM7").HasMaxLength(50);
              this.Property(t => t.AllowSalesToPI).HasColumnName("AllowSalesToPI").HasMaxLength(50);
              this.Property(t => t.AllowEffectiveAssessmentDate).HasColumnName("AllowEffectiveAssessmentDate").HasMaxLength(50);
              this.Property(t => t.AllowAutoFreightCalculation).HasColumnName("AllowAutoFreightCalculation").HasMaxLength(50);
              this.Property(t => t.AllowSubItems).HasColumnName("AllowSubItems").HasMaxLength(50);
              this.Property(t => t.AllowEntryDoNotAllocate).HasColumnName("AllowEntryDoNotAllocate").HasMaxLength(50);
              this.Property(t => t.AllowPreviousItems).HasColumnName("AllowPreviousItems").HasMaxLength(50);
              this.Property(t => t.AllowOversShort).HasColumnName("AllowOversShort").HasMaxLength(50);
              this.Property(t => t.AllowContainers).HasColumnName("AllowContainers").HasMaxLength(50);
              this.Property(t => t.AllowNonXEntries).HasColumnName("AllowNonXEntries").HasMaxLength(50);
              this.Property(t => t.AllowValidateTariffCodes).HasColumnName("AllowValidateTariffCodes").HasMaxLength(50);
              this.Property(t => t.AllowCleanBond).HasColumnName("AllowCleanBond").HasMaxLength(50);
              this.Property(t => t.OrderEntriesBy).HasColumnName("OrderEntriesBy").HasMaxLength(50);
              this.Property(t => t.OpeningStockDate).HasColumnName("OpeningStockDate");
              this.Property(t => t.DeclarantCode).HasColumnName("DeclarantCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.WeightCalculationMethod).HasColumnName("WeightCalculationMethod").HasMaxLength(50);
              this.Property(t => t.BondQuantum).HasColumnName("BondQuantum");
              this.Property(t => t.DataFolder).HasColumnName("DataFolder").HasMaxLength(999);
              this.Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasMaxLength(50);
              this.Property(t => t.IsActive).HasColumnName("IsActive");
              this.Property(t => t.Email).HasColumnName("Email").HasMaxLength(255);
              this.Property(t => t.EmailPassword).HasColumnName("EmailPassword").HasMaxLength(50);
              this.Property(t => t.AsycudaLogin).HasColumnName("AsycudaLogin").HasMaxLength(50);
              this.Property(t => t.AsycudaPassword).HasColumnName("AsycudaPassword").HasMaxLength(50);
              this.Property(t => t.AssessIM7).HasColumnName("AssessIM7");
              this.Property(t => t.AssessEX).HasColumnName("AssessEX");
              this.Property(t => t.TestMode).HasColumnName("TestMode");
              this.Property(t => t.BondTypeId).HasColumnName("BondTypeId");
              this.Property(t => t.RequirePOs).HasColumnName("RequirePOs");
              this.HasMany(t => t.AsycudaDocumentSetEx).WithRequired(t => (ApplicationSettings)t.ApplicationSettings);
              this.HasMany(t => t.AsycudaDocument).WithOptional(t => t.ApplicationSettings).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.AsycudaDocumentItem).WithOptional(t => t.ApplicationSettings).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.InventoryItemsEx).WithRequired(t => (ApplicationSettings)t.ApplicationSettings);
              this.HasMany(t => t.FileTypes).WithRequired(t => (ApplicationSettings)t.ApplicationSettings);
              this.HasMany(t => t.InfoMapping).WithRequired(t => (ApplicationSettings)t.ApplicationSettings);
              this.HasMany(t => t.EmailMapping).WithRequired(t => (ApplicationSettings)t.ApplicationSettings);
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
