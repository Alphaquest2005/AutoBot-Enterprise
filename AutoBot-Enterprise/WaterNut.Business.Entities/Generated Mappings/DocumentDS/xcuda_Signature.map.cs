namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_SignatureMap : EntityTypeConfiguration<xcuda_Signature>
    {
        public xcuda_SignatureMap()
        {                        
              this.HasKey(t => t.Signature_Id);        
              this.ToTable("xcuda_Signature");
              this.Property(t => t.Date).HasColumnName("Date");
              this.Property(t => t.Signature_Id).HasColumnName("Signature_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Transit_Id).HasColumnName("Transit_Id");
              this.HasOptional(t => t.xcuda_Transit).WithMany(t =>(ICollection<xcuda_Signature>) t.xcuda_Signature).HasForeignKey(d => d.Transit_Id);
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
