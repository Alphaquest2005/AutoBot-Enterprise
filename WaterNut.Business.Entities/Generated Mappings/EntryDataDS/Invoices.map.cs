namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InvoicesMap : EntityTypeConfiguration<Invoices>
    {
        public InvoicesMap()
        {                        
              this.HasKey(t => t.EntryData_Id);        
              this.ToTable("EntryData_Invoices");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.PONumber).HasColumnName("PONumber").HasMaxLength(50);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
