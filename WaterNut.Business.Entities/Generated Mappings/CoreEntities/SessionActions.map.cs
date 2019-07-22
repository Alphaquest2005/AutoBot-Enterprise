namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SessionActionsMap : EntityTypeConfiguration<SessionActions>
    {
        public SessionActionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("SessionActions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.SessionId).HasColumnName("SessionId");
              this.Property(t => t.ActionId).HasColumnName("ActionId");
              this.HasRequired(t => t.Actions).WithMany(t =>(ICollection<SessionActions>) t.SessionActions).HasForeignKey(d => d.ActionId);
              this.HasRequired(t => t.Sessions).WithMany(t =>(ICollection<SessionActions>) t.SessionActions).HasForeignKey(d => d.SessionId);
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
