﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ExpiredEntriesLstMap : EntityTypeConfiguration<ExpiredEntriesLst>
    {
        public ExpiredEntriesLstMap()
        {                        
              this.HasKey(t => new {t.Office, t.RegistrationDate, t.RegistrationNumber});        
              this.ToTable("ExpiredEntriesLst");
              this.Property(t => t.Office).HasColumnName("Office").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegistrationSerial).HasColumnName("RegistrationSerial").IsRequired().HasMaxLength(1);
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(8);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate").IsRequired().HasMaxLength(50);
              this.Property(t => t.AssessmentSerial).HasColumnName("AssessmentSerial").IsRequired().HasMaxLength(1);
              this.Property(t => t.AssessmentNumber).HasColumnName("AssessmentNumber").IsRequired().HasMaxLength(8);
              this.Property(t => t.AssessmentDate).HasColumnName("AssessmentDate").IsRequired().HasMaxLength(50);
              this.Property(t => t.DeclarantCode).HasColumnName("DeclarantCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.DeclarantReference).HasColumnName("DeclarantReference").IsRequired().HasMaxLength(50);
              this.Property(t => t.Exporter).HasColumnName("Exporter").HasMaxLength(50);
              this.Property(t => t.Consignee).HasColumnName("Consignee").HasMaxLength(50);
              this.Property(t => t.Expiration).HasColumnName("Expiration").IsRequired().HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.GeneralProcedure).HasColumnName("GeneralProcedure").IsRequired().HasMaxLength(1);
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
