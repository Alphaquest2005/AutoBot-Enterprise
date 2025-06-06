using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Country, xcuda_Destination are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for BaseDataModel if SaveDocumentCT is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveDestination(xcuda_Country c)
        {
            var des = c.xcuda_Destination; // Potential NullReferenceException
            if (des == null)
            {
                des = new xcuda_Destination(true) { Country_Id = c.Country_Id, TrackingState = TrackingState.Added };
                c.xcuda_Destination = des; // Potential NullReferenceException
                des.xcuda_Country = c;
                //await BaseDataModel.Instance.SaveDocumentCT(da).ConfigureAwait(false); // Assuming SaveDocumentCT exists
            }
            des.Destination_country_code = a.General_information.Country.Destination.Destination_country_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            if (a.General_information.Country.Destination.Destination_country_name != null) // Assuming 'a' is accessible field, Potential NullReferenceException
                des.Destination_country_name = a.General_information.Country.Destination.Destination_country_name.Text.FirstOrDefault(); // Potential NullReferenceException
            //Exp.Export_country_region = a.General_information.Country.Export.Export_country_region.; // Original code was commented out
        }
    }
}