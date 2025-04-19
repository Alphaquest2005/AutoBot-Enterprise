namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ASYCUDAExMap : EntityTypeConfiguration<xcuda_ASYCUDAEx>
    {
        public xcuda_ASYCUDAExMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_ASYCUDAEx");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RegistrationYear).HasColumnName("RegistrationYear");
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_ASYCUDAEx)t.Ex);
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
