namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Assessment_noticeMap : EntityTypeConfiguration<xcuda_Assessment_notice>
    {
        public xcuda_Assessment_noticeMap()
        {                        
              this.HasKey(t => t.Assessment_notice_Id);        
              this.ToTable("xcuda_Assessment_notice");
              this.Property(t => t.Assessment_notice_Id).HasColumnName("Assessment_notice_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.HasOptional(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<xcuda_Assessment_notice>) t.xcuda_Assessment_notice).HasForeignKey(d => d.ASYCUDA_Id);
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
