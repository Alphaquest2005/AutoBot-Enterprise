namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CancelledEntriesLstMap : EntityTypeConfiguration<CancelledEntriesLst>
    {
        public CancelledEntriesLstMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("CancelledEntriesLst");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Office).HasColumnName("Office").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(8);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate").IsRequired().HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
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
