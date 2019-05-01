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
              this.HasKey(t => new {t.AllocationId, t.EntryDataId});        
              this.ToTable("SalesDataAllocations");
              this.Property(t => t.AllocationId).HasColumnName("AllocationId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.SalesData).WithMany(t =>(ICollection<SalesDataAllocations>) t.SalesDataAllocations).HasForeignKey(d => d.EntryDataId);
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
