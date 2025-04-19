namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Taxation_lineMap : EntityTypeConfiguration<xcuda_Taxation_line>
    {
        public xcuda_Taxation_lineMap()
        {                        
              this.HasKey(t => t.Taxation_line_Id);        
              this.ToTable("xcuda_Taxation_line");
              this.Property(t => t.Duty_tax_Base).HasColumnName("Duty_tax_Base").HasMaxLength(20);
              this.Property(t => t.Duty_tax_rate).HasColumnName("Duty_tax_rate");
              this.Property(t => t.Duty_tax_amount).HasColumnName("Duty_tax_amount");
              this.Property(t => t.Taxation_line_Id).HasColumnName("Taxation_line_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Taxation_Id).HasColumnName("Taxation_Id");
              this.Property(t => t.Duty_tax_code).HasColumnName("Duty_tax_code").HasMaxLength(20);
              this.Property(t => t.Duty_tax_MP).HasColumnName("Duty_tax_MP").HasMaxLength(20);
              this.HasOptional(t => t.xcuda_Taxation).WithMany(t =>(ICollection<xcuda_Taxation_line>) t.xcuda_Taxation_line).HasForeignKey(d => d.Taxation_Id);
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
