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
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryData_OpeningStock");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.OPSNumber).HasColumnName("OPSNumber").IsRequired().HasMaxLength(50);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
