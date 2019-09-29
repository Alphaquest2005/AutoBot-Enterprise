namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class RegExChainMap : EntityTypeConfiguration<RegExChain>
    {
        public RegExChainMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-RegExChain");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.PreviousRegExId).HasColumnName("PreviousRegExId");
              this.HasOptional(t => t.RegExChain2).WithMany(t =>(ICollection<RegExChain>) t.RegExChain1).HasForeignKey(d => d.PreviousRegExId);
              this.HasRequired(t => t.RegularExpressions).WithMany(t =>(ICollection<RegExChain>) t.RegExChain).HasForeignKey(d => d.RegExId);
              this.HasMany(t => t.RegExChain1).WithOptional(t => t.RegExChain2).HasForeignKey(d => d.PreviousRegExId);
              this.HasMany(t => t.TemplateLinesRegularExpressions).WithRequired(t => (RegExChain)t.RegExChain);
              this.HasMany(t => t.TemplateRegularExpressions).WithRequired(t => (RegExChain)t.RegExChain);
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
