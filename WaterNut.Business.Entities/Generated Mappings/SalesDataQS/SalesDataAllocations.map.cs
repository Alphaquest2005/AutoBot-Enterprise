namespace SalesDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SalesDataAllocationsMap : EntityTypeConfiguration<SalesDataAllocations>
    {
        public SalesDataAllocationsMap()
        {                        
              this.HasKey(t => new {t.AllocationId, t.EntryData_Id});        
              this.ToTable("SalesDataAllocations");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.SalesData).WithMany(t =>(ICollection<SalesDataAllocations>) t.SalesDataAllocations).HasForeignKey(d => d.EntryData_Id);
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
