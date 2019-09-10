namespace ValuationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xC71_ItemMap : EntityTypeConfiguration<xC71_Item>
    {
        public xC71_ItemMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xC71_Item");
              this.Property(t => t.Terms_of_Delivery_Code).HasColumnName("Terms_of_Delivery_Code").HasMaxLength(255);
              this.Property(t => t.Terms_of_Delivery_Desc).HasColumnName("Terms_of_Delivery_Desc").HasMaxLength(255);
              this.Property(t => t.Invoice_Number).HasColumnName("Invoice_Number").IsRequired().HasMaxLength(255);
              this.Property(t => t.Invoice_Date).HasColumnName("Invoice_Date").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_ind).HasColumnName("Currency_Rate_ind").HasMaxLength(255);
              this.Property(t => t.Net_Price).HasColumnName("Net_Price").IsRequired().HasMaxLength(255);
              this.Property(t => t.Currency_code_net).HasColumnName("Currency_code_net").HasMaxLength(255);
              this.Property(t => t.Currency_Name_net).HasColumnName("Currency_Name_net").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_net).HasColumnName("Currency_Rate_net").HasMaxLength(255);
              this.Property(t => t.Indirect_Payments).HasColumnName("Indirect_Payments").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_com).HasColumnName("Currency_Rate_com").HasMaxLength(255);
              this.Property(t => t.Commissions).HasColumnName("Commissions").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_brg).HasColumnName("Currency_Rate_brg").HasMaxLength(255);
              this.Property(t => t.Brokerage).HasColumnName("Brokerage").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_cap).HasColumnName("Currency_Rate_cap").HasMaxLength(255);
              this.Property(t => t.Containers_Packaging).HasColumnName("Containers_Packaging").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_mcp).HasColumnName("Currency_Rate_mcp").HasMaxLength(255);
              this.Property(t => t.Material_Components).HasColumnName("Material_Components").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_tls).HasColumnName("Currency_Rate_tls").HasMaxLength(255);
              this.Property(t => t.Tool_Dies).HasColumnName("Tool_Dies").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_mcg).HasColumnName("Currency_Rate_mcg").HasMaxLength(255);
              this.Property(t => t.Materials_Consumed).HasColumnName("Materials_Consumed").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_eng).HasColumnName("Currency_Rate_eng").HasMaxLength(255);
              this.Property(t => t.Engineering_Development).HasColumnName("Engineering_Development").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_roy).HasColumnName("Currency_Rate_roy").HasMaxLength(255);
              this.Property(t => t.Royalties_licence_fees).HasColumnName("Royalties_licence_fees").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_pro).HasColumnName("Currency_Rate_pro").HasMaxLength(255);
              this.Property(t => t.Proceeds).HasColumnName("Proceeds").HasMaxLength(255);
              this.Property(t => t.Currency_code_tpt).HasColumnName("Currency_code_tpt").HasMaxLength(255);
              this.Property(t => t.Currency_Name_tpt).HasColumnName("Currency_Name_tpt").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_tpt).HasColumnName("Currency_Rate_tpt").HasMaxLength(255);
              this.Property(t => t.Transport).HasColumnName("Transport").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_lhc).HasColumnName("Currency_Rate_lhc").HasMaxLength(255);
              this.Property(t => t.Loading_handling).HasColumnName("Loading_handling").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_ins).HasColumnName("Currency_Rate_ins").HasMaxLength(255);
              this.Property(t => t.Insurance).HasColumnName("Insurance").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_aim).HasColumnName("Currency_Rate_aim").HasMaxLength(255);
              this.Property(t => t.Transport_after_import).HasColumnName("Transport_after_import").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_cfc).HasColumnName("Currency_Rate_cfc").HasMaxLength(255);
              this.Property(t => t.Construction).HasColumnName("Construction").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_oth).HasColumnName("Currency_Rate_oth").HasMaxLength(255);
              this.Property(t => t.Other_charges).HasColumnName("Other_charges").HasMaxLength(255);
              this.Property(t => t.Currency_Rate_txs).HasColumnName("Currency_Rate_txs").HasMaxLength(255);
              this.Property(t => t.Customs_duties_taxes).HasColumnName("Customs_duties_taxes").HasMaxLength(255);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Value_declaration_form_Id).HasColumnName("Value_declaration_form_Id");
              this.Property(t => t.Currency_Name_com).HasColumnName("Currency_Name_com").HasMaxLength(255);
              this.Property(t => t.Currency_code_ind).HasColumnName("Currency_code_ind").HasMaxLength(255);
              this.Property(t => t.Currency_code_mcp).HasColumnName("Currency_code_mcp").HasMaxLength(255);
              this.Property(t => t.Currency_code_ins).HasColumnName("Currency_code_ins").HasMaxLength(255);
              this.Property(t => t.Currency_Name_ind).HasColumnName("Currency_Name_ind").HasMaxLength(255);
              this.Property(t => t.Currency_Name_mcg).HasColumnName("Currency_Name_mcg").HasMaxLength(255);
              this.Property(t => t.Other_specify).HasColumnName("Other_specify").HasMaxLength(255);
              this.Property(t => t.Currency_Name_mcp).HasColumnName("Currency_Name_mcp").HasMaxLength(255);
              this.Property(t => t.Currency_Name_brg).HasColumnName("Currency_Name_brg").HasMaxLength(255);
              this.Property(t => t.Currency_code_tls).HasColumnName("Currency_code_tls").HasMaxLength(255);
              this.Property(t => t.Currency_code_txs).HasColumnName("Currency_code_txs").HasMaxLength(255);
              this.Property(t => t.Currency_code_oth).HasColumnName("Currency_code_oth").HasMaxLength(255);
              this.Property(t => t.Currency_Name_eng).HasColumnName("Currency_Name_eng").HasMaxLength(255);
              this.Property(t => t.Currency_Name_cap).HasColumnName("Currency_Name_cap").HasMaxLength(255);
              this.Property(t => t.Currency_Name_aim).HasColumnName("Currency_Name_aim").HasMaxLength(255);
              this.Property(t => t.Currency_code_eng).HasColumnName("Currency_code_eng").HasMaxLength(255);
              this.Property(t => t.Currency_code_com).HasColumnName("Currency_code_com").HasMaxLength(255);
              this.Property(t => t.Currency_Name_lhc).HasColumnName("Currency_Name_lhc").HasMaxLength(255);
              this.Property(t => t.Currency_code_roy).HasColumnName("Currency_code_roy").HasMaxLength(255);
              this.Property(t => t.Currency_code_aim).HasColumnName("Currency_code_aim").HasMaxLength(255);
              this.Property(t => t.Currency_Name_tls).HasColumnName("Currency_Name_tls").HasMaxLength(255);
              this.Property(t => t.Currency_code_mcg).HasColumnName("Currency_code_mcg").HasMaxLength(255);
              this.Property(t => t.Currency_code_pro).HasColumnName("Currency_code_pro").HasMaxLength(255);
              this.Property(t => t.Currency_Name_cfc).HasColumnName("Currency_Name_cfc").HasMaxLength(255);
              this.Property(t => t.Currency_Name_roy).HasColumnName("Currency_Name_roy").HasMaxLength(255);
              this.Property(t => t.Currency_code_brg).HasColumnName("Currency_code_brg").HasMaxLength(255);
              this.Property(t => t.Currency_code_cap).HasColumnName("Currency_code_cap").HasMaxLength(255);
              this.Property(t => t.Currency_Name_ins).HasColumnName("Currency_Name_ins").HasMaxLength(255);
              this.Property(t => t.Currency_Name_pro).HasColumnName("Currency_Name_pro").HasMaxLength(255);
              this.Property(t => t.Currency_code_cfc).HasColumnName("Currency_code_cfc").HasMaxLength(255);
              this.Property(t => t.Currency_Name_txs).HasColumnName("Currency_Name_txs").HasMaxLength(255);
              this.Property(t => t.Currency_code_lhc).HasColumnName("Currency_code_lhc").HasMaxLength(255);
              this.Property(t => t.Currency_Name_oth).HasColumnName("Currency_Name_oth").HasMaxLength(255);
              this.HasOptional(t => t.xC71_Value_declaration_form).WithMany(t =>(ICollection<xC71_Item>) t.xC71_Item).HasForeignKey(d => d.Value_declaration_form_Id);
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
