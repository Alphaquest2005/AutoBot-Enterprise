namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xSalesDetailsMap : EntityTypeConfiguration<xSalesDetails>
    {
        public xSalesDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("xSalesDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.xSalesFileId).HasColumnName("xSalesFileId");
              this.Property(t => t.FileLine).HasColumnName("FileLine");
              this.Property(t => t.Line).HasColumnName("Line");
              this.Property(t => t.Date).HasColumnName("Date");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.CustomerName).HasColumnName("CustomerName").HasMaxLength(50);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.ItemDescription).HasColumnName("ItemDescription").HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.SalesQuantity).HasColumnName("SalesQuantity");
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.xQuantity).HasColumnName("xQuantity");
              this.Property(t => t.Price).HasColumnName("Price");
              this.Property(t => t.DutyFreePaid).HasColumnName("DutyFreePaid").IsRequired().HasMaxLength(50);
              this.Property(t => t.pCNumber).HasColumnName("pCNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.pLineNumber).HasColumnName("pLineNumber");
              this.Property(t => t.pRegDate).HasColumnName("pRegDate");
              this.Property(t => t.CIFValue).HasColumnName("CIFValue");
              this.Property(t => t.DutyLiablity).HasColumnName("DutyLiablity");
              this.Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(50);
              this.HasRequired(t => t.xSalesFiles).WithMany(t =>(ICollection<xSalesDetails>) t.xSalesDetails).HasForeignKey(d => d.xSalesFileId);
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
