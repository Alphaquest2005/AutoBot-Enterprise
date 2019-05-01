namespace EntryDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetEntryDataDetailMap : EntityTypeConfiguration<AsycudaDocumentSetEntryDataDetail>
    {
        public AsycudaDocumentSetEntryDataDetailMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaDocumentSetEntryDataDetails");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.EntryDataDetailsEx).WithMany(t =>(ICollection<AsycudaDocumentSetEntryDataDetail>) t.AsycudaDocumentSets).HasForeignKey(d => d.EntryDataDetailsId);
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
