namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TransportMap : EntityTypeConfiguration<xcuda_Transport>
    {
        public xcuda_TransportMap()
        {                        
              this.HasKey(t => t.Transport_Id);        
              this.ToTable("xcuda_Transport");
              this.Property(t => t.Container_flag).HasColumnName("Container_flag");
              this.Property(t => t.Single_waybill_flag).HasColumnName("Single_waybill_flag");
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Location_of_goods).HasColumnName("Location_of_goods").HasMaxLength(50);
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Transport>) t.xcuda_Transport).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Border_office).WithOptional(t => t.xcuda_Transport).HasForeignKey(d => d.Transport_Id);
              this.HasMany(t => t.xcuda_Delivery_terms).WithOptional(t => t.xcuda_Transport).HasForeignKey(d => d.Transport_Id);
              this.HasMany(t => t.xcuda_Means_of_transport).WithOptional(t => t.xcuda_Transport).HasForeignKey(d => d.Transport_Id);
              this.HasMany(t => t.xcuda_Place_of_loading).WithOptional(t => t.xcuda_Transport).HasForeignKey(d => d.Transport_Id);
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
