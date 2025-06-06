using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_General_information, xcuda_Country are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveCountry(xcuda_General_information gi)
        {
            var c = gi.xcuda_Country; // Potential NullReferenceException
            if (c == null)
            {
                c = new xcuda_Country(true) {Country_Id = gi.ASYCUDA_Id, TrackingState = TrackingState.Added };
                gi.xcuda_Country = c; // Potential NullReferenceException
            }
            c.Country_first_destination = a.General_information.Country.Country_first_destination.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            c.Country_of_origin_name = a.General_information.Country.Country_of_origin_name; // Assuming 'a' is accessible field, Potential NullReferenceException
            c.Trading_country = a.General_information.Country.Trading_country.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            // These call methods which need to be in their own partial classes
            SaveExport(c);
            SaveDestination(c);
        }
    }
}