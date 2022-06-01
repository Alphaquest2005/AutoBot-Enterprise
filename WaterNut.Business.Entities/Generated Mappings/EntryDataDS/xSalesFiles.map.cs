namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xSalesFilesMap : EntityTypeConfiguration<xSalesFiles>
    {
        public xSalesFilesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("xSalesFiles");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.StartDate).HasColumnName("StartDate");
              this.Property(t => t.EndDate).HasColumnName("EndDate");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").HasMaxLength(255);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Asycuda_Id).HasColumnName("Asycuda_Id");
              this.Property(t => t.IsImported).HasColumnName("IsImported");
              this.HasMany(t => t.xSalesDetails).WithRequired(t => (xSalesFiles)t.xSalesFiles);
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
