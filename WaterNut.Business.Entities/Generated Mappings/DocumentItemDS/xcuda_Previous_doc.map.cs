namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Previous_docMap : EntityTypeConfiguration<xcuda_Previous_doc>
    {
        public xcuda_Previous_docMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Previous_doc");
              this.Property(t => t.Summary_declaration).HasColumnName("Summary_declaration").HasMaxLength(255);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Previous_document_reference).HasColumnName("Previous_document_reference").HasMaxLength(50);
              this.Property(t => t.Previous_warehouse_code).HasColumnName("Previous_warehouse_code").HasMaxLength(50);
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => (xcuda_Previous_doc)t.xcuda_Previous_doc);
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
