using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Warehouse is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Warehouse is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Warehouse()
        {
            var w = da.Document.xcuda_Warehouse.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException
            if (w == null)
            {
                w = new xcuda_Warehouse(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                da.Document.xcuda_Warehouse.Add(w); // Potential NullReferenceException
            }
            w.Identification = a.Warehouse.Identification.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            w.Delay = Convert.ToInt32(a.Warehouse.Delay == "" ? "0" : a.Warehouse.Delay); // Assuming 'a' is accessible field, Potential NullReferenceException
            //await DBaseDataModel.Instance.Savexcuda_Warehouse(w).ConfigureAwait(false); // Assuming Savexcuda_Warehouse exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}