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
    
    internal partial class xcuda_Previous_doc_Mapping : EntityTypeConfiguration<xcuda_Previous_doc>
    {
        public xcuda_Previous_doc_Mapping()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Previous_doc");
              this.Property(t => t.Summary_declaration).HasColumnName("Summary_declaration").IsUnicode(false);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => t.xcuda_Previous_doc);
         }
    }
}
