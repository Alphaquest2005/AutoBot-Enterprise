namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class StartMap : EntityTypeConfiguration<Start>
    {
        public StartMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-Start");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.PartId).HasColumnName("PartId");
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.HasRequired(t => t.Parts).WithMany(t =>(ICollection<Start>) t.Start).HasForeignKey(d => d.PartId);
              this.HasRequired(t => t.RegularExpressions).WithMany(t =>(ICollection<Start>) t.Start).HasForeignKey(d => d.RegExId);
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
