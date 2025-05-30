using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Tarification, xcuda_Item, xcuda_HScode, xcuda_Inventory_Item are here
using InventoryDS.Business.Entities; // Assuming InventoryItem is here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_HScode(xcuda_Tarification t,xcuda_Item di, ASYCUDAItem ai)
        {
            var h = t.xcuda_HScode;//.FirstOrDefault(); // Potential NullReferenceException
            if (h == null)
            {
                h = new xcuda_HScode(true) { Item_Id = t.Item_Id, TrackingState = TrackingState.Added };
                t.xcuda_HScode = h; // Potential NullReferenceException
            }

            h.Commodity_code = ai.Tarification.HScode.Commodity_code.Text.FirstOrDefault() ?? "NULL ON IMPORT"; // Potential NullReferenceException
            h.Precision_1 = ai.Tarification.HScode.Precision_1.Text.FirstOrDefault(); // Potential NullReferenceException
            if (ai.Tarification.HScode.Precision_4.Text.FirstOrDefault() != null) // Potential NullReferenceException
            {
                h.Precision_4 = ai.Tarification.HScode.Precision_4.Text.FirstOrDefault(); // Potential NullReferenceException
            }
            else
            {
                //if (!NoMessages)
                //    throw new ApplicationException(string.Format("Null Product Code on Line{0}", di.LineNumber));
            }

            // This calls SaveInventoryItem, which needs to be in its own partial class
            var i = await SaveInventoryItem(ai).ConfigureAwait(false);
            if(i != null)
            h.xcuda_Inventory_Item = new xcuda_Inventory_Item(true)
            {
                TrackingState = TrackingState.Added,
                InventoryItemId = i.Id
            };
        }
    }
}