namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SessionScheduleMap : EntityTypeConfiguration<SessionSchedule>
    {
        public SessionScheduleMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("SessionSchedule");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.SesseionId).HasColumnName("SesseionId");
              this.Property(t => t.RunDateTime).HasColumnName("RunDateTime");
              this.Property(t => t.ApplicationSettingId).HasColumnName("ApplicationSettingId");
              this.Property(t => t.ActionId).HasColumnName("ActionId");
              this.Property(t => t.ParameterSetId).HasColumnName("ParameterSetId");
              this.HasRequired(t => t.Sessions).WithMany(t =>(ICollection<SessionSchedule>) t.SessionSchedule).HasForeignKey(d => d.SesseionId);
              this.HasOptional(t => t.ParameterSet).WithMany(t =>(ICollection<SessionSchedule>) t.SessionSchedule).HasForeignKey(d => d.ParameterSetId);
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
