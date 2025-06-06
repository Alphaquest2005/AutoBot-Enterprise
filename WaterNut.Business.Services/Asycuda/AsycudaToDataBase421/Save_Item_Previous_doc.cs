using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Previous_doc are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Previous_doc is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Previous_doc(xcuda_Item di, ASYCUDAItem ai)
        {
            var pd = di.xcuda_Previous_doc; // Potential NullReferenceException
            if (pd == null)
            {
                pd = new xcuda_Previous_doc(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                // di.xcuda_Previous_doc.Add(pd);
                di.xcuda_Previous_doc = pd; // Potential NullReferenceException
            }
            pd.Summary_declaration = ai.Previous_doc.Summary_declaration.Text.FirstOrDefault(); // Potential NullReferenceException
            if (da.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber == null && ai.Previous_doc.Summary_declaration != null) // Assuming 'da' is accessible field, Potential NullReferenceException
                da.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber = ai.Previous_doc.Summary_declaration.Text.FirstOrDefault(); // Potential NullReferenceException

            //await DIBaseDataModel.Instance.Savexcuda_Previous_doc(pd).ConfigureAwait(false); // Assuming Savexcuda_Previous_doc exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}