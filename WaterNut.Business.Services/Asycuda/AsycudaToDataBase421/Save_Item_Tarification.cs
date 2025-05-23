using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Tarification are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Tarification is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Tarification(xcuda_Item di, ASYCUDAItem ai)
        {
            var t = di.xcuda_Tarification;//.FirstOrDefault(); // Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Tarification(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                di.xcuda_Tarification = t; // Potential NullReferenceException
            }

            t.Extended_customs_procedure = ai.Tarification.Extended_customs_procedure.Text.FirstOrDefault(); // Potential NullReferenceException
            t.National_customs_procedure = ai.Tarification.National_customs_procedure.Text.FirstOrDefault(); // Potential NullReferenceException
            if (ai.Tarification.Item_price != "") // Potential NullReferenceException
                t.Item_price = Convert.ToSingle(ai.Tarification.Item_price); // Potential NullReferenceException
            if (ai.Tarification.Value_item.Text.Count > 0) // Potential NullReferenceException
                t.Value_item = ai.Tarification.Value_item.Text[0]; // Potential NullReferenceException

            // These call methods which need to be in their own partial classes
            Save_Supplementary_unit(t, ai);

            if (ai.Tarification.Attached_doc_item.Text.Count > 0) // Potential NullReferenceException
                t.Attached_doc_item = ai.Tarification.Attached_doc_item.Text[0]; // Potential NullReferenceException

            await SaveCustomsProcedure(t).ConfigureAwait(false);
            await Save_HScode(t, di,ai).ConfigureAwait(false);

            //await DIBaseDataModel.Instance.Savexcuda_Tarification(t).ConfigureAwait(false); // Assuming Savexcuda_Tarification exists
        }
    }
}