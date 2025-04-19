namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ParameterSetParametersMap : EntityTypeConfiguration<ParameterSetParameters>
    {
        public ParameterSetParametersMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ParameterSetParameters");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ParameterSetId).HasColumnName("ParameterSetId");
              this.Property(t => t.ParameterId).HasColumnName("ParameterId");
              this.HasRequired(t => t.Parameters).WithMany(t =>(ICollection<ParameterSetParameters>) t.ParameterSetParameters).HasForeignKey(d => d.ParameterId);
              this.HasRequired(t => t.ParameterSet).WithMany(t =>(ICollection<ParameterSetParameters>) t.ParameterSetParameters).HasForeignKey(d => d.ParameterSetId);
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
