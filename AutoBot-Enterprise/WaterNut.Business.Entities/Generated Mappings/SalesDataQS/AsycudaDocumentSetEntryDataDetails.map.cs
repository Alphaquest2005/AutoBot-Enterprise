namespace SalesDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetEntryDataDetailsMap : EntityTypeConfiguration<AsycudaDocumentSetEntryDataDetails>
    {
        public AsycudaDocumentSetEntryDataDetailsMap()
        {                        
              this.HasKey(t => new {t.AsycudaDocumentSetId, t.EntryDataDetailsId});        
              this.ToTable("AsycudaDocumentSetEntryDataDetails");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Id).HasColumnName("Id");
              this.HasRequired(t => t.SalesDataDetail).WithMany(t =>(ICollection<AsycudaDocumentSetEntryDataDetails>) t.AsycudaDocumentSets).HasForeignKey(d => d.EntryDataDetailsId);
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
