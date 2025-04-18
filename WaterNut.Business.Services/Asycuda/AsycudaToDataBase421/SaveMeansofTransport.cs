using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Transport, xcuda_Means_of_transport are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveMeansofTransport(xcuda_Transport t)
        {
            var m = t.xcuda_Means_of_transport.FirstOrDefault(); // Potential NullReferenceException
            if (m == null)
            {
                m = new xcuda_Means_of_transport(true) { Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Means_of_transport.Add(m); // Potential NullReferenceException
            }

            // These call methods which need to be in their own partial classes
            SaveDepartureArrivalInformation(m);
            SaveBorderInformation(m);
            //m.Inland_mode_of_transport = a.Transport.Means_of_transport.Inland_mode_of_transport. // Original code was commented out
        }
    }
}