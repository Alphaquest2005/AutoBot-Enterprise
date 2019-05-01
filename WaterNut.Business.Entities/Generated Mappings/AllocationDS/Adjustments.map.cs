namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AdjustmentsMap : EntityTypeConfiguration<Adjustments>
    {
        public AdjustmentsMap()
        {                        
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("EntryData_Adjustments");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (Adjustments)t.Adjustments);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
