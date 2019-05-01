namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OpeningStockMap : EntityTypeConfiguration<OpeningStock>
    {
        public OpeningStockMap()
        {                        
              this.HasKey(t => t.EntryDataId);        
              this.ToTable("EntryData_OpeningStock");
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.OPSNumber).HasColumnName("OPSNumber").IsRequired().HasMaxLength(50);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
