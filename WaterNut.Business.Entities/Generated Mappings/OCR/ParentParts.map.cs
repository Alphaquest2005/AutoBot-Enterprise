namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ParentPartsMap : EntityTypeConfiguration<ParentParts>
    {
        public ParentPartsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-ParentPart");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ParentPartId).HasColumnName("ParentPartId");
              this.HasRequired(t => t.Part).WithOptional(t => (ParentParts)t.ParentPart);
              this.HasRequired(t => t.ParentPart).WithMany(t =>(ICollection<ParentParts>) t.ChildParts).HasForeignKey(d => d.ParentPartId);
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
