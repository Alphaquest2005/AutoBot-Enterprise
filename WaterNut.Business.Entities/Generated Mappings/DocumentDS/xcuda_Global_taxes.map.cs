namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Global_taxesMap : EntityTypeConfiguration<xcuda_Global_taxes>
    {
        public xcuda_Global_taxesMap()
        {                        
              this.HasKey(t => t.Global_taxes_Id);        
              this.ToTable("xcuda_Global_taxes");
              this.Property(t => t.Global_taxes_Id).HasColumnName("Global_taxes_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Global_taxes>) t.xcuda_Global_taxes).HasForeignKey(d => d.ASYCUDA_Id);
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
