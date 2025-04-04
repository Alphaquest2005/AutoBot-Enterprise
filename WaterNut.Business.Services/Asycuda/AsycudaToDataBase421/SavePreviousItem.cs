using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA, ASYCUDAPrev_decl are here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_PreviousItem are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SavePreviousItem()
        {
            for (var i = 0; i < a.Prev_decl.Count; i++) // Assuming 'a' is accessible field
            {
                try
                {
                    var ai = a.Prev_decl.ElementAt(i);
                    if (ai == null) continue;
                    // Assuming 'da' is accessible field
                    var itm = da.DocumentItems.OrderBy(x => x.LineNumber).ElementAtOrDefault(i) ?? da.DocumentItems.FirstOrDefault(x => x.ItemNumber == ai.Prev_decl_HS_spec.Text.FirstOrDefault() && x.ItemQuantity == Convert.ToDouble(ai.Prev_decl_supp_quantity) && x.Gross_weight == Convert.ToDecimal(ai.Prev_decl_weight)); // Potential NullReferenceExceptions
                    if (itm == null) continue;
                    if (itm.xcuda_Tarification.Extended_customs_procedure == "9071") return; // Potential NullReferenceException
                    var pi = new xcuda_PreviousItem(true)
                                 {
                                     PreviousItem_Id = itm.Item_Id, TrackingState = TrackingState.Added
                                 };

                    itm.xcuda_PreviousItem = pi;
                    pi.xcuda_Item = itm;

                    if (LinkPi) // Assuming LinkPi is accessible field
                    {
                        // This calls LinkPIItem, which needs to be in its own partial class
                        await LinkPIItem(ai, itm, pi, NoMessages).ConfigureAwait(false); // Assuming NoMessages is accessible field
                    }

                    pi.Commodity_code = ai.Prev_decl_HS_prec.Text.FirstOrDefault(); // Potential NullReferenceException
                    pi.Current_item_number = Convert.ToInt32(ai.Prev_decl_current_item);
                    pi.Current_value = Convert.ToSingle(Math.Round(Convert.ToDouble(ai.Prev_decl_ref_value), 2));
                    pi.Goods_origin = ai.Prev_decl_country_origin.Text.FirstOrDefault(); // Potential NullReferenceException
                    pi.Hs_code = ai.Prev_decl_HS_code.Text.FirstOrDefault(); // Potential NullReferenceException
                    pi.Net_weight = Convert.ToDecimal(ai.Prev_decl_weight);
                    pi.Packages_number = ai.Prev_decl_number_packages;
                    pi.Prev_net_weight = Convert.ToDecimal(ai.Prev_decl_weight_written_off);
                    pi.Prev_reg_cuo = ai.Prev_decl_office_code;
                    pi.Prev_decl_HS_spec = ai.Prev_decl_HS_spec.Text.FirstOrDefault(); // Potential NullReferenceException
                    pi.Prev_reg_year = int.Parse(ai.Prev_decl_reg_year);
                    pi.Prev_reg_nbr = ai.Prev_decl_reg_number;
                    pi.Prev_reg_ser = ai.Prev_decl_reg_serial.Text.FirstOrDefault(); // Potential NullReferenceException
                    if (!string.IsNullOrEmpty(ai.Prev_decl_supp_quantity_written_off))
                        pi.Preveious_suplementary_quantity = Convert.ToSingle(ai.Prev_decl_supp_quantity_written_off);
                    pi.Previous_item_number = Convert.ToInt32(ai.Prev_decl_item_number);
                    pi.Previous_Packages_number = ai.Prev_decl_number_packages_written_off;

                    if (ai.Prev_decl_ref_value_written_off != null)
                        pi.Previous_value = (float)Math.Round(Convert.ToDouble(ai.Prev_decl_ref_value_written_off), 2);
                    if (!string.IsNullOrEmpty(ai.Prev_decl_supp_quantity))
                        pi.Suplementary_Quantity = Convert.ToDecimal(ai.Prev_decl_supp_quantity);

                    //await DataSpace.DocumentItemDS.ViewModels.BaseDataModel.Instance.Savexcuda_PreviousItem(pi).ConfigureAwait(false);
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }
    }
}