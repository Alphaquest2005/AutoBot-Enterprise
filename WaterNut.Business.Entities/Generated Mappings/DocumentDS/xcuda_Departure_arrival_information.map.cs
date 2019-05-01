namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Departure_arrival_informationMap : EntityTypeConfiguration<xcuda_Departure_arrival_information>
    {
        public xcuda_Departure_arrival_informationMap()
        {                        
              this.HasKey(t => t.Departure_arrival_information_Id);        
              this.ToTable("xcuda_Departure_arrival_information");
              this.Property(t => t.Departure_arrival_information_Id).HasColumnName("Departure_arrival_information_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Means_of_transport_Id).HasColumnName("Means_of_transport_Id");
              this.Property(t => t.Identity).HasColumnName("Identity").HasMaxLength(50);
              this.Property(t => t.Nationality).HasColumnName("Nationality").HasMaxLength(50);
              this.HasOptional(t => t.xcuda_Means_of_transport).WithMany(t =>(ICollection<xcuda_Departure_arrival_information>) t.xcuda_Departure_arrival_information).HasForeignKey(d => d.Means_of_transport_Id);
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
