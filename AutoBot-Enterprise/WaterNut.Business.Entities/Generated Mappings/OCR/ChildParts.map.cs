namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ChildPartsMap : EntityTypeConfiguration<ChildParts>
    {
        public ChildPartsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-ChildPart");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ParentPartId).HasColumnName("ParentPartId");
              this.Property(t => t.ChildPartId).HasColumnName("ChildPartId");
              this.HasRequired(t => t.ChildPart).WithMany(t =>(ICollection<ChildParts>) t.ChildParts).HasForeignKey(d => d.ChildPartId);
              this.HasRequired(t => t.ParentPart).WithMany(t =>(ICollection<ChildParts>) t.ParentParts).HasForeignKey(d => d.ParentPartId);
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
