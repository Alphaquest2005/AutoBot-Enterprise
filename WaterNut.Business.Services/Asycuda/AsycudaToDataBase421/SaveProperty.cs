using System.Threading.Tasks;
using DocumentDS.Business.Entities; // Assuming xcuda_Property is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Property is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveProperty()
        {
            var p = da.Document.xcuda_Property;//.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException

            if (p == null)
            {
                p = new xcuda_Property(true) { TrackingState = TrackingState.Added };
                da.Document.xcuda_Property = p; // Potential NullReferenceException
                // da.xcuda_Property.Add(p);
            }
            // p.Date_of_declaration = a.Property.Date_of_declaration.ToString(); // Original code was commented out
            // This calls SaveNbers, which needs to be in its own partial class
            SaveNbers(p);
            //await DBaseDataModel.Instance.Savexcuda_Property(p).ConfigureAwait(false); // Assuming Savexcuda_Property exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}