using System;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Valuation_item, xcuda_Weight_itm are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Weight_Item(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var wi = vi.xcuda_Weight_itm; // Potential NullReferenceException
            if (wi == null)
            {
                wi = new xcuda_Weight_itm(true) { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_Weight_itm = wi; // Potential NullReferenceException
            }
            if (ai.Valuation_item.Weight_itm.Gross_weight_itm != "") // Potential NullReferenceException
                wi.Gross_weight_itm = Convert.ToDecimal(ai.Valuation_item.Weight_itm.Gross_weight_itm); // Potential NullReferenceException

            if (ai.Valuation_item.Weight_itm.Net_weight_itm != "") // Potential NullReferenceException
                wi.Net_weight_itm = Convert.ToDecimal(ai.Valuation_item.Weight_itm.Net_weight_itm); // Potential NullReferenceException
        }
    }
}