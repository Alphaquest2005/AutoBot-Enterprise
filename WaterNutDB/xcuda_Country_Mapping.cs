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
    
    internal partial class xcuda_Country_Mapping : EntityTypeConfiguration<xcuda_Country>
    {
        public xcuda_Country_Mapping()
        {                        
              this.HasKey(t => t.Country_Id);        
              this.ToTable("xcuda_Country");
              this.Property(t => t.Country_first_destination).HasColumnName("Country_first_destination").IsUnicode(false);
              this.Property(t => t.Country_of_origin_name).HasColumnName("Country_of_origin_name").IsUnicode(false);
              this.Property(t => t.Country_Id).HasColumnName("Country_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Place_of_loading_Id).HasColumnName("Place_of_loading_Id");
              this.Property(t => t.Trading_country).HasColumnName("Trading_country").IsUnicode(false).HasMaxLength(50);
              this.HasRequired(t => t.xcuda_General_information).WithOptional(t => t.xcuda_Country);
         }
    }
}
