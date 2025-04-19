namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_ERRReport_SubmitWarehouseErrorsMap : EntityTypeConfiguration<TODO_ERRReport_SubmitWarehouseErrors>
    {
        public TODO_ERRReport_SubmitWarehouseErrorsMap()
        {                        
              this.HasKey(t => new {t.Error, t.LineNumber, t.ApplicationSettingsId, t.CNumber});        
              this.ToTable("TODO-ERRReport-SubmitWarehouseErrors");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.CNumber).HasColumnName("CNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.Description).HasColumnName("Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Error).HasColumnName("Error").IsRequired().IsUnicode(false).HasMaxLength(26);
              this.Property(t => t.Info).HasColumnName("Info").HasMaxLength(62);
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
