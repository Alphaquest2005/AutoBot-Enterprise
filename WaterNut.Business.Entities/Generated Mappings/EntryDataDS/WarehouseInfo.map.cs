namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class WarehouseInfoMap : EntityTypeConfiguration<WarehouseInfo>
    {
        public WarehouseInfoMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("WarehouseInfo");
              this.Property(t => t.WarehouseNo).HasColumnName("WarehouseNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.HasRequired(t => t.EntryData_PurchaseOrders).WithMany(t =>(ICollection<WarehouseInfo>) t.WarehouseInfo).HasForeignKey(d => d.EntryData_Id);
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
