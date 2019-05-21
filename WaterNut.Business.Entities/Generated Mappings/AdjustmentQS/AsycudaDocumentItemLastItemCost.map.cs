namespace AdjustmentQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentItemLastItemCostMap : EntityTypeConfiguration<AsycudaDocumentItemLastItemCost>
    {
        public AsycudaDocumentItemLastItemCostMap()
        {                        
              this.HasKey(t => new {t.applicationsettingsid, t.ItemNumber, t.assessmentdate});        
              this.ToTable("AsycudaDocumentItemLastItemCost");
              this.Property(t => t.applicationsettingsid).HasColumnName("applicationsettingsid").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.LocalItemCost).HasColumnName("LocalItemCost");
              this.Property(t => t.assessmentdate).HasColumnName("assessmentdate");
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
