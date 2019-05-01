namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Supplementary_unitMap : EntityTypeConfiguration<xcuda_Supplementary_unit>
    {
        public xcuda_Supplementary_unitMap()
        {                        
              this.HasKey(t => t.Supplementary_unit_Id);        
              this.ToTable("xcuda_Supplementary_unit");
              this.Property(t => t.Suppplementary_unit_quantity).HasColumnName("Suppplementary_unit_quantity");
              this.Property(t => t.Supplementary_unit_Id).HasColumnName("Supplementary_unit_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Tarification_Id).HasColumnName("Tarification_Id");
              this.Property(t => t.Suppplementary_unit_code).HasColumnName("Suppplementary_unit_code").HasMaxLength(4);
              this.Property(t => t.Suppplementary_unit_name).HasColumnName("Suppplementary_unit_name").HasMaxLength(255);
              this.Property(t => t.IsFirstRow).HasColumnName("IsFirstRow");
              this.HasRequired(t => t.xcuda_Tarification).WithMany(t =>(ICollection<xcuda_Supplementary_unit>) t.Unordered_xcuda_Supplementary_unit).HasForeignKey(d => d.Tarification_Id);
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
