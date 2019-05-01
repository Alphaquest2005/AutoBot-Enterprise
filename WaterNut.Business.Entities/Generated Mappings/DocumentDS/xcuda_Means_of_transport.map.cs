namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Means_of_transportMap : EntityTypeConfiguration<xcuda_Means_of_transport>
    {
        public xcuda_Means_of_transportMap()
        {                        
              this.HasKey(t => t.Means_of_transport_Id);        
              this.ToTable("xcuda_Means_of_transport");
              this.Property(t => t.Means_of_transport_Id).HasColumnName("Means_of_transport_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id");
              this.Property(t => t.Inland_mode_of_transport).HasColumnName("Inland_mode_of_transport").HasMaxLength(50);
              this.HasOptional(t => t.xcuda_Transport).WithMany(t =>(ICollection<xcuda_Means_of_transport>) t.xcuda_Means_of_transport).HasForeignKey(d => d.Transport_Id);
              this.HasMany(t => t.xcuda_Border_information).WithOptional(t => t.xcuda_Means_of_transport).HasForeignKey(d => d.Means_of_transport_Id);
              this.HasMany(t => t.xcuda_Departure_arrival_information).WithOptional(t => t.xcuda_Means_of_transport).HasForeignKey(d => d.Means_of_transport_Id);
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
