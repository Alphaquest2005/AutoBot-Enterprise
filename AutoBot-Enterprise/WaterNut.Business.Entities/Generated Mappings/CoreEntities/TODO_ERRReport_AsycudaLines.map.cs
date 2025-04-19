namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_ERRReport_AsycudaLinesMap : EntityTypeConfiguration<TODO_ERRReport_AsycudaLines>
    {
        public TODO_ERRReport_AsycudaLinesMap()
        {                        
              this.HasKey(t => t.Error);        
              this.ToTable("TODO-ERRReport-AsycudaLines");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.Description).HasColumnName("Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Error).HasColumnName("Error").IsRequired().IsUnicode(false).HasMaxLength(26);
              this.Property(t => t.Info).HasColumnName("Info").HasMaxLength(66);
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
