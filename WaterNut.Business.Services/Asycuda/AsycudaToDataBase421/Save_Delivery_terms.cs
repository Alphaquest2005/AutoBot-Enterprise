using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Transport, xcuda_Delivery_terms are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Delivery_terms(xcuda_Transport t)
        {
            var d = t.xcuda_Delivery_terms.FirstOrDefault(); // Potential NullReferenceException
            if (d == null)
            {
                d = new xcuda_Delivery_terms(true) { Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Delivery_terms.Add(d); // Potential NullReferenceException
            }
            if (a.Transport.Delivery_terms.Code.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                d.Code = a.Transport.Delivery_terms.Code.Text[0]; // Potential NullReferenceException
            //d.Place = a.Transport.Delivery_terms.Place // Original code was commented out
        }
    }
}