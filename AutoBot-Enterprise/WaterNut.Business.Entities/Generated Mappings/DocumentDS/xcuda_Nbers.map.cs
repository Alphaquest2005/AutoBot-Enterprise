namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_NbersMap : EntityTypeConfiguration<xcuda_Nbers>
    {
        public xcuda_NbersMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Nbers");
              this.Property(t => t.Number_of_loading_lists).HasColumnName("Number_of_loading_lists").HasMaxLength(20);
              this.Property(t => t.Total_number_of_items).HasColumnName("Total_number_of_items").HasMaxLength(20);
              this.Property(t => t.Total_number_of_packages).HasColumnName("Total_number_of_packages");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Property).WithOptional(t => (xcuda_Nbers)t.xcuda_Nbers);
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
