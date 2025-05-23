using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Means_of_transport, xcuda_Border_information are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveBorderInformation(xcuda_Means_of_transport m)
        {
            var d = m.xcuda_Border_information.FirstOrDefault(); // Potential NullReferenceException
            if (d == null)
            {
                d = new xcuda_Border_information(true) { Means_of_transport_Id = m.Means_of_transport_Id, TrackingState = TrackingState.Added };
                m.xcuda_Border_information.Add(d); // Potential NullReferenceException
            }
            //if (a.Transport.Means_of_transport.Border_information.Nationality.ToString() != null) // Assuming 'a' is accessible field, Potential NullReferenceException
            //    d.Nationality = a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text[0]; // Potential NullReferenceException

            //if (a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            //    d.Identity = a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text[0]; // Potential NullReferenceException
            if (a.Transport.Means_of_transport.Border_information.Mode.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                d.Mode = Convert.ToInt32(a.Transport.Means_of_transport.Border_information.Mode.Text[0]); // Potential NullReferenceException, FormatException
        }
    }
}