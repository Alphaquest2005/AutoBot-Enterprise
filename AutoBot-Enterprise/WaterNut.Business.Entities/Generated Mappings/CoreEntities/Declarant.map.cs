namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class DeclarantMap : EntityTypeConfiguration<Declarant>
    {
        public DeclarantMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ApplicationSettings-Declarants");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.DeclarantCode).HasColumnName("DeclarantCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.IsDefault).HasColumnName("IsDefault");
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<Declarant>) t.Declarants).HasForeignKey(d => d.ApplicationSettingsId);
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
