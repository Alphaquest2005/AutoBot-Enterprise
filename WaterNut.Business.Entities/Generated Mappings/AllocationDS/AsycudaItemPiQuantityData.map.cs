namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaItemPiQuantityDataMap : EntityTypeConfiguration<AsycudaItemPiQuantityData>
    {
        public AsycudaItemPiQuantityDataMap()
        {                        
              this.HasKey(t => new {t.Id, t.pLineNumber});        
              this.ToTable("AsycudaItemPiQuantityData");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.PiWeight).HasColumnName("PiWeight");
              this.Property(t => t.AssessmentDate).HasColumnName("AssessmentDate");
              this.Property(t => t.IsAssessed).HasColumnName("IsAssessed");
              this.Property(t => t.xItem_Id).HasColumnName("xItem_Id");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().IsUnicode(false).HasMaxLength(9);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(30);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").HasMaxLength(20);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.pReference).HasColumnName("pReference").HasMaxLength(30);
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
