﻿namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_TypeMap : EntityTypeConfiguration<xcuda_Type>
    {
        public xcuda_TypeMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_Type");
              this.Property(t => t.Type_of_declaration).HasColumnName("Type_of_declaration").HasMaxLength(20);
              this.Property(t => t.Declaration_gen_procedure_code).HasColumnName("Declaration_gen_procedure_code").HasMaxLength(20);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Identification).WithOptional(t => (xcuda_Type)t.xcuda_Type);
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
