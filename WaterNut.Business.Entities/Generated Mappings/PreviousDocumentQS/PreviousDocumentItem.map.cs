namespace PreviousDocumentQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PreviousDocumentItemMap : EntityTypeConfiguration<PreviousDocumentItem>
    {
        public PreviousDocumentItemMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("PreviousDocumentItem");
              this.Property(t => t.Amount_deducted_from_licence).HasColumnName("Amount_deducted_from_licence").HasMaxLength(10);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Licence_number).HasColumnName("Licence_number").HasMaxLength(6);
              this.Property(t => t.Free_text_1).HasColumnName("Free_text_1").HasMaxLength(30);
              this.Property(t => t.Free_text_2).HasColumnName("Free_text_2").HasMaxLength(30);
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.IsAssessed).HasColumnName("IsAssessed");
              this.Property(t => t.DPQtyAllocated).HasColumnName("DPQtyAllocated");
              this.Property(t => t.DFQtyAllocated).HasColumnName("DFQtyAllocated");
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.AttributeOnlyAllocation).HasColumnName("AttributeOnlyAllocation");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.DoNotEX).HasColumnName("DoNotEX");
              this.Property(t => t.Item_price).HasColumnName("Item_price");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(20);
              this.Property(t => t.DutyLiability).HasColumnName("DutyLiability");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.Statistical_value).HasColumnName("Statistical_value");
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.Description_of_goods).HasColumnName("Description_of_goods").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Commercial_Description).HasColumnName("Commercial_Description").IsUnicode(false).HasMaxLength(255);
              this.Property(t => t.Suppplementary_unit_code).HasColumnName("Suppplementary_unit_code").HasMaxLength(4);
              this.Property(t => t.ItemQuantity).HasColumnName("ItemQuantity");
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.CNumber).HasColumnName("CNumber").HasMaxLength(20);
              this.Property(t => t.AssessmentDate).HasColumnName("AssessmentDate");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasRequired(t => t.PreviousDocument).WithMany(t =>(ICollection<PreviousDocumentItem>) t.PreviousDocumentItems).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.PreviousItemsExes).WithOptional(t => t.PreviousDocumentItem).HasForeignKey(d => d.PreviousDocumentItemId);
              this.HasMany(t => t.PreviousItemEx).WithRequired(t => (PreviousDocumentItem)t.AsycudaDocumentItem);
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
