namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ContainerAsycudaDocumentSetMap : EntityTypeConfiguration<ContainerAsycudaDocumentSet>
    {
        public ContainerAsycudaDocumentSetMap()
        {                        
              this.HasKey(t => t.ContainerAsycudaDocumentSetId);        
              this.ToTable("ContainerAsycudaDocumentSets");
              this.Property(t => t.Container_Id).HasColumnName("Container_Id");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.ContainerAsycudaDocumentSetId).HasColumnName("ContainerAsycudaDocumentSetId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.Container).WithMany(t =>(ICollection<ContainerAsycudaDocumentSet>) t.ContainerAsycudaDocumentSets).HasForeignKey(d => d.Container_Id);
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
