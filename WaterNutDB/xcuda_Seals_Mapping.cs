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
    
    internal partial class xcuda_Seals_Mapping : EntityTypeConfiguration<xcuda_Seals>
    {
        public xcuda_Seals_Mapping()
        {                        
              this.HasKey(t => t.Seals_Id);        
              this.ToTable("xcuda_Seals");
              this.Property(t => t.Number).HasColumnName("Number").IsUnicode(false);
              this.Property(t => t.Seals_Id).HasColumnName("Seals_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Transit_Id).HasColumnName("Transit_Id");
              this.HasOptional(t => t.xcuda_Transit).WithMany(t => t.xcuda_Seals).HasForeignKey(d => d.Transit_Id);
         }
    }
}
