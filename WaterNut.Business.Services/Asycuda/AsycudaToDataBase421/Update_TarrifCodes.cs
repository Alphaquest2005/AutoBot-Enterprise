using System;
using System.Data.Entity; // For Include
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using InventoryDS.Business.Entities; // Assuming InventoryDSContext, TariffCode, TariffCategory, TariffCategoryCodeSuppUnit, TariffSupUnitLkp are here
using TrackableEntities; // Assuming TrackingState is here
using TrackableEntities.EF6; // Assuming ApplyChanges exists here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Update_TarrifCodes(ASYCUDAItem ai)
        {
            try
            {
                if (!ai.Tarification.HScode.Commodity_code.Text.Any()) return; // Potential NullReferenceException

                using (var ctx = new InventoryDSContext())
                {
                    var tariffCode = ctx.TariffCodes
                        .Include("TariffCategory.TariffCategoryCodeSuppUnits.TariffSupUnitLkp")
                        .Include(x => x.TariffCategory)
                        .FirstOrDefault(x => x.TariffCodeName == ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault()); // Potential NullReferenceException
                        if(tariffCode == null)
                        tariffCode = new TariffCode(true)
                                     {
                                         TariffCodeName = ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault(), // Potential NullReferenceException
                            TariffCategory = ctx.TariffCategories.FirstOrDefault(x =>
                                                              x.TariffCategoryCode == ai.Tarification.HScode // Potential NullReferenceException
                                                                  .Commodity_code.Text.FirstOrDefault().Substring(0, 4)), // Potential NullReferenceException
                                         TrackingState = TrackingState.Added
                                     };

                    if (tariffCode.TariffCategory == null)
                    {
                        var cat = ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault()?.Substring(0, 4); // Potential NullReferenceException
                        var tariffCategory = ctx.TariffCategories
                            .Include("TariffCategoryCodeSuppUnits.TariffSupUnitLkp")
                            .FirstOrDefault(x => x.TariffCategoryCode == cat);
                        if (tariffCategory == null)
                        {
                            tariffCategory = new TariffCategory(true)
                            {
                                TariffCategoryCode = cat,
                                Description = ai.Goods_description.Description_of_goods.Text.Any() ? ai.Goods_description.Description_of_goods.Text[0] : "" // Potential NullReferenceException
                                ,
                                TrackingState = TrackingState.Added
                            };
                        }
                        tariffCode.TariffCategory = tariffCategory;
                    }

                    for (var i = 0;
                        i < ai.Tarification.Supplementary_unit.Count(x => x.Suppplementary_unit_code.Text.Count > 0); // Potential NullReferenceException
                        i++)
                    {
                        var au = ai.Tarification.Supplementary_unit.ElementAt(i); // Potential NullReferenceException

                        var lst = tariffCode.TariffCategory?.TariffCategoryCodeSuppUnits? // Null conditional operators added
                            .Where(z => z.TariffSupUnitLkp.SuppUnitCode2 == au.Suppplementary_unit_code.Text[0]); // Potential NullReferenceException
                        if (lst == null || !lst.Any())
                        {
                            var tcc = au.Suppplementary_unit_code.Text[0]; // Potential NullReferenceException
                            var tn = au.Suppplementary_unit_name.Text.Any() ? au.Suppplementary_unit_name.Text[0] : ""; // Potential NullReferenceException

                            TariffSupUnitLkp tariffSupUnitLkp =
                                ctx.TariffSupUnitLkps.FirstOrDefault(x => x.SuppUnitCode2 == tcc)
                                ?? new TariffSupUnitLkp(true)
                                {
                                    SuppUnitCode2 = tcc,
                                    SuppUnitName2 = tn,
                                    SuppQty = 1,
                                    TrackingState = TrackingState.Added
                                };

                            var supUnit = new TariffCategoryCodeSuppUnit(true)
                            {
                                TariffCategory = tariffCode.TariffCategory,
                                TariffSupUnitLkp = tariffSupUnitLkp,
                                TrackingState = TrackingState.Added
                            };

                            if (tariffCode.TariffCategory.TariffCategoryCodeSuppUnits == null) // Ensure collection is initialized
                            {
                                tariffCode.TariffCategory.TariffCategoryCodeSuppUnits = new System.Collections.ObjectModel.ObservableCollection<TariffCategoryCodeSuppUnit>();
                            }
                            tariffCode.TariffCategory.TariffCategoryCodeSuppUnits.Add(supUnit);
                            ctx.ApplyChanges(supUnit); // Assuming ApplyChanges exists
                            ctx.SaveChanges();
                            supUnit.AcceptChanges(); // Assuming AcceptChanges exists
                        }
                    }

                    if(ai.Goods_description.Description_of_goods.Text.Any()) tariffCode.Description = ai.Goods_description.Description_of_goods.Text[0]; // Potential NullReferenceException
                    if (ai.Licence_number.Text.Any()) tariffCode.TariffCategory.LicenseRequired = true; // Potential NullReferenceException

                    for (var i = 0; i < ai.Taxation.Taxation_line.Count(x => x.Duty_tax_code.Text.Count > 0); i++) // Potential NullReferenceException
                    {
                        var au = ai.Taxation.Taxation_line.ElementAt(i); // Potential NullReferenceException
                        var rate = (Convert.ToDouble(au.Duty_tax_rate) / 100).ToString("00.00"); // Potential NullReferenceException
                        switch (au.Duty_tax_code.Text[0]) // Potential NullReferenceException
                        {
                            case "CET":
                                tariffCode.RateofDuty = rate;
                                break;
                            case "CSC":
                                tariffCode.CustomsServiceCharge = rate;
                                break;
                            case "EVL":
                                tariffCode.EnvironmentalLevy = rate;
                                break;
                            case "EXT":
                                tariffCode.ExciseTax = rate;
                                break;
                            case "VAT":
                                tariffCode.VatRate = rate;
                                break;
                            case "PET":
                                tariffCode.PetrolTax = rate;
                                break;
                            default:
                                break;
                        }
                    }
                    ctx.ApplyChanges(tariffCode); // Assuming ApplyChanges exists
                    ctx.SaveChanges();
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