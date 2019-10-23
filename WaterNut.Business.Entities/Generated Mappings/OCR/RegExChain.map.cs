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
              this.HasRequired(t => t.RegularExpressions).WithMany(t =>(ICollection<RegExChain>) t.RegExChain).HasForeignKey(d => d.RegExId);
              this.HasMany(t => t.End).WithRequired(t => (RegExChain)t.RegExChain);
              this.HasMany(t => t.Start).WithRequired(t => (RegExChain)t.RegExChain);
              this.HasMany(t => t.Lines).WithRequired(t => (RegExChain)t.RegExChain);
              this.HasOptional(t => t.RegExParent).WithRequired(t => (RegExChain)t.RegExChain);
              this.HasMany(t => t.PreviousRegExParent).WithRequired(t => (RegExChain)t.PreviousRegExParent);
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
