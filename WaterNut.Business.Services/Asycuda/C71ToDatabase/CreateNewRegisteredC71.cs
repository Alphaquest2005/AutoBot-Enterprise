using System.Collections.Generic;
using TrackableEntities; // Assuming TrackingState is here
using ValuationDS.Business.Entities; // Assuming Registered, xC71_Identification_segment, xC71_Buyer_segment, xC71_Seller_segment, xC71_Declarant_segment, xC71_Item are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private Registered CreateNewRegisteredC71()
        {
            return new Registered(true)
            {
                xC71_Identification_segment = new xC71_Identification_segment(true)
                {
                    xC71_Buyer_segment = new xC71_Buyer_segment(true) { TrackingState = TrackingState.Added },
                    xC71_Seller_segment = new xC71_Seller_segment(true) { TrackingState = TrackingState.Added },
                    xC71_Declarant_segment = new xC71_Declarant_segment(true) { TrackingState = TrackingState.Added },
                    TrackingState = TrackingState.Added
                },
                xC71_Item = new List<xC71_Item>(),
                TrackingState = TrackingState.Added
            };
        }
    }
}