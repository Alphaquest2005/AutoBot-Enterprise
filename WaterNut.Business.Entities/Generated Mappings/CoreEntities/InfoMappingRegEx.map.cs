namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InfoMappingRegExMap : EntityTypeConfiguration<InfoMappingRegEx>
    {
        public InfoMappingRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("InfoMappingRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InfoMappingId).HasColumnName("InfoMappingId");
              this.Property(t => t.KeyRegX).HasColumnName("KeyRegX").IsRequired();
              this.Property(t => t.FieldRx).HasColumnName("FieldRx").IsRequired();
              this.Property(t => t.KeyReplaceRx).HasColumnName("KeyReplaceRx");
              this.Property(t => t.FieldReplaceRx).HasColumnName("FieldReplaceRx");
              this.HasRequired(t => t.InfoMapping).WithMany(t =>(ICollection<InfoMappingRegEx>) t.InfoMappingRegEx).HasForeignKey(d => d.InfoMappingId);
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
