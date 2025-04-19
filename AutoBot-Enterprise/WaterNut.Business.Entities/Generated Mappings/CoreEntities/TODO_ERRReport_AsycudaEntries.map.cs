namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_ERRReport_AsycudaEntriesMap : EntityTypeConfiguration<TODO_ERRReport_AsycudaEntries>
    {
        public TODO_ERRReport_AsycudaEntriesMap()
        {                        
              this.HasKey(t => t.Error);        
              this.ToTable("TODO-ERRReport-AsycudaEntries");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(50);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(50);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.Error).HasColumnName("Error").IsRequired().IsUnicode(false).HasMaxLength(16);
              this.Property(t => t.Info).HasColumnName("Info").HasMaxLength(130);
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
