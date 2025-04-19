namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Financial_GuaranteeMap : EntityTypeConfiguration<xcuda_Financial_Guarantee>
    {
        public xcuda_Financial_GuaranteeMap()
        {                        
              this.HasKey(t => t.Guarantee_Id);        
              this.ToTable("xcuda_Financial_Guarantee");
              this.Property(t => t.Guarantee_Id).HasColumnName("Guarantee_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Financial_Id).HasColumnName("Financial_Id");
              this.Property(t => t.Amount).HasColumnName("Amount");
              this.Property(t => t.Date).HasColumnName("Date");
              this.HasRequired(t => t.xcuda_Financial).WithMany(t =>(ICollection<xcuda_Financial_Guarantee>) t.xcuda_Financial_Guarantee).HasForeignKey(d => d.Financial_Id);
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
