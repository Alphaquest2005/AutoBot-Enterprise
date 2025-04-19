namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class Customs_ProcedureInOutMap : EntityTypeConfiguration<Customs_ProcedureInOut>
    {
        public Customs_ProcedureInOutMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("Customs_ProcedureInOut");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.WarehouseCustomsProcedureId).HasColumnName("WarehouseCustomsProcedureId");
              this.Property(t => t.ExwarehouseCustomsProcedureId).HasColumnName("ExwarehouseCustomsProcedureId");
              this.HasRequired(t => t.InCustomsProcedure).WithMany(t =>(ICollection<Customs_ProcedureInOut>) t.InCustomsProcedure).HasForeignKey(d => d.WarehouseCustomsProcedureId);
              this.HasRequired(t => t.OutCustomsProcedure).WithMany(t =>(ICollection<Customs_ProcedureInOut>) t.OutCustomsProcedure).HasForeignKey(d => d.ExwarehouseCustomsProcedureId);
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
