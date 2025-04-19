namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Border_officeMap : EntityTypeConfiguration<xcuda_Border_office>
    {
        public xcuda_Border_officeMap()
        {                        
              this.HasKey(t => t.Border_office_Id);        
              this.ToTable("xcuda_Border_office");
              this.Property(t => t.Border_office_Id).HasColumnName("Border_office_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id");
              this.Property(t => t.Code).HasColumnName("Code").HasMaxLength(10);
              this.Property(t => t.Name).HasColumnName("Name").HasMaxLength(100);
              this.HasOptional(t => t.xcuda_Transport).WithMany(t =>(ICollection<xcuda_Border_office>) t.xcuda_Border_office).HasForeignKey(d => d.Transport_Id);
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
