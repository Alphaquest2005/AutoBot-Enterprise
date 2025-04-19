namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SalesMap : EntityTypeConfiguration<Sales>
    {
        public SalesMap()
        {                        
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryData_Sales");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.INVNumber).HasColumnName("INVNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.CustomerName).HasColumnName("CustomerName").HasMaxLength(255);
              this.Property(t => t.Tax).HasColumnName("Tax");
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (Sales)t.Sales);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
