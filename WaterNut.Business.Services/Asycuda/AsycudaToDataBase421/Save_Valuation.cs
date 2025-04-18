using System;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Valuation is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Valuation is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Valuation()
        {
            var v = da.Document.xcuda_Valuation; // Assuming 'da' is accessible field, Potential NullReferenceException
            if (v == null)
            {
                v = new xcuda_Valuation(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                da.Document.xcuda_Valuation = v; // Potential NullReferenceException
            }
            v.Calculation_working_mode = a.Valuation.Calculation_working_mode; // Assuming 'a' is accessible field, Potential NullReferenceException
            v.Total_CIF = Convert.ToSingle(a.Valuation.Total_CIF); // Potential NullReferenceException, FormatException
            v.Total_cost = Convert.ToSingle(a.Valuation.Total_cost); // Potential NullReferenceException, FormatException

            // These call methods which need to be in their own partial classes
            Save_Valuation_Weight(v);
            Save_Gs_Invoice(v);
            Save_Gs_External_freight(v);
            Save_Total(v);
            //await DBaseDataModel.Instance.Savexcuda_Valuation(v).ConfigureAwait(false); // Assuming Savexcuda_Valuation exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}