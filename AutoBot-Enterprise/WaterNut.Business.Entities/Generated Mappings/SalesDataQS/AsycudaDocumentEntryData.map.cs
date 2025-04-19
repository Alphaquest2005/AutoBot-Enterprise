namespace SalesDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentEntryDataMap : EntityTypeConfiguration<AsycudaDocumentEntryData>
    {
        public AsycudaDocumentEntryDataMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaDocumentEntryData");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.AsycudaDocumentId).HasColumnName("AsycudaDocumentId");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id");
              this.HasRequired(t => t.SalesData).WithMany(t =>(ICollection<AsycudaDocumentEntryData>) t.AsycudaDocuments).HasForeignKey(d => d.EntryData_Id);
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
