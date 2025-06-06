using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Suppliers_link are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Suppliers_link is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Suppliers_link(xcuda_Item di, ASYCUDAItem ai)
        {
            var sl = di.xcuda_Suppliers_link.FirstOrDefault(); // Potential NullReferenceException
            if (sl == null)
            {
                sl = new xcuda_Suppliers_link(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                di.xcuda_Suppliers_link.Add(sl); // Potential NullReferenceException
            }

            sl.Suppliers_link_code = ai.Suppliers_link.Suppliers_link_code; // Potential NullReferenceException
            //await DIBaseDataModel.Instance.Savexcuda_Suppliers_link(sl).ConfigureAwait(false); // Assuming Savexcuda_Suppliers_link exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}