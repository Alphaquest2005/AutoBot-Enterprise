namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetEntryDataExMap : EntityTypeConfiguration<AsycudaDocumentSetEntryDataEx>
    {
        public AsycudaDocumentSetEntryDataExMap()
        {                        
              this.HasKey(t => new {t.AsycudaDocumentSetId, t.EntryData_Id, t.EntryDataId, t.EntryDataDate});        
              this.ToTable("AsycudaDocumentSetEntryDataEx");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataId).HasColumnName("EntryDataId").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntryDataDate).HasColumnName("EntryDataDate");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile");
              this.HasRequired(t => t.AsycudaDocumentSetEx).WithMany(t =>(ICollection<AsycudaDocumentSetEntryDataEx>) t.AsycudaDocumentSetEntryDataEx).HasForeignKey(d => d.AsycudaDocumentSetId);
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
