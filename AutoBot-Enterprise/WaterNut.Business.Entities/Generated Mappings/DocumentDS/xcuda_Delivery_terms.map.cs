namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Delivery_termsMap : EntityTypeConfiguration<xcuda_Delivery_terms>
    {
        public xcuda_Delivery_termsMap()
        {                        
              this.HasKey(t => t.Delivery_terms_Id);        
              this.ToTable("xcuda_Delivery_terms");
              this.Property(t => t.Delivery_terms_Id).HasColumnName("Delivery_terms_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id");
              this.Property(t => t.Code).HasColumnName("Code").HasMaxLength(50);
              this.Property(t => t.Place).HasColumnName("Place").HasMaxLength(50);
              this.Property(t => t.Situation).HasColumnName("Situation").HasMaxLength(50);
              this.HasOptional(t => t.xcuda_Transport).WithMany(t =>(ICollection<xcuda_Delivery_terms>) t.xcuda_Delivery_terms).HasForeignKey(d => d.Transport_Id);
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
