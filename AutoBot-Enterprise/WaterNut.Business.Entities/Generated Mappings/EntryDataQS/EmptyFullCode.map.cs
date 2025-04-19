namespace EntryDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmptyFullCodeMap : EntityTypeConfiguration<EmptyFullCode>
    {
        public EmptyFullCodeMap()
        {                        
              this.HasKey(t => t.EmptyFullCodeId);        
              this.ToTable("EmptyFullCodes");
              this.Property(t => t.EmptyFullCodeName).HasColumnName("EmptyFullCode").HasMaxLength(20);
              this.Property(t => t.EmptyFullDescription).HasColumnName("EmptyFullDescription").HasMaxLength(100);
              this.Property(t => t.EmptyFullCodeId).HasColumnName("EmptyFullCodeId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
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
