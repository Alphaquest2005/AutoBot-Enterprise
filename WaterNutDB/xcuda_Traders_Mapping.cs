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
    
    internal partial class xcuda_Traders_Mapping : EntityTypeConfiguration<xcuda_Traders>
    {
        public xcuda_Traders_Mapping()
        {                        
              this.HasKey(t => t.Traders_Id);        
              this.ToTable("xcuda_Traders");
              this.Property(t => t.Traders_Id).HasColumnName("Traders_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_ASYCUDA).WithOptional(t => t.xcuda_Traders);
         }
    }
}
