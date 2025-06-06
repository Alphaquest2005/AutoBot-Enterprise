using Asycuda421; // Assuming Value_declaration_formItem is here
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrackableEntities; // Assuming TrackingState is here
using ValuationDS.Business.Entities; // Assuming xC71_Value_declaration_form, xC71_Item are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        // Assuming 'da' is a field accessible across partial classes
        // private ValuationDS.Business.Entities.Registered da;

        private void SaveItems(ObservableCollection<Value_declaration_formItem> aItems,
            xC71_Value_declaration_form xC71ValueDeclarationForm) // Parameter name changed for clarity
        {
            try
            {
                foreach (var aItem in aItems)
                {
                    // Potential NullReferenceException if da or xC71_Item is null
                    var dItem = da.xC71_Item.FirstOrDefault(x => x.Invoice_Number == aItem.Invoice_Number.Text.FirstOrDefault()); // Potential NullReferenceException
                    if (dItem == null)
                    {
                        dItem = new xC71_Item(true)
                        {
                            TrackingState = TrackingState.Added
                        };
                        da.xC71_Item.Add(dItem); // Potential NullReferenceException
                    }

                    // Potential NullReferenceExceptions on accessing aItem properties
                    dItem.Terms_of_Delivery_Code = aItem.Terms_of_Delivery_Code.Text.FirstOrDefault();
                    dItem.Terms_of_Delivery_Desc = aItem.Terms_of_Delivery_Desc.Text.FirstOrDefault();
                    dItem.Invoice_Number = aItem.Invoice_Number.Text.FirstOrDefault();
                    dItem.Invoice_Date = aItem.Invoice_Date;
                    dItem.Currency_Rate_ind = aItem.Currency_Rate_ind;
                    dItem.Net_Price = aItem.Net_Price;
                    dItem.Currency_code_net = aItem.Currency_code_net.Text.FirstOrDefault();
                    dItem.Currency_Name_net = aItem.Currency_Name_net.Text.FirstOrDefault();
                    dItem.Currency_Rate_net = aItem.Currency_Rate_net.Text.FirstOrDefault();
                    dItem.Indirect_Payments = aItem.Indirect_Payments;
                    dItem.Currency_Rate_com = aItem.Currency_Rate_com;
                    dItem.Commissions = aItem.Commissions;
                    dItem.Currency_Rate_brg = aItem.Currency_Rate_brg;
                    dItem.Brokerage = aItem.Brokerage;
                    dItem.Currency_Rate_cap = aItem.Currency_Rate_cap;
                    dItem.Containers_Packaging = aItem.Containers_Packaging;
                    dItem.Currency_Rate_mcp = aItem.Currency_Rate_mcp;
                    dItem.Material_Components = aItem.Material_Components;
                    dItem.Currency_Rate_tls = aItem.Currency_Rate_tls;
                    dItem.Tool_Dies = aItem.Tool_Dies;
                    dItem.Currency_Rate_mcg = aItem.Currency_Rate_mcg;
                    dItem.Materials_Consumed = aItem.Materials_Consumed;
                    dItem.Currency_Rate_eng = aItem.Currency_Rate_eng;
                    dItem.Engineering_Development = aItem.Engineering_Development;
                    dItem.Currency_Rate_roy = aItem.Currency_Rate_roy;
                    dItem.Royalties_licence_fees = aItem.Royalties_licence_fees;
                    dItem.Currency_Rate_pro = aItem.Currency_Rate_pro.Text.FirstOrDefault();
                    dItem.Proceeds = aItem.Proceeds.Text.FirstOrDefault();
                    dItem.Currency_code_tpt = aItem.Currency_code_tpt.Text.FirstOrDefault();
                    dItem.Currency_Name_tpt = aItem.Currency_Name_tpt.Text.FirstOrDefault();
                    dItem.Currency_Rate_tpt = aItem.Currency_Rate_tpt.Text.FirstOrDefault();
                    dItem.Transport = aItem.Transport.Text.FirstOrDefault();
                    dItem.Currency_Rate_lhc = aItem.Currency_Rate_lhc.Text.FirstOrDefault();
                    dItem.Loading_handling = aItem.Loading_handling.Text.FirstOrDefault();
                    dItem.Currency_Rate_ins = aItem.Currency_Rate_ins;
                    dItem.Insurance = aItem.Insurance;
                    dItem.Currency_Rate_aim = aItem.Currency_Rate_aim;
                    dItem.Transport_after_import = aItem.Transport_after_import;
                    dItem.Currency_Rate_cfc = aItem.Currency_Rate_cfc;
                    dItem.Construction = aItem.Construction;
                    dItem.Currency_Rate_oth = aItem.Currency_Rate_oth;
                    dItem.Other_charges = aItem.Other_charges;
                    dItem.Currency_Rate_txs = aItem.Currency_Rate_txs;
                    dItem.Customs_duties_taxes = aItem.Customs_duties_taxes;
                    dItem.Currency_Name_com = aItem.Currency_Name_com.Text.FirstOrDefault();
                    dItem.Currency_code_ind = aItem.Currency_code_ind.Text.FirstOrDefault();
                    dItem.Currency_code_mcp = aItem.Currency_code_mcp.Text.FirstOrDefault();
                    dItem.Currency_code_ins = aItem.Currency_code_ins.Text.FirstOrDefault();
                    dItem.Currency_Name_ind = aItem.Currency_Name_ind.Text.FirstOrDefault();
                    dItem.Currency_Name_mcg = aItem.Currency_Name_mcg.Text.FirstOrDefault();
                  //  dItem.Other_specify = aItem.Other_specify.Text.FirstOrDefault();
                    dItem.Currency_Name_mcp = aItem.Currency_Name_mcp.Text.FirstOrDefault();
                    dItem.Currency_Name_brg = aItem.Currency_Name_brg.Text.FirstOrDefault();
                    dItem.Currency_code_tls = aItem.Currency_code_tls.Text.FirstOrDefault();
                    dItem.Currency_code_txs = aItem.Currency_code_txs.Text.FirstOrDefault();
                    dItem.Currency_code_oth = aItem.Currency_code_oth.Text.FirstOrDefault();
                    dItem.Currency_Name_eng = aItem.Currency_Name_eng.Text.FirstOrDefault();
                    dItem.Currency_Name_cap = aItem.Currency_Name_cap.Text.FirstOrDefault();
                    dItem.Currency_Name_aim = aItem.Currency_Name_aim.Text.FirstOrDefault();
                    dItem.Currency_code_eng = aItem.Currency_code_eng.Text.FirstOrDefault();
                    dItem.Currency_code_com = aItem.Currency_code_com.Text.FirstOrDefault();
                    dItem.Currency_Name_lhc = aItem.Currency_Name_lhc.Text.FirstOrDefault();
                    dItem.Currency_code_roy = aItem.Currency_code_roy.Text.FirstOrDefault();
                    dItem.Currency_code_aim = aItem.Currency_code_aim.Text.FirstOrDefault();
                    dItem.Currency_Name_tls = aItem.Currency_Name_tls.Text.FirstOrDefault();
                    dItem.Currency_code_mcg = aItem.Currency_code_mcg.Text.FirstOrDefault();
                    dItem.Currency_code_pro = aItem.Currency_code_pro.Text.FirstOrDefault();
                    dItem.Currency_Name_cfc = aItem.Currency_Name_cfc.Text.FirstOrDefault();
                    dItem.Currency_Name_roy = aItem.Currency_Name_roy.Text.FirstOrDefault();
                    dItem.Currency_code_brg = aItem.Currency_code_brg.Text.FirstOrDefault();
                    dItem.Currency_code_cap = aItem.Currency_code_cap.Text.FirstOrDefault();
                    dItem.Currency_Name_ins = aItem.Currency_Name_ins.Text.FirstOrDefault();
                    dItem.Currency_Name_pro = aItem.Currency_Name_pro.Text.FirstOrDefault();
                    dItem.Currency_code_cfc = aItem.Currency_code_cfc.Text.FirstOrDefault();
                    dItem.Currency_Name_txs = aItem.Currency_Name_txs.Text.FirstOrDefault();
                    dItem.Currency_code_lhc = aItem.Currency_code_lhc.Text.FirstOrDefault();
                    dItem.Currency_Name_oth = aItem.Currency_Name_oth.Text.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}