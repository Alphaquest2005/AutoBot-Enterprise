namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_ASYCUDAMap : EntityTypeConfiguration<xcuda_ASYCUDA>
    {
        public xcuda_ASYCUDAMap()
        {                        
              this.HasKey(t => t.ASYCUDA_Id);        
              this.ToTable("xcuda_ASYCUDA");
              this.Property(t => t.id).HasColumnName("id").HasMaxLength(10);
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed));
              this.HasMany(t => t.xcuda_Assessment_notice).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Container).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasOptional(t => t.xcuda_Declarant).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasMany(t => t.xcuda_Export_release).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Financial).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasOptional(t => t.xcuda_General_information).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasMany(t => t.xcuda_Global_taxes).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasOptional(t => t.xcuda_Identification).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasOptional(t => t.xcuda_Property).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasMany(t => t.xcuda_Suppliers_documents).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Transit).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasMany(t => t.xcuda_Transport).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasOptional(t => t.xcuda_Valuation).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasMany(t => t.xcuda_Warehouse).WithOptional(t => t.xcuda_ASYCUDA).HasForeignKey(d => d.ASYCUDA_Id);
              this.HasOptional(t => t.xcuda_Traders).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasOptional(t => t.Ex).WithRequired(t => (xcuda_ASYCUDA) t.xcuda_ASYCUDA);
              this.HasMany(t => t.AsycudaDocumentEntryDatas).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasOptional(t => t.xcuda_ASYCUDA_ExtendedProperties).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
              this.HasMany(t => t.AsycudaDocument_Attachments).WithRequired(t => (xcuda_ASYCUDA)t.xcuda_ASYCUDA);
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
