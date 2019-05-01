namespace EntryDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ContainerTypeMap : EntityTypeConfiguration<ContainerType>
    {
        public ContainerTypeMap()
        {                        
              this.HasKey(t => t.ContainerCode);        
              this.ToTable("ContainerTypes");
              this.Property(t => t.ContainerCode).HasColumnName("ContainerCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.ContainerTypeDescription).HasColumnName("ContainerTypeDescription").HasMaxLength(50);
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
