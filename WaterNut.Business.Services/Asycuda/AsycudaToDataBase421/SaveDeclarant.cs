using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Declarant is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Declarant is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveDeclarant()
        {
            try
            {
                var d = da.Document.xcuda_Declarant;//.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException
                if (d == null)
                {
                    da.Document.xcuda_Declarant = new xcuda_Declarant(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                    d = da.Document.xcuda_Declarant; // Potential NullReferenceException
                    //da.xcuda_Declarant.Add(d);
                }

                d.Declarant_name = a.Declarant.Declarant_name.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
                d.Declarant_representative = a.Declarant.Declarant_representative.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
                d.Declarant_code = a.Declarant.Declarant_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException

                //if(a.Declarant.Reference.Number.Text.Count > 0)
                d.Number = a.Declarant.Reference.Number.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
                //await DBaseDataModel.Instance.Savexcuda_Declarant(d).ConfigureAwait(false); // Assuming Savexcuda_Declarant exists
            }
            catch (Exception Ex)
            {
                throw new Exception("Declarant fail to import - " + a.Declarant.Reference.Number); // Assuming 'a' is accessible field, Potential NullReferenceException
            }
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}