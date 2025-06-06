using System.Collections.Generic;
using LicenseDS.Business.Entities; // Assuming Registered, xLIC_General_segment, xLIC_Lic_item_segment are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private static Registered CreateRegisteredLicense(string regNumber, string file)
        {
            Registered ndoc;
            ndoc = new Registered(true)
            {
                RegistrationNumber = regNumber,
                SourceFile = file,
                xLIC_General_segment = new xLIC_General_segment(true) { TrackingState = TrackingState.Added },
                xLIC_Lic_item_segment = new List<xLIC_Lic_item_segment>(),
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, // Assuming CurrentApplicationSettings exists
                TrackingState = TrackingState.Added
            };
            return ndoc;
        }
    }
}