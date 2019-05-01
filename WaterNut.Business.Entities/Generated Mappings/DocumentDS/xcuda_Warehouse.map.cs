namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_WarehouseMap : EntityTypeConfiguration<xcuda_Warehouse>
    {
        public xcuda_WarehouseMap()
        {                        
              this.HasKey(t => t.Warehouse_Id);        
              this.ToTable("xcuda_Warehouse");
              this.Property(t => t.Identification).HasColumnName("Identification").HasMaxLength(20);
              this.Property(t => t.Delay).HasColumnName("Delay").HasMaxLength(4);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Warehouse_Id).HasColumnName("Warehouse_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Warehouse>) t.xcuda_Warehouse).HasForeignKey(d => d.ASYCUDA_Id);
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
