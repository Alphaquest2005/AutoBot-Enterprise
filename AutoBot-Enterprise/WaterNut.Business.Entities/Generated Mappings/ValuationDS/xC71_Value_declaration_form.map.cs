namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xC71_Value_declaration_formMap : EntityTypeConfiguration<xC71_Value_declaration_form>
    {
        public xC71_Value_declaration_formMap()
        {                        
              this.HasKey(t => t.Value_declaration_form_Id);        
              this.ToTable("xC71_Value_declaration_form");
              this.Property(t => t.Value_declaration_form_Id).HasColumnName("Value_declaration_form_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasOptional(t => t.xC71_Identification_segment).WithRequired(t => (xC71_Value_declaration_form)t.xC71_Value_declaration_form);
              this.HasMany(t => t.xC71_Item).WithOptional(t => t.xC71_Value_declaration_form).HasForeignKey(d => d.Value_declaration_form_Id);
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
