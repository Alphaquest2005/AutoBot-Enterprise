using Asycuda421; // Assuming Licence is here
using LicenseDS.Business.Entities; // Assuming xLIC_License is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private Licence DatabaseToLicence(xLIC_License lic)
        {
            var aLic = new Licence();
            // These call methods which need to be in their own partial classes
            ExportGeneralInfo(aLic.General_segment, lic.xLIC_General_segment); // Potential NullReferenceException
            ExportItems(aLic.Lic_item_segment, lic); // Potential NullReferenceException
            return aLic;
        }
    }
}