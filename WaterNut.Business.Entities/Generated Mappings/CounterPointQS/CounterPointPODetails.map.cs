namespace CounterPointQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CounterPointPODetailsMap : EntityTypeConfiguration<CounterPointPODetails>
    {
        public CounterPointPODetailsMap()
        {                        
              this.HasKey(t => t.PO_NO);        
              this.ToTable("CounterPointPODetails");
              this.Property(t => t.PO_NO).HasColumnName("PO_NO").IsRequired().IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.SEQ_NO).HasColumnName("SEQ_NO");
              this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO").IsRequired().IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.ORD_QTY).HasColumnName("ORD_QTY");
              this.Property(t => t.ORD_UNIT).HasColumnName("ORD_UNIT").IsUnicode(false).HasMaxLength(15);
              this.Property(t => t.ITEM_DESCR).HasColumnName("ITEM_DESCR").IsRequired().IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.ORD_COST).HasColumnName("ORD_COST");
              this.Property(t => t.UNIT_WEIGHT).HasColumnName("UNIT_WEIGHT");
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
