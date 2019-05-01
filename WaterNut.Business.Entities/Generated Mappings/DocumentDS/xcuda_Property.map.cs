namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_PropertyMap : EntityTypeConfiguration<xcuda_Property>
    {
        public xcuda_PropertyMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Property");
              this.Property(t => t.Sad_flow).HasColumnName("Sad_flow").HasMaxLength(20);
              this.Property(t => t.Date_of_declaration).HasColumnName("Date_of_declaration").HasMaxLength(20);
              this.Property(t => t.Selected_page).HasColumnName("Selected_page").HasMaxLength(20);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Place_of_declaration).HasColumnName("Place_of_declaration").HasMaxLength(20);
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => (xcuda_Property)t.xcuda_Property);
              this.HasOptional(t => t.xcuda_Forms).WithRequired(t => (xcuda_Property)t.xcuda_Property);
              this.HasOptional(t => t.xcuda_Nbers).WithRequired(t => (xcuda_Property)t.xcuda_Property);
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
