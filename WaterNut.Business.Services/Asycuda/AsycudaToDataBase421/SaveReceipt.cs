using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification, xcuda_receipt are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveReceipt(xcuda_Identification di)
        {
            var r = di.xcuda_receipt; // Potential NullReferenceException
            if (r == null)
            {
                r = new xcuda_receipt(true) { ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added };
                di.xcuda_receipt = r; // Potential NullReferenceException
                // di.xcuda_Registration.Add(r); // Original code had this commented out, likely a copy-paste error
            }
            if (a.Identification.receipt.Date != "1/1/0001") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Date = a.Identification.receipt.Date; // Potential NullReferenceException
            if (a.Identification.receipt.Number != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Number = a.Identification.receipt.Number; // Potential NullReferenceException
        }
    }
}