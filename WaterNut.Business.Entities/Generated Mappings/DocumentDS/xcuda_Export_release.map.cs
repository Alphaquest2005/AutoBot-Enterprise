namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Export_releaseMap : EntityTypeConfiguration<xcuda_Export_release>
    {
        public xcuda_Export_releaseMap()
        {                        
              this.HasKey(t => t.Export_release_Id);        
              this.ToTable("xcuda_Export_release");
              this.Property(t => t.Date_of_exit).HasColumnName("Date_of_exit").HasMaxLength(20);
              this.Property(t => t.Time_of_exit).HasColumnName("Time_of_exit").HasMaxLength(20);
              this.Property(t => t.Export_release_Id).HasColumnName("Export_release_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Export_release>) t.xcuda_Export_release).HasForeignKey(d => d.ASYCUDA_Id);
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
