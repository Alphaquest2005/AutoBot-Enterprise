namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentItemEntryDataDetailsMap : EntityTypeConfiguration<AsycudaDocumentItemEntryDataDetails>
    {
        public AsycudaDocumentItemEntryDataDetailsMap()
        {                        
              this.HasKey(t => new {t.EntryDataDetailsId, t.Item_Id, t.ImportComplete, t.EntryData_Id, t.Asycuda_id});        
              this.ToTable("AsycudaDocumentItemEntryDataDetails");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.key).HasColumnName("key").HasMaxLength(101);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(40);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.ImportComplete).HasColumnName("ImportComplete");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.CustomsProcedure).HasColumnName("CustomsProcedure").HasMaxLength(11);
              this.Property(t => t.Asycuda_id).HasColumnName("Asycuda_id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataType).HasColumnName("EntryDataType").HasMaxLength(50);
              this.HasRequired(t => t.EntryDataDetails).WithMany(t =>(ICollection<AsycudaDocumentItemEntryDataDetails>) t.AsycudaDocumentItemEntryDataDetails).HasForeignKey(d => d.EntryDataDetailsId);
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
