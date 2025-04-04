using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_General_information is here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.Business.Entities; // Assuming DocumentCT is here
// Need using for DBaseDataModel if Savexcuda_General_information is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveGeneralInformation()
        {
            var gi = da.Document.xcuda_General_information; // Assuming 'da' is accessible field, Potential NullReferenceException
            if (gi == null)
            {
                gi = new xcuda_General_information() {ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                da.Document.xcuda_General_information = gi; // Potential NullReferenceException
            }
            gi.Value_details = a.General_information.Value_details; // Assuming 'a' is accessible field, Potential NullReferenceException
            gi.Comments_free_text = a.General_information.Comments_free_text.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException

            // These call methods which need to be in their own partial classes
            SetEffectiveAssessmentDate(da, gi.Comments_free_text);
            SaveCountry(gi);
            //await DBaseDataModel.Instance.Savexcuda_General_information(gi).ConfigureAwait(false); // Assuming Savexcuda_General_information exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}