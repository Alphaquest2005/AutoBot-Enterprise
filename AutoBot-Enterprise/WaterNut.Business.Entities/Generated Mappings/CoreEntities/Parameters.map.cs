namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ParametersMap : EntityTypeConfiguration<Parameters>
    {
        public ParametersMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("Parameters");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.Property(t => t.Value).HasColumnName("Value").IsRequired().HasMaxLength(50);
              this.HasMany(t => t.ParameterSetParameters).WithRequired(t => (Parameters)t.Parameters);
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
