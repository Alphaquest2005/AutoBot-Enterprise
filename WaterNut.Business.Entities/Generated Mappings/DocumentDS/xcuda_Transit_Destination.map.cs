namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Transit_DestinationMap : EntityTypeConfiguration<xcuda_Transit_Destination>
    {
        public xcuda_Transit_DestinationMap()
        {                        
              this.HasKey(t => t.Destination_Id);        
              this.ToTable("xcuda_Transit_Destination");
              this.Property(t => t.Destination_Id).HasColumnName("Destination_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Office).HasColumnName("Office").HasMaxLength(10);
              this.Property(t => t.Country).HasColumnName("Country").HasMaxLength(10);
              this.Property(t => t.Transit_Id).HasColumnName("Transit_Id");
              this.HasRequired(t => t.xcuda_Transit).WithMany(t =>(ICollection<xcuda_Transit_Destination>) t.xcuda_Transit_Destination).HasForeignKey(d => d.Transit_Id);
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
