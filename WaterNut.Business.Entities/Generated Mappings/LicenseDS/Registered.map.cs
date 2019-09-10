namespace LicenseDS.Business.Entities.Mapping
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
              this.HasKey(t => t.LicenseId);        
              this.ToTable("xLIC_License_Registered");
              this.Property(t => t.LicenseId).HasColumnName("LicenseId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
