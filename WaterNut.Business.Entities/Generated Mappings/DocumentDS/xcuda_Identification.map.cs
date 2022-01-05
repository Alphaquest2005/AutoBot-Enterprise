namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_IdentificationMap : EntityTypeConfiguration<xcuda_Identification>
    {
        public xcuda_IdentificationMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Identification");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Manifest_reference_number).HasColumnName("Manifest_reference_number").HasMaxLength(50);
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_Identification)t.xcuda_Identification);
              this.HasOptional(t => t.xcuda_Assessment).WithRequired(t => (xcuda_Identification) t.xcuda_Identification);
              this.HasOptional(t => t.xcuda_Office_segment).WithRequired(t => (xcuda_Identification) t.xcuda_Identification);
              this.HasOptional(t => t.xcuda_receipt).WithRequired(t => (xcuda_Identification) t.xcuda_Identification);
              this.HasOptional(t => t.xcuda_Registration).WithRequired(t => (xcuda_Identification) t.xcuda_Identification);
              this.HasOptional(t => t.xcuda_Type).WithRequired(t => (xcuda_Identification) t.xcuda_Identification);
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
