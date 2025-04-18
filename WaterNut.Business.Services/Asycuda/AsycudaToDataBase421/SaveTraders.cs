using System.Threading.Tasks;
using DocumentDS.Business.Entities; // Assuming xcuda_Traders is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Traders is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveTraders()
        {
            var t = da.Document.xcuda_Traders; // Assuming 'da' is accessible field, Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Traders(true) { Traders_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                da.Document.xcuda_Traders = t; // Potential NullReferenceException
            }
            // These call methods which need to be in their own partial classes
            SaveExporter(t);
            SaveConsignee(t);
            SaveTradersFinancial(t);
            //await DBaseDataModel.Instance.Savexcuda_Traders(t).ConfigureAwait(false); // Assuming Savexcuda_Traders exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}