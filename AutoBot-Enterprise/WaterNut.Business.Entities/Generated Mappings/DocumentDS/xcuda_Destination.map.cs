namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_DestinationMap : EntityTypeConfiguration<xcuda_Destination>
    {
        public xcuda_DestinationMap()
        {                        
              this.HasKey(t => t.Country_Id);        
              this.ToTable("xcuda_Destination");
              this.Property(t => t.Destination_country_code).HasColumnName("Destination_country_code");
              this.Property(t => t.Destination_country_name).HasColumnName("Destination_country_name");
              this.Property(t => t.Country_Id).HasColumnName("Country_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Country).WithOptional(t => (xcuda_Destination)t.xcuda_Destination);
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
