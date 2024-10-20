﻿namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class RegisteredMap : EntityTypeConfiguration<Registered>
    {
        public RegisteredMap()
        {                        
              this.HasKey(t => t.Value_declaration_form_Id);        
              this.ToTable("xC71_Value_declaration_form_Registered");
              this.Property(t => t.Value_declaration_form_Id).HasColumnName("Value_declaration_form_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.id).HasColumnName("id").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegNumber).HasColumnName("RegNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired().HasMaxLength(300);
              this.Property(t => t.DocumentReference).HasColumnName("DocumentReference").HasMaxLength(300);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
