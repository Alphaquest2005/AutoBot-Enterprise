using System;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Valuation_item are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Valuation_item is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Valuation_item(xcuda_Item di, ASYCUDAItem ai)
        {
            var vi = di.xcuda_Valuation_item;//.FirstOrDefault();
            if (vi == null)
            {
                vi = new xcuda_Valuation_item(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
              //  DIBaseDataModel.Instance.Savexcuda_Valuation_item(vi); // Assuming Savexcuda_Valuation_item exists
                di.xcuda_Valuation_item = vi;//di.xcuda_Valuation_item.Add(vi);
            }
            if (ai.Valuation_item.Alpha_coeficient_of_apportionment != "") // Potential NullReferenceException
                vi.Alpha_coeficient_of_apportionment = ai.Valuation_item.Alpha_coeficient_of_apportionment; // Potential NullReferenceException
            if (ai.Valuation_item.Rate_of_adjustement != "") // Potential NullReferenceException
                vi.Rate_of_adjustement = Convert.ToDouble(ai.Valuation_item.Rate_of_adjustement); // Potential NullReferenceException
            if (ai.Valuation_item.Statistical_value != "") // Potential NullReferenceException
                vi.Statistical_value = Convert.ToSingle(ai.Valuation_item.Statistical_value); // Potential NullReferenceException
            if (ai.Valuation_item.Total_CIF_itm != "") // Potential NullReferenceException
                vi.Total_CIF_itm = Convert.ToSingle(ai.Valuation_item.Total_CIF_itm); // Potential NullReferenceException
            if (ai.Valuation_item.Total_cost_itm != "") // Potential NullReferenceException
                vi.Total_cost_itm = Convert.ToSingle(ai.Valuation_item.Total_cost_itm); // Potential NullReferenceException

            // These call methods which need to be in their own partial classes
            Save_Item_Invoice(vi, ai);
            Save_item_External_freight(vi, ai);
            Save_item_Internal_freight(vi, ai);
            Save_item_deduction(vi, ai);
            Save_item_other_cost(vi, ai);
            Save_item_insurance(vi, ai);
            Save_Weight_Item(vi, ai);

           //await DIBaseDataModel.Instance.Savexcuda_Valuation_item(vi).ConfigureAwait(false); // Assuming Savexcuda_Valuation_item exists
           // Added await Task.CompletedTask to make the method async as declared
           await Task.CompletedTask;
        }
    }
}