namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetC71Map : EntityTypeConfiguration<AsycudaDocumentSetC71>
    {
        public AsycudaDocumentSetC71Map()
        {                        
              this.HasKey(t => t.Value_declaration_form_Id);        
              this.ToTable("AsycudaDocumentSetC71");
              this.Property(t => t.Address).HasColumnName("Address").HasMaxLength(255);
              this.Property(t => t.Value_declaration_form_Id).HasColumnName("Value_declaration_form_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Total).HasColumnName("Total");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.FilePath).HasColumnName("FilePath").IsRequired().HasMaxLength(255);
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.RegNumber).HasColumnName("RegNumber").IsRequired().HasMaxLength(50);
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
