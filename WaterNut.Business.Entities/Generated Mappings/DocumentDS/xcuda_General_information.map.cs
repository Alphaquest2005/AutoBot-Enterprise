namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_General_informationMap : EntityTypeConfiguration<xcuda_General_information>
    {
        public xcuda_General_informationMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_General_information");
              this.Property(t => t.Value_details).HasColumnName("Value_details").HasMaxLength(20);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.CAP).HasColumnName("CAP").HasMaxLength(20);
              this.Property(t => t.Additional_information).HasColumnName("Additional_information").HasMaxLength(20);
              this.Property(t => t.Comments_free_text).HasColumnName("Comments_free_text").HasMaxLength(255);
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_General_information)t.xcuda_General_information);
              this.HasOptional(t => t.xcuda_Country).WithRequired(t => (xcuda_General_information)t.xcuda_General_information);
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
