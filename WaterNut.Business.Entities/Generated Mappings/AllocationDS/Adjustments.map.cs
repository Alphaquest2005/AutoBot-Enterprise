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
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryData_Adjustments");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.Tax).HasColumnName("Tax");
              this.Property(t => t.Vendor).HasColumnName("Vendor").HasMaxLength(50);
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (Adjustments)t.Adjustments);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
