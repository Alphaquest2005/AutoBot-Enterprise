using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using InventoryDS.Business.Entities; // Assuming InventoryDSContext, InventoryItem, InventoryItemSource, InventorySources are here
using TrackableEntities; // Assuming TrackingState is here
using TrackableEntities.EF6; // Assuming ApplyChanges, SingleUpdate exist here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<InventoryItem> SaveInventoryItem(ASYCUDAItem ai)
        {
            try
            {
                using (var ctx = new InventoryDSContext(){StartTracking = true})
                {
                    var iv = ctx.InventoryItems.FirstOrDefault(x =>
                        x.ItemNumber == ai.Tarification.HScode.Precision_4.Text.FirstOrDefault() && x.ApplicationSettingsId == da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId); // Potential NullReferenceExceptions
                    ////    (await DataSpace.InventoryDS.ViewModels.BaseDataModel.Instance.SearchInventoryItem(new List<string>()
                    ////{
                    ////    string.Format("ItemNumber == \"{0}\"",ai.Tarification.HScode.Precision_4.Text.FirstOrDefault())
                    ////}).ConfigureAwait(false)).FirstOrDefault();
                    //InventoryItems.FirstOrDefault(i => i.ItemNumber == ai.Tarification.HScode.Precision_4.Text.FirstOrDefault());
                    if (iv == null && ai.Tarification.HScode.Precision_4.Text.FirstOrDefault() != null) // Potential NullReferenceException
                    {
                        iv = new InventoryItem(true)
                        {
                            ItemNumber = ai.Tarification.HScode.Precision_4.Text.FirstOrDefault(), // Potential NullReferenceException
                            ApplicationSettingsId = da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId, // Potential NullReferenceException
                            Description = ai.Goods_description.Commercial_Description.Text.FirstOrDefault()??"", // Potential NullReferenceException
                            TariffCode = ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault(), // Potential NullReferenceException
                            InventoryItemSources = new List<InventoryItemSource>() {new InventoryItemSource(true)
                            {
                                InventorySourceId = ctx.InventorySources.First(x => x.Name == "Asycuda").Id, // InvalidOperationException if no match
                            }},
                            TrackingState = TrackingState.Added
                        };

                        ctx.ApplyChanges(iv); // Assuming ApplyChanges exists
                        ctx.SaveChanges();

                        return iv;
                    }

                    if (iv == null || !updateItemsTariffCode || iv.TariffCode == ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault()) return iv; // Potential NullReferenceException
                    //iv.StartTracking();
                    iv.TariffCode = ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault(); // Potential NullReferenceException

                    ctx.SingleUpdate(iv); // Assuming SingleUpdate exists
                    ctx.SaveChanges();

                    //include tarrifcode
                    return iv;
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