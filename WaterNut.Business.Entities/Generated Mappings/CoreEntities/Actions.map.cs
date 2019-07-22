namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ActionsMap : EntityTypeConfiguration<Actions>
    {
        public ActionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("Actions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.HasMany(t => t.FileTypeActions).WithRequired(t => (Actions)t.Actions);
              this.HasMany(t => t.SessionActions).WithRequired(t => (Actions)t.Actions);
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
