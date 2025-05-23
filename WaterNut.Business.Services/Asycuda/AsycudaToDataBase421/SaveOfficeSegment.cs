using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification, xcuda_Office_segment are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveOfficeSegment(xcuda_Identification di)
        {
            var o = di.xcuda_Office_segment;//.FirstOrDefault(); // Potential NullReferenceException
            if (o == null)
            {
                o = new xcuda_Office_segment(true) { ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added };
                di.xcuda_Office_segment = o; // Potential NullReferenceException
                // di.xcuda_Office_segment.Add(o);
            }
            o.Customs_clearance_office_code = a.Identification.Office_segment.Customs_clearance_office_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            o.Customs_Clearance_office_name = a.Identification.Office_segment.Customs_Clearance_office_name.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
        }
    }
}