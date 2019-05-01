namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_DeclarantMap : EntityTypeConfiguration<xcuda_Declarant>
    {
        public xcuda_DeclarantMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Declarant");
              this.Property(t => t.Declarant_code).HasColumnName("Declarant_code").HasMaxLength(20);
              this.Property(t => t.Declarant_name).HasColumnName("Declarant_name").HasMaxLength(255);
              this.Property(t => t.Declarant_representative).HasColumnName("Declarant_representative").HasMaxLength(255);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Number).HasColumnName("Number").HasMaxLength(30);
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_Declarant)t.xcuda_Declarant);
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
