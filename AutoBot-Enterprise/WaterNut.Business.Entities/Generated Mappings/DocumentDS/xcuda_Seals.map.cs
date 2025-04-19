namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_SealsMap : EntityTypeConfiguration<xcuda_Seals>
    {
        public xcuda_SealsMap()
        {                        
              this.HasKey(t => t.Seals_Id);        
              this.ToTable("xcuda_Seals");
              this.Property(t => t.Number).HasColumnName("Number");
              this.Property(t => t.Seals_Id).HasColumnName("Seals_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Transit_Id).HasColumnName("Transit_Id");
              this.HasOptional(t => t.xcuda_Transit).WithMany(t =>(ICollection<xcuda_Seals>) t.xcuda_Seals).HasForeignKey(d => d.Transit_Id);
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
