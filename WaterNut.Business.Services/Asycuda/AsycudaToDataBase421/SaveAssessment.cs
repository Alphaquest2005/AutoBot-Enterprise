using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification, xcuda_Assessment are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveAssessment(xcuda_Identification di)
        {
            var r = di.xcuda_Assessment; // Potential NullReferenceException
            if (r == null)
            {
                r = new xcuda_Assessment(true) { ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added };
                di.xcuda_Assessment = r; // Potential NullReferenceException
                // di.xcuda_Registration.Add(r); // Original code had this commented out, likely a copy-paste error
            }
            if (a.Identification.Assessment.Date != "1/1/0001") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Date = a.Identification.Assessment.Date; // Potential NullReferenceException
            if (a.Identification.Assessment.Number != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Number = a.Identification.Assessment.Number; // Potential NullReferenceException
        }
    }
}