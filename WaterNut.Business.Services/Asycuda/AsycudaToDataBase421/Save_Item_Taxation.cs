using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Taxation are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Taxation is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Taxation(xcuda_Item di, ASYCUDAItem ai)
        {
            var t = di.xcuda_Taxation.FirstOrDefault(); // Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Taxation(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                di.xcuda_Taxation.Add(t); // Potential NullReferenceException
            }

            //t.Counter_of_normal_mode_of_payment = ai.Taxation.Counter_of_normal_mode_of_payment
            //t.Displayed_item_taxes_amount = ai.Taxation.Displayed_item_taxes_amount;
            if (ai.Taxation.Item_taxes_amount != "") // Potential NullReferenceException
                t.Item_taxes_amount = Convert.ToSingle(ai.Taxation.Item_taxes_amount); // Potential NullReferenceException
            //t.Item_taxes_guaranted_amount = ai.Taxation.Item_taxes_guaranted_amount;
            if (ai.Taxation.Item_taxes_mode_of_payment.Text.Count > 0) // Potential NullReferenceException
                t.Item_taxes_mode_of_payment = ai.Taxation.Item_taxes_mode_of_payment.Text[0]; // Potential NullReferenceException

            // This calls Save_Taxation_line, which needs to be in its own partial class
            Save_Taxation_line(t, ai);

            //await DIBaseDataModel.Instance.Savexcuda_Taxation(t).ConfigureAwait(false); // Assuming Savexcuda_Taxation exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}