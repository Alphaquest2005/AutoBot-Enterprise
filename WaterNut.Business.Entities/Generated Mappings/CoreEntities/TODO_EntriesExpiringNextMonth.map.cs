namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_EntriesExpiringNextMonthMap : EntityTypeConfiguration<TODO_EntriesExpiringNextMonth>
    {
        public TODO_EntriesExpiringNextMonthMap()
        {                        
              this.HasKey(t => new {t.Type, t.ApplicationSettingsId, t.ASYCUDA_Id});        
              this.ToTable("TODO-EntriesExpiringNextMonth");
              this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().IsUnicode(false).HasMaxLength(13);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
