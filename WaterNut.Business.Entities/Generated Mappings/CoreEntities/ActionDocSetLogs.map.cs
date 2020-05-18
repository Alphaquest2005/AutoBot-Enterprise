namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ActionDocSetLogsMap : EntityTypeConfiguration<ActionDocSetLogs>
    {
        public ActionDocSetLogsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ActionDocSetLogs");
              this.Property(t => t.ActonId).HasColumnName("ActonId");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasRequired(t => t.Actions).WithMany(t =>(ICollection<ActionDocSetLogs>) t.ActionDocSetLogs).HasForeignKey(d => d.ActonId);
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
