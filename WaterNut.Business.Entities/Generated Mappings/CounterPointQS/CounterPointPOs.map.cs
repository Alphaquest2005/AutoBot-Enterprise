namespace CounterPointQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CounterPointPOsMap : EntityTypeConfiguration<CounterPointPOs>
    {
        public CounterPointPOsMap()
        {                        
              this.HasKey(t => t.PurchaseOrderNo);        
              this.ToTable("CounterPointPOs");
              this.Property(t => t.PurchaseOrderNo).HasColumnName("PO_NO").IsRequired().IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.Date).HasColumnName("DATE");
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
