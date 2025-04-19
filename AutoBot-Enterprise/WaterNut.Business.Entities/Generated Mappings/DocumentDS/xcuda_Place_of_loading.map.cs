namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Place_of_loadingMap : EntityTypeConfiguration<xcuda_Place_of_loading>
    {
        public xcuda_Place_of_loadingMap()
        {                        
              this.HasKey(t => t.Place_of_loading_Id);        
              this.ToTable("xcuda_Place_of_loading");
              this.Property(t => t.Place_of_loading_Id).HasColumnName("Place_of_loading_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id");
              this.HasOptional(t => t.xcuda_Transport).WithMany(t =>(ICollection<xcuda_Place_of_loading>) t.xcuda_Place_of_loading).HasForeignKey(d => d.Transport_Id);
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
