﻿namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_item_deductionMap : EntityTypeConfiguration<xcuda_item_deduction>
    {
        public xcuda_item_deductionMap()
        {                        
              this.HasKey(t => t.Valuation_item_Id);        
              this.ToTable("xcuda_item_deduction");
              this.Property(t => t.Amount_national_currency).HasColumnName("Amount_national_currency");
              this.Property(t => t.Amount_foreign_currency).HasColumnName("Amount_foreign_currency");
              this.Property(t => t.Currency_name).HasColumnName("Currency_name").HasMaxLength(50);
              this.Property(t => t.Currency_rate).HasColumnName("Currency_rate");
              this.Property(t => t.Valuation_item_Id).HasColumnName("Valuation_item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Valuation_item).WithOptional(t => (xcuda_item_deduction)t.xcuda_item_deduction);
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
