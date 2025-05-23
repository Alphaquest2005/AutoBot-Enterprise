using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Traders, xcuda_Traders_Financial are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveTradersFinancial(xcuda_Traders t)
        {
            if (a.Traders.Financial.Financial_code.Text.Count == 0) return; // Assuming 'a' is accessible field, Potential NullReferenceException
            var f = t.xcuda_Traders_Financial; // Potential NullReferenceException
            if (f == null)
            {
                f = new xcuda_Traders_Financial(true) { Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Traders_Financial = f; // Potential NullReferenceException
            }
            if (a.Traders.Financial.Financial_code.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                f.Financial_code = a.Traders.Financial.Financial_code.Text[0]; // Potential NullReferenceException
            }
            if (a.Traders.Financial.Financial_name.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                f.Financial_name = a.Traders.Financial.Financial_name.Text[0]; // Potential NullReferenceException
            }
        }
    }
}