using System.Linq;
using System.Threading.Tasks;
using DocumentItemDS.Business.Entities; // Assuming xcuda_PreviousItem, EntryPreviousItems are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if SaveEntryPreviousItems is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task LinkPi2Item(int itemId, xcuda_PreviousItem pi)
        {
            if (pi.xcuda_Items.Any(x => x.Item_Id == itemId)) return; // Potential NullReferenceException if xcuda_Items is null
            var ep = new EntryPreviousItems(true)
            {
                Item_Id = itemId,
                PreviousItem_Id = pi.PreviousItem_Id,
                TrackingState = TrackingState.Added
            };
            // //await DIBaseDataModel.Instance.SaveEntryPreviousItems(ep).ConfigureAwait(false); // Assuming SaveEntryPreviousItems exists
            pi.xcuda_Items.Add(ep); // Potential NullReferenceException if xcuda_Items is null
        }
    }
}