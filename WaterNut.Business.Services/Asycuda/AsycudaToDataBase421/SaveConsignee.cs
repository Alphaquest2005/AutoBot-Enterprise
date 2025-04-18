using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Traders, xcuda_Consignee are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveConsignee(xcuda_Traders t)
        {
            var c = t.xcuda_Consignee; // Potential NullReferenceException
            if (c == null)
            {
                c = new xcuda_Consignee(true) { Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Consignee = c; // Potential NullReferenceException
            }
            if (a.Traders.Consignee.Consignee_code.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                c.Consignee_code = a.Traders.Consignee.Consignee_code.Text[0]; // Potential NullReferenceException
            }
            if (a.Traders.Consignee.Consignee_name.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                c.Consignee_name = a.Traders.Consignee.Consignee_name.Text[0]; // Potential NullReferenceException
            }
        }
    }
}