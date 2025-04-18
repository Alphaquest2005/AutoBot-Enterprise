using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Goods_description are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Goods_description is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Goods_description(xcuda_Item di, ASYCUDAItem ai)
        {
            var g = di.xcuda_Goods_description;//.FirstOrDefault(); // Potential NullReferenceException
            if (g == null)
            {
                g = new xcuda_Goods_description(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                di.xcuda_Goods_description = g; // Potential NullReferenceException
            }
            g.Commercial_Description = ai.Goods_description.Commercial_Description.Text.FirstOrDefault(); // Potential NullReferenceException
            g.Country_of_origin_code = ai.Goods_description.Country_of_origin_code.Text.FirstOrDefault(); // Potential NullReferenceException
            g.Description_of_goods = ai.Goods_description.Description_of_goods.Text.FirstOrDefault(); // Potential NullReferenceException

            //await DIBaseDataModel.Instance.Savexcuda_Goods_description(g).ConfigureAwait(false); // Assuming Savexcuda_Goods_description exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}