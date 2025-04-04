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
    
    internal partial class xcuda_Transport_Mapping : EntityTypeConfiguration<xcuda_Transport>
    {
        public xcuda_Transport_Mapping()
        {                        
              this.HasKey(t => t.Transport_Id);        
              this.ToTable("xcuda_Transport");
              this.Property(t => t.Container_flag).HasColumnName("Container_flag");
              this.Property(t => t.Single_waybill_flag).HasColumnName("Single_waybill_flag");
              this.Property(t => t.Transport_Id).HasColumnName("Transport_Id");
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Location_of_goods).HasColumnName("Location_of_goods").IsUnicode(false).HasMaxLength(50);
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t => t.xcuda_Transport).HasForeignKey(d => d.ASYCUDA_Id);
         }
    }
}
