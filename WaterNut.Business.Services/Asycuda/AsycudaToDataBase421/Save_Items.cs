using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA, ASYCUDAItem are here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item is here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Items()
        {
            try
            {
                da.Document.xcuda_ASYCUDA_ExtendedProperties.DocumentLines = a.Item.Count; // Assuming 'da' and 'a' are accessible fields, Potential NullReferenceException
                for (var i = 0; i < a.Item.Count; i++)
                // Parallel.For(0, a.Item.Count, i =>
                {
                    var ai = a.Item.ElementAt(i);
                    xcuda_Item di;

                    //took this out because it bugging with number of items just leave it so
                    //if (!ai.Tarification.HScode.Commodity_code.Text.Any()) continue;

                    di = da.DocumentItems.ElementAtOrDefault(i); // Potential NullReferenceException

                    if (di == null)
                    {
                        di = new xcuda_Item(true)
                        {
                            ASYCUDA_Id = da.Document.ASYCUDA_Id, // Potential NullReferenceException
                            ImportComplete = false,
                            TrackingState = TrackingState.Added
                        };
                        // db.xcuda_Item.CreateObject();//
                        //DIBaseDataModel.Instance.Savexcuda_Item(di);
                        da.DocumentItems.Add(di); // Potential NullReferenceException
                    }

                    if (!string.IsNullOrEmpty(a.Identification.Registration.Number)) // Potential NullReferenceException
                    {
                        di.IsAssessed = true;
                    }

                    di.LineNumber = i + 1;
                    di.SalesFactor = 1;

                    if (!string.IsNullOrEmpty(ai.Quantity_deducted_from_licence))
                    {
                        if(ai.Licence_number.Text.Any()) di.Licence_number = ai.Licence_number.Text[0]; // Potential NullReferenceException
                        di.Amount_deducted_from_licence = Math.Round(double.Parse(ai.Amount_deducted_from_licence == "" ? "0" : ai.Amount_deducted_from_licence), 4).ToString();
                        di.Quantity_deducted_from_licence = Math.Round(double.Parse(ai.Quantity_deducted_from_licence == "" ? "0" : ai.Quantity_deducted_from_licence), 4).ToString();
                    }

                    // These call methods which need to be in their own partial classes
                    await Save_PreviousInvoiceInfo(di, ai).ConfigureAwait(false);
                    await Save_Item_Suppliers_link(di, ai).ConfigureAwait(false);
                    await Save_Item_Attached_documents(di, ai).ConfigureAwait(false);
                    await Save_Item_Packages(di, ai).ConfigureAwait(false);
                    await Save_Item_Tarification(di, ai).ConfigureAwait(false);
                    await Save_Item_Goods_description(di, ai).ConfigureAwait(false);
                    await Save_Item_Previous_doc(di, ai).ConfigureAwait(false);
                    await Save_Item_Taxation(di, ai).ConfigureAwait(false);
                    await Save_Item_Valuation_item(di, ai).ConfigureAwait(false);

                    di.ImportComplete = true;
                    //await DIBaseDataModel.Instance.Savexcuda_Item(di).ConfigureAwait(false); // Assuming Savexcuda_Item exists
                    if (UpdateItemsTariffCode) // Assuming UpdateItemsTariffCode is accessible field
                    {
                        // This calls Update_TarrifCodes, which needs to be in its own partial class
                        Update_TarrifCodes(ai);
                    }
                }
                //    );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}