namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailMappingActionsMap : EntityTypeConfiguration<EmailMappingActions>
    {
        public EmailMappingActionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EmailMappingActions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EmailMappingId).HasColumnName("EmailMappingId");
              this.Property(t => t.ActionId).HasColumnName("ActionId");
              this.HasRequired(t => t.Actions).WithMany(t =>(ICollection<EmailMappingActions>) t.EmailMappingActions).HasForeignKey(d => d.ActionId);
              this.HasRequired(t => t.EmailMapping).WithMany(t =>(ICollection<EmailMappingActions>) t.EmailMappingActions).HasForeignKey(d => d.EmailMappingId);
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
