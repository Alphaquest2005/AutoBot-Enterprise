namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PartMap : EntityTypeConfiguration<Part>
    {
        public PartMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-PartsView");
              this.Property(t => t.Name).HasColumnName("Part").HasMaxLength(50);
              this.Property(t => t.StartMultiLine).HasColumnName("StartMultiLine");
              this.Property(t => t.Start).HasColumnName("Start");
              this.Property(t => t.EndMultiLine).HasColumnName("EndMultiLine");
              this.Property(t => t.End).HasColumnName("End");
              this.Property(t => t.IsRecuring).HasColumnName("IsRecuring");
              this.Property(t => t.IsComposite).HasColumnName("IsComposite");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.StartRegExId).HasColumnName("StartRegExId");
              this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
              this.Property(t => t.InvoiceName).HasColumnName("Invoice").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.Invoice).WithMany(t =>(ICollection<Part>) t.Parts).HasForeignKey(d => d.InvoiceId);
              this.HasMany(t => t.OCR_LinesView).WithOptional(t => t.Part).HasForeignKey(d => d.PartId);
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
