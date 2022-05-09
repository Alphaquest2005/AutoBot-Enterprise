namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaItemBasicInfoMap : EntityTypeConfiguration<AsycudaItemBasicInfo>
    {
        public AsycudaItemBasicInfoMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("AsycudaItemBasicInfo");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.ItemQuantity).HasColumnName("ItemQuantity");
              this.Property(t => t.DPQtyAllocated).HasColumnName("DPQtyAllocated");
              this.Property(t => t.DFQtyAllocated).HasColumnName("DFQtyAllocated");
              this.Property(t => t.IsAssessed).HasColumnName("IsAssessed");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.Commercial_Description).HasColumnName("Commercial_Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(20);
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
