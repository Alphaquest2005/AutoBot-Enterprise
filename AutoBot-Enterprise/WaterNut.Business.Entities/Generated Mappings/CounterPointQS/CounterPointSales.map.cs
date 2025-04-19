namespace CounterPointQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CounterPointSalesMap : EntityTypeConfiguration<CounterPointSales>
    {
        public CounterPointSalesMap()
        {                        
              this.HasKey(t => t.InvoiceNo);        
              this.ToTable("CounterPointSales");
              this.Property(t => t.InvoiceNo).HasColumnName("INVNO").IsRequired().IsUnicode(false).HasMaxLength(66);
              this.Property(t => t.Date).HasColumnName("DATE");
              this.Property(t => t.TAX_AMT).HasColumnName("TAX_AMT");
              this.Property(t => t.CustomerName).HasColumnName("CUSTOMER NAME").IsUnicode(false).HasMaxLength(81);
              this.Property(t => t.LineNumber).HasColumnName("LIN_CNT");
              this.Property(t => t.Downloaded).HasColumnName("Downloaded");
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
