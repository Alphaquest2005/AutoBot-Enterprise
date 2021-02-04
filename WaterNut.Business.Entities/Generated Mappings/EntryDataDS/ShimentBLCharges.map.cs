namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShimentBLChargesMap : EntityTypeConfiguration<ShimentBLCharges>
    {
        public ShimentBLChargesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShimentBLCharges");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.Property(t => t.Amount).HasColumnName("Amount");
              this.Property(t => t.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(50);
              this.Property(t => t.BLId).HasColumnName("BLId");
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShimentBLCharges>) t.ShimentBLCharges).HasForeignKey(d => d.BLId);
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
