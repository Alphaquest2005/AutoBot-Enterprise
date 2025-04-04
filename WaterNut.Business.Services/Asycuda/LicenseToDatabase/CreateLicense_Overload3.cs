using System.Collections.Generic;
using LicenseDS.Business.Entities; // Assuming xLIC_License, xLIC_General_segment, xLIC_Lic_item_segment are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private static xLIC_License CreateLicense()
        {
            xLIC_License ndoc;
            ndoc = new xLIC_License(true)
            {
                xLIC_General_segment = new xLIC_General_segment(true) {TrackingState = TrackingState.Added},
                xLIC_Lic_item_segment = new List<xLIC_Lic_item_segment>(),
                TrackingState = TrackingState.Added
            };
            return ndoc;
        }
    }
}