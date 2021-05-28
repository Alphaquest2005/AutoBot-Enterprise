namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class C71SummaryMap : EntityTypeConfiguration<C71Summary>
    {
        public C71SummaryMap()
        {                        
              this.HasKey(t => t.Value_declaration_form_Id);        
              this.ToTable("C71Summary");
              this.Property(t => t.Address).HasColumnName("Address").HasMaxLength(255);
              this.Property(t => t.Value_declaration_form_Id).HasColumnName("Value_declaration_form_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.RegisteredId).HasColumnName("RegisteredId");
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(255);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").HasMaxLength(300);
              this.Property(t => t.RegNumber).HasColumnName("RegNumber").HasMaxLength(50);
              this.Property(t => t.DocumentReference).HasColumnName("DocumentReference").HasMaxLength(50);
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
