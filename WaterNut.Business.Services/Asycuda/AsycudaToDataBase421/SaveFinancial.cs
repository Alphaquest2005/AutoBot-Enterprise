using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Financial is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Financial is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveFinancial()
        {
            var f = da.Document.xcuda_Financial.FirstOrDefault(); // Assuming 'da' is accessible field, Potential NullReferenceException
            if (f == null)
            {
                f = new xcuda_Financial(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
               // await DBaseDataModel.Instance.Savexcuda_Financial(f).ConfigureAwait(false); // Assuming Savexcuda_Financial exists
                da.Document.xcuda_Financial.Add(f); // Potential NullReferenceException
            }
            if (a.Financial.Deffered_payment_reference.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                f.Deffered_payment_reference = a.Financial.Deffered_payment_reference.Text[0]; // Potential NullReferenceException

            f.Mode_of_payment = a.Financial.Mode_of_payment; // Potential NullReferenceException

            // These call methods which need to be in their own partial classes
            Save_Amounts(f);
            Save_Guarantee(f);
                //await DBaseDataModel.Instance.Savexcuda_Financial(f).ConfigureAwait(false); // Assuming Savexcuda_Financial exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}