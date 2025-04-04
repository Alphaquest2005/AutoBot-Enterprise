using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Transport, xcuda_Border_office are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Border_office(xcuda_Transport t)
        {
            var bo = t.xcuda_Border_office.FirstOrDefault(); // Potential NullReferenceException
            if (bo == null)
            {
                bo = new xcuda_Border_office(true) { Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Border_office.Add(bo); // Potential NullReferenceException
            }
            if (a.Transport.Border_office.Code.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                bo.Code = a.Transport.Border_office.Code.Text[0]; // Potential NullReferenceException

            if (a.Transport.Border_office.Name.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                bo.Name = a.Transport.Border_office.Name.Text[0]; // Potential NullReferenceException
        }
    }
}