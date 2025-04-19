namespace CounterPointQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CounterPointSalesDetailsMap : EntityTypeConfiguration<CounterPointSalesDetails>
    {
        public CounterPointSalesDetailsMap()
        {                        
              this.HasKey(t => t.INVNO);        
              this.ToTable("CounterPointSalesDetails");
              this.Property(t => t.INVNO).HasColumnName("INVNO").IsRequired().IsUnicode(false).HasMaxLength(66);
              this.Property(t => t.SEQ_NO).HasColumnName("SEQ_NO");
              this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO").IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.ITEM_DESCR).HasColumnName("ITEM_DESCR").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.QUANTITY).HasColumnName("QUANTITY");
              this.Property(t => t.COST).HasColumnName("COST");
              this.Property(t => t.ACCT_NO).HasColumnName("ACCT NO").IsUnicode(false).HasMaxLength(15);
              this.Property(t => t.CUSTOMER_NAME).HasColumnName("CUSTOMER NAME").IsUnicode(false).HasMaxLength(81);
              this.Property(t => t.DATE).HasColumnName("DATE");
              this.Property(t => t.TAX_AMT).HasColumnName("TAX_AMT");
              this.Property(t => t.UNIT_WEIGHT).HasColumnName("UNIT_WEIGHT");
              this.Property(t => t.QTY_UNIT).HasColumnName("QTY_UNIT").IsUnicode(false).HasMaxLength(15);
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
