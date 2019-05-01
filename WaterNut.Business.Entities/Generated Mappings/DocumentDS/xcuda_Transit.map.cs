namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TransitMap : EntityTypeConfiguration<xcuda_Transit>
    {
        public xcuda_TransitMap()
        {                        
              this.HasKey(t => t.Transit_Id);        
              this.ToTable("xcuda_Transit");
              this.Property(t => t.Time_limit).HasColumnName("Time_limit").HasMaxLength(10);
              this.Property(t => t.Transit_Id).HasColumnName("Transit_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Transit>) t.xcuda_Transit).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Principal).WithOptional(t => t.xcuda_Transit).HasForeignKey(d => d.Transit_Id);
              this.HasMany(t => t.xcuda_Seals).WithOptional(t => t.xcuda_Transit).HasForeignKey(d => d.Transit_Id);
              this.HasMany(t => t.xcuda_Signature).WithOptional(t => t.xcuda_Transit).HasForeignKey(d => d.Transit_Id);
              this.HasMany(t => t.xcuda_Transit_Destination).WithRequired(t => (xcuda_Transit)t.xcuda_Transit);
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
