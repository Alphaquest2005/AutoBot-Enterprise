using System;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification, xcuda_Registration are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveRegistration(xcuda_Identification di)
        {
            var r = di.xcuda_Registration; // Potential NullReferenceException
            if (r == null)
            {
                r = new xcuda_Registration(true) { ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added };
                di.xcuda_Registration = r; // Potential NullReferenceException
                // di.xcuda_Registration.Add(r);
            }
            if (a.Identification.Registration.Date != "1/1/0001") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Date = DateTime.Parse(a.Identification.Registration.Date); // Potential NullReferenceException, FormatException
            if (a.Identification.Registration.Number != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                r.Number = a.Identification.Registration.Number; // Potential NullReferenceException
        }
    }
}