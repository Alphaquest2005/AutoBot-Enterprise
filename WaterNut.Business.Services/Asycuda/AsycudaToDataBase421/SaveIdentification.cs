using System.Threading.Tasks;
using DocumentDS.Business.Entities; // Assuming xcuda_Identification is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Identification is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveIdentification()
        {
            var di = da.Document.xcuda_Identification;//.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException
            if (di == null)
            {
                di = new xcuda_Identification(true) { TrackingState = TrackingState.Added };
                da.Document.xcuda_Identification = di; // Potential NullReferenceException
                // da.xcuda_Identification.Add(di);
            }

            // These call methods which need to be in their own partial classes
            SaveManifestReferenceNumber(di);
            SaveOfficeSegment(di);
            SaveRegistration(di);
            SaveAssessment(di);
            SaveReceipt(di);
            await SaveType(di).ConfigureAwait(false);

            //await DBaseDataModel.Instance.Savexcuda_Identification(di).ConfigureAwait(false); // Assuming Savexcuda_Identification exists
        }
    }
}