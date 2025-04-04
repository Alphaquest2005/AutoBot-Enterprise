using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Country, xcuda_Export are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveExport(xcuda_Country c)
        {
            var Exp = c.xcuda_Export; // Potential NullReferenceException
            if (Exp == null)
            {
                Exp = new xcuda_Export(true) { Country_Id = c.Country_Id, TrackingState = TrackingState.Added };
                c.xcuda_Export = Exp; // Potential NullReferenceException
            }
            Exp.Export_country_code = a.General_information.Country.Export.Export_country_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            Exp.Export_country_name = a.General_information.Country.Export.Export_country_name.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            //Exp.Export_country_region = a.General_information.Country.Export.Export_country_region.; // Original code was commented out
        }
    }
}