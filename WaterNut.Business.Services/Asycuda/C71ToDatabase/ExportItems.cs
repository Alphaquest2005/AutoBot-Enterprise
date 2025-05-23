using Asycuda421; // Assuming Value_declaration_formItem and related types are here
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

        private void ExportItems(ObservableCollection<Value_declaration_formItem> adocItem, xC71_Value_declaration_form c71)
        {
            try
            {
                foreach (var dItem in c71.xC71_Item) // Potential NullReferenceException
                {
                    //var aItem = adocItem.FirstOrDefault(x => x.Invoice_Number.Text.FirstOrDefault() == dItem.Invoice_Number);
                    //if (aItem == null)
                    //{
                        var aItem = new Value_declaration_formItem();
                        adocItem.Add(aItem);
                   // }

                    // Potential NullReferenceExceptions throughout if dItem or its properties are null
                    aItem.Terms_of_Delivery_Code.Text.Add(dItem.Terms_of_Delivery_Code);
                    if (dItem.Terms_of_Delivery_Desc == null) aItem.Terms_of_Delivery_Desc = new Value_declaration_formItemTerms_of_Delivery_Desc() { @null = new object() }; else aItem.Terms_of_Delivery_Desc.Text.Add(dItem.Terms_of_Delivery_Desc);
                    aItem.Invoice_Number.Text.Add(dItem.Invoice_Number);
                    aItem.Invoice_Date = dItem.Invoice_Date;
                    if (dItem.Currency_code_ind == null) aItem.Currency_code_ind = new Value_declaration_formItemCurrency_code_ind() { @null = new object() }; else aItem.Currency_code_ind.Text.Add(dItem.Currency_code_ind);
                    if (dItem.Currency_Name_ind == null) aItem.Currency_Name_ind = new Value_declaration_formItemCurrency_Name_ind() { @null = new object() }; else aItem.Currency_Name_ind.Text.Add(dItem.Currency_Name_ind);
                    aItem.Currency_Rate_ind = dItem.Currency_Rate_ind??"0";
                    aItem.Net_Price = dItem.Net_Price;
                    if (dItem.Currency_code_net == null) aItem.Currency_code_net = new Value_declaration_formItemCurrency_code_net() { @null = new object() }; else aItem.Currency_code_net.Text.Add(dItem.Currency_code_net);
                    if (dItem.Currency_Name_net == null) aItem.Currency_Name_net = new Value_declaration_formItemCurrency_Name_net() { @null = new object() }; else aItem.Currency_Name_net.Text.Add(dItem.Currency_Name_net);
                    if (dItem.Currency_Rate_net == null) aItem.Currency_Rate_net = new Value_declaration_formItemCurrency_Rate_net() { @null = new object() }; else aItem.Currency_Rate_net.Text.Add(dItem.Currency_Rate_net);

                    aItem.Indirect_Payments = dItem.Indirect_Payments??"0";
                    aItem.Currency_Rate_com = dItem.Currency_Rate_com ?? "0";
                    aItem.Commissions = dItem.Commissions ?? "0";
                    aItem.Currency_Rate_brg = dItem.Currency_Rate_brg ?? "0";
                    aItem.Brokerage = dItem.Brokerage ?? "0";
                    aItem.Currency_Rate_cap = dItem.Currency_Rate_cap ?? "0";
                    aItem.Containers_Packaging = dItem.Containers_Packaging??"0";
                    aItem.Currency_Rate_mcp = dItem.Currency_Rate_mcp ?? "0";
                    aItem.Material_Components = dItem.Material_Components??"0";
                    aItem.Currency_Rate_tls = dItem.Currency_Rate_tls ?? "0";
                    aItem.Tool_Dies = dItem.Tool_Dies ?? "0";
                    aItem.Currency_Rate_mcg = dItem.Currency_Rate_mcg ?? "0";
                    aItem.Materials_Consumed = dItem.Materials_Consumed??"0";
                    aItem.Currency_Rate_eng = dItem.Currency_Rate_eng ?? "0";
                    aItem.Engineering_Development = dItem.Engineering_Development??"0";
                    aItem.Currency_Rate_roy = dItem.Currency_Rate_roy ?? "0";
                    aItem.Royalties_licence_fees = dItem.Royalties_licence_fees??"0";
                    aItem.Currency_Rate_ins = dItem.Currency_Rate_ins ?? "0";
                    aItem.Insurance = dItem.Insurance??"0";
                    aItem.Currency_Rate_aim = dItem.Currency_Rate_aim ?? "0";
                    aItem.Transport_after_import = dItem.Transport_after_import ?? "0";
                    aItem.Currency_Rate_cfc = dItem.Currency_Rate_cfc ?? "0";
                    aItem.Construction = dItem.Construction ?? "0";
                    aItem.Currency_Rate_oth = dItem.Currency_Rate_oth ?? "0";
                    aItem.Other_charges = dItem.Other_charges ?? "0";
                    aItem.Currency_Rate_txs = dItem.Currency_Rate_txs ?? "0";
                    aItem.Customs_duties_taxes = dItem.Customs_duties_taxes ?? "0";
                    if (dItem.Currency_Name_com == null) aItem.Currency_Name_com = new Value_declaration_formItemCurrency_Name_com() { @null = new object() }; else aItem.Currency_Name_com.Text.Add(dItem.Currency_Name_com);
                    if (dItem.Currency_Rate_pro == null) aItem.Currency_Rate_pro = new Value_declaration_formItemCurrency_Rate_pro() { @null = new object() }; else aItem.Currency_Rate_pro.Text.Add(dItem.Currency_Rate_pro);
                    if (dItem.Proceeds == null) aItem.Proceeds = new Value_declaration_formItemProceeds() { @null = new object() }; else aItem.Proceeds.Text.Add(dItem.Proceeds);
                    if (dItem.Currency_code_tpt == null) aItem.Currency_code_tpt = new Value_declaration_formItemCurrency_code_tpt() { @null = new object() }; else aItem.Currency_code_tpt.Text.Add(dItem.Currency_code_tpt);
                    if (dItem.Currency_Name_tpt == null) aItem.Currency_Name_tpt = new Value_declaration_formItemCurrency_Name_tpt() { @null = new object() }; else aItem.Currency_Name_tpt.Text.Add(dItem.Currency_Name_tpt);
                    if (dItem.Currency_Rate_tpt == null) aItem.Currency_Rate_tpt = new Value_declaration_formItemCurrency_Rate_tpt() { @null = new object() }; else aItem.Currency_Rate_tpt.Text.Add(dItem.Currency_Rate_tpt);
                    if (dItem.Transport == null) aItem.Transport = new Value_declaration_formItemTransport() { @null = new object() }; else aItem.Transport.Text.Add(dItem.Transport);
                    if (dItem.Currency_Rate_lhc == null) aItem.Currency_Rate_lhc = new Value_declaration_formItemCurrency_Rate_lhc() { @null = new object() }; else aItem.Currency_Rate_lhc.Text.Add(dItem.Currency_Rate_lhc);
                    if (dItem.Loading_handling == null) aItem.Loading_handling = new Value_declaration_formItemLoading_handling() { @null = new object() }; else aItem.Loading_handling.Text.Add(dItem.Loading_handling);

                    if (dItem.Currency_code_mcp == null) aItem.Currency_code_mcp = new Value_declaration_formItemCurrency_code_mcp() { @null = new object() }; else aItem.Currency_code_mcp.Text.Add(dItem.Currency_code_mcp);
                    if (dItem.Currency_code_ins == null) aItem.Currency_code_ins = new Value_declaration_formItemCurrency_code_ins() { @null = new object() }; else aItem.Currency_code_ins.Text.Add(dItem.Currency_code_ins);

                    if (dItem.Currency_Name_mcg == null) aItem.Currency_Name_mcg = new Value_declaration_formItemCurrency_Name_mcg() { @null = new object() }; else aItem.Currency_Name_mcg.Text.Add(dItem.Currency_Name_mcg);
                    //if (dItem.Other_specify == null) aItem.Other_specify = new Value_declaration_formItemOther_specify() { @null = new object() }; else aItem.Other_specify.Text.Add(dItem.Other_specify);

                    aItem.Other_specify = null; // Explicitly setting to null as in original code

                    if (dItem.Currency_Name_mcp == null) aItem.Currency_Name_mcp = new Value_declaration_formItemCurrency_Name_mcp() { @null = new object() }; else aItem.Currency_Name_mcp.Text.Add(dItem.Currency_Name_mcp);
                    if (dItem.Currency_Name_brg == null) aItem.Currency_Name_brg = new Value_declaration_formItemCurrency_Name_brg() { @null = new object() }; else aItem.Currency_Name_brg.Text.Add(dItem.Currency_Name_brg);
                    if (dItem.Currency_code_tls == null) aItem.Currency_code_tls = new Value_declaration_formItemCurrency_code_tls() { @null = new object() }; else aItem.Currency_code_tls.Text.Add(dItem.Currency_code_tls);
                    if (dItem.Currency_code_txs == null) aItem.Currency_code_txs = new Value_declaration_formItemCurrency_code_txs() { @null = new object() }; else aItem.Currency_code_txs.Text.Add(dItem.Currency_code_txs);
                    if (dItem.Currency_code_oth == null) aItem.Currency_code_oth = new Value_declaration_formItemCurrency_code_oth() { @null = new object() }; else aItem.Currency_code_oth.Text.Add(dItem.Currency_code_oth);
                    if (dItem.Currency_Name_eng == null) aItem.Currency_Name_eng = new Value_declaration_formItemCurrency_Name_eng() { @null = new object() }; else aItem.Currency_Name_eng.Text.Add(dItem.Currency_Name_eng);
                    if (dItem.Currency_Name_cap == null) aItem.Currency_Name_cap = new Value_declaration_formItemCurrency_Name_cap() { @null = new object() }; else aItem.Currency_Name_cap.Text.Add(dItem.Currency_Name_cap);
                    if (dItem.Currency_Name_aim == null) aItem.Currency_Name_aim = new Value_declaration_formItemCurrency_Name_aim() { @null = new object() }; else aItem.Currency_Name_aim.Text.Add(dItem.Currency_Name_aim);
                    if (dItem.Currency_code_eng == null) aItem.Currency_code_eng = new Value_declaration_formItemCurrency_code_eng() { @null = new object() }; else aItem.Currency_code_eng.Text.Add(dItem.Currency_code_eng);
                    if (dItem.Currency_code_com == null) aItem.Currency_code_com = new Value_declaration_formItemCurrency_code_com() { @null = new object() }; else aItem.Currency_code_com.Text.Add(dItem.Currency_code_com);
                    if (dItem.Currency_Name_lhc == null) aItem.Currency_Name_lhc = new Value_declaration_formItemCurrency_Name_lhc() { @null = new object() }; else aItem.Currency_Name_lhc.Text.Add(dItem.Currency_Name_lhc);
                    if (dItem.Currency_code_roy == null) aItem.Currency_code_roy = new Value_declaration_formItemCurrency_code_roy() { @null = new object() }; else aItem.Currency_code_roy.Text.Add(dItem.Currency_code_roy);
                    if (dItem.Currency_code_aim == null) aItem.Currency_code_aim = new Value_declaration_formItemCurrency_code_aim() { @null = new object() }; else aItem.Currency_code_aim.Text.Add(dItem.Currency_code_aim);
                    if (dItem.Currency_Name_tls == null) aItem.Currency_Name_tls = new Value_declaration_formItemCurrency_Name_tls() { @null = new object() }; else aItem.Currency_Name_tls.Text.Add(dItem.Currency_Name_tls);
                    if (dItem.Currency_code_mcg == null) aItem.Currency_code_mcg = new Value_declaration_formItemCurrency_code_mcg() { @null = new object() }; else aItem.Currency_code_mcg.Text.Add(dItem.Currency_code_mcg);
                    if (dItem.Currency_code_pro == null) aItem.Currency_code_pro = new Value_declaration_formItemCurrency_code_pro() { @null = new object() }; else aItem.Currency_code_pro.Text.Add(dItem.Currency_code_pro);
                    if (dItem.Currency_Name_cfc == null) aItem.Currency_Name_cfc = new Value_declaration_formItemCurrency_Name_cfc() { @null = new object() }; else aItem.Currency_Name_cfc.Text.Add(dItem.Currency_Name_cfc);
                    if (dItem.Currency_Name_roy == null) aItem.Currency_Name_roy = new Value_declaration_formItemCurrency_Name_roy() { @null = new object() }; else aItem.Currency_Name_roy.Text.Add(dItem.Currency_Name_roy);
                    if (dItem.Currency_code_brg == null) aItem.Currency_code_brg = new Value_declaration_formItemCurrency_code_brg() { @null = new object() }; else aItem.Currency_code_brg.Text.Add(dItem.Currency_code_brg);
                    if (dItem.Currency_code_cap == null) aItem.Currency_code_cap = new Value_declaration_formItemCurrency_code_cap() { @null = new object() }; else aItem.Currency_code_cap.Text.Add(dItem.Currency_code_cap);
                    if (dItem.Currency_Name_ins == null) aItem.Currency_Name_ins = new Value_declaration_formItemCurrency_Name_ins() { @null = new object() }; else aItem.Currency_Name_ins.Text.Add(dItem.Currency_Name_ins);
                    if (dItem.Currency_Name_pro == null) aItem.Currency_Name_pro = new Value_declaration_formItemCurrency_Name_pro() { @null = new object() }; else aItem.Currency_Name_pro.Text.Add(dItem.Currency_Name_pro);
                    if (dItem.Currency_code_cfc == null) aItem.Currency_code_cfc = new Value_declaration_formItemCurrency_code_cfc() { @null = new object() }; else aItem.Currency_code_cfc.Text.Add(dItem.Currency_code_cfc);
                    if (dItem.Currency_Name_txs == null) aItem.Currency_Name_txs = new Value_declaration_formItemCurrency_Name_txs() { @null = new object() }; else aItem.Currency_Name_txs.Text.Add(dItem.Currency_Name_txs);
                    if (dItem.Currency_code_lhc == null) aItem.Currency_code_lhc = new Value_declaration_formItemCurrency_code_lhc() { @null = new object() }; else aItem.Currency_code_lhc.Text.Add(dItem.Currency_code_lhc);
                    if (dItem.Currency_Name_oth == null) aItem.Currency_Name_oth = new Value_declaration_formItemCurrency_Name_oth() { @null = new object() }; else aItem.Currency_Name_oth.Text.Add(dItem.Currency_Name_oth);
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