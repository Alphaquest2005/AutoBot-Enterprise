namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_RegistrationMap : EntityTypeConfiguration<xcuda_Registration>
    {
        public xcuda_RegistrationMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Registration");
              this.Property(t => t.Number).HasColumnName("Number").HasMaxLength(20);
              this.Property(t => t.Date).HasColumnName("Date");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Identification).WithOptional(t => (xcuda_Registration)t.xcuda_Registration);
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
