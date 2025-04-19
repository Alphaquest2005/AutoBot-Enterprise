namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Financial_AmountsMap : EntityTypeConfiguration<xcuda_Financial_Amounts>
    {
        public xcuda_Financial_AmountsMap()
        {                        
              this.HasKey(t => t.Amounts_Id);        
              this.ToTable("xcuda_Financial_Amounts");
              this.Property(t => t.Amounts_Id).HasColumnName("Amounts_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Financial_Id).HasColumnName("Financial_Id");
              this.Property(t => t.Total_manual_taxes).HasColumnName("Total_manual_taxes");
              this.Property(t => t.Global_taxes).HasColumnName("Global_taxes");
              this.Property(t => t.Totals_taxes).HasColumnName("Totals_taxes");
              this.HasRequired(t => t.xcuda_Financial).WithMany(t =>(ICollection<xcuda_Financial_Amounts>) t.xcuda_Financial_Amounts).HasForeignKey(d => d.Financial_Id);
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
