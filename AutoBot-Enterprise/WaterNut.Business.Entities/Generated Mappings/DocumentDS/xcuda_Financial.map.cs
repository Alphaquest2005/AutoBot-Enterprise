namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_FinancialMap : EntityTypeConfiguration<xcuda_Financial>
    {
        public xcuda_FinancialMap()
        {                        
              this.HasKey(t => t.Financial_Id);        
              this.ToTable("xcuda_Financial");
              this.Property(t => t.Financial_Id).HasColumnName("Financial_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Deffered_payment_reference).HasColumnName("Deffered_payment_reference").HasMaxLength(100);
              this.Property(t => t.Mode_of_payment).HasColumnName("Mode_of_payment").HasMaxLength(100);
              this.Property(t => t.Financial_Code).HasColumnName("Financial_Code").HasMaxLength(100);
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Financial>) t.xcuda_Financial).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Financial_Amounts).WithRequired(t => (xcuda_Financial)t.xcuda_Financial);
              this.HasMany(t => t.xcuda_Financial_Guarantee).WithRequired(t => (xcuda_Financial)t.xcuda_Financial);
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
