namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentAttachmentsMap : EntityTypeConfiguration<ShipmentAttachments>
    {
        public ShipmentAttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentAttachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.ShipmentID).HasColumnName("ShipmentID");
              this.HasRequired(t => t.Shipment).WithMany(t =>(ICollection<ShipmentAttachments>) t.ShipmentAttachments).HasForeignKey(d => d.ShipmentID);
              this.HasRequired(t => t.Attachments).WithMany(t =>(ICollection<ShipmentAttachments>) t.ShipmentAttachments).HasForeignKey(d => d.AttachmentId);
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
