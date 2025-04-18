using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification, xcuda_Type are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveType(xcuda_Identification di)
        {
            var t = di.xcuda_Type; // Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Type(true) { TrackingState = TrackingState.Added };
                di.xcuda_Type = t; // Potential NullReferenceException
            }

            t.Declaration_gen_procedure_code = a.Identification.Type.Declaration_gen_procedure_code; // Assuming 'a' is accessible field, Potential NullReferenceException
            t.Type_of_declaration = a.Identification.Type.Type_of_declaration; // Assuming 'a' is accessible field, Potential NullReferenceException

            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}