//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WaterNutDB
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Infrastructure;
    
    internal partial class xcuda_Packages_Mapping : EntityTypeConfiguration<xcuda_Packages>
    {
        public xcuda_Packages_Mapping()
        {                        
              this.HasKey(t => t.Packages_Id);        
              this.ToTable("xcuda_Packages");
              this.Property(t => t.Number_of_packages).HasColumnName("Number_of_packages");
              this.Property(t => t.Kind_of_packages_code).HasColumnName("Kind_of_packages_code").IsUnicode(false);
              this.Property(t => t.Kind_of_packages_name).HasColumnName("Kind_of_packages_name").IsUnicode(false);
              this.Property(t => t.Packages_Id).HasColumnName("Packages_Id");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id");
              this.Property(t => t.Marks1_of_packages).HasColumnName("Marks1_of_packages");
              this.Property(t => t.Marks2_of_packages).HasColumnName("Marks2_of_packages");
              this.HasOptional(t => t.xcuda_Item).WithMany(t => t.xcuda_Packages).HasForeignKey(d => d.Item_Id);
         }
    }
}
