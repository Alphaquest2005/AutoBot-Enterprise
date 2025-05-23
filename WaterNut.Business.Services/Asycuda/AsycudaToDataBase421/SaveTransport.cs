using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Transport is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Transport is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveTransport()
        {
            var t = da.Document.xcuda_Transport.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Transport(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                da.Document.xcuda_Transport.Add(t); // Potential NullReferenceException
            }
            t.Container_flag = a.Transport.Container_flag; // Assuming 'a' is accessible field, Potential NullReferenceException
            t.Single_waybill_flag = a.Transport.Single_waybill_flag; // Assuming 'a' is accessible field, Potential NullReferenceException
            if (a.Transport.Location_of_goods.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                t.Location_of_goods = a.Transport.Location_of_goods.Text[0]; // Potential NullReferenceException
            }
            // These call methods which need to be in their own partial classes
            SaveMeansofTransport(t);
            Save_Delivery_terms(t);
            Save_Border_office(t);
            //await DBaseDataModel.Instance.Savexcuda_Transport(t).ConfigureAwait(false); // Assuming Savexcuda_Transport exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}