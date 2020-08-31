namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EntryDataExTotalsMap : EntityTypeConfiguration<EntryDataExTotals>
    {
        public EntryDataExTotalsMap()
        {                        
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryDataExTotals");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.AllocatedTotal).HasColumnName("AllocatedTotal");
              this.Property(t => t.TotalLines).HasColumnName("TotalLines");
              this.Property(t => t.Tax).HasColumnName("Tax");
              this.Property(t => t.ClassifiedLines).HasColumnName("ClassifiedLines");
              this.Property(t => t.LicenseLines).HasColumnName("LicenseLines");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.QtyLicensesRequired).HasColumnName("QtyLicensesRequired");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.HasRequired(t => t.EntryData).WithOptional(t => (EntryDataExTotals)t.EntryDataTotals);
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
