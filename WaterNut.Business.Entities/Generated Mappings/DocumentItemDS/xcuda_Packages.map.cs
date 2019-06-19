namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_PackagesMap : EntityTypeConfiguration<xcuda_Packages>
    {
        public xcuda_PackagesMap()
        {                        
              this.HasKey(t => t.Packages_Id);        
              this.ToTable("xcuda_Packages");
              this.Property(t => t.Number_of_packages).HasColumnName("Number_of_packages");
              this.Property(t => t.Kind_of_packages_code).HasColumnName("Kind_of_packages_code").HasMaxLength(20);
              this.Property(t => t.Kind_of_packages_name).HasColumnName("Kind_of_packages_name").HasMaxLength(20);
              this.Property(t => t.Packages_Id).HasColumnName("Packages_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.Marks1_of_packages).HasColumnName("Marks1_of_packages").HasMaxLength(40);
              this.Property(t => t.Marks2_of_packages).HasColumnName("Marks2_of_packages").HasMaxLength(40);
              this.HasOptional(t => t.xcuda_Item).WithMany(t =>(ICollection<xcuda_Packages>) t.xcuda_Packages).HasForeignKey(d => d.Item_Id);
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
