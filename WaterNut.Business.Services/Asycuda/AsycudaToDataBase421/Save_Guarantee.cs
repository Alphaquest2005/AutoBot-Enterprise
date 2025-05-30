using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Financial, xcuda_Financial_Guarantee are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Guarantee(xcuda_Financial f)
        {
            var g = f.xcuda_Financial_Guarantee.FirstOrDefault(); // Potential NullReferenceException
            if (g == null)
            {
                g = new xcuda_Financial_Guarantee(true) { Financial_Id = f.Financial_Id, TrackingState = TrackingState.Added };
                f.xcuda_Financial_Guarantee.Add(g); // Potential NullReferenceException
            }
            if (a.Financial.Guarantee.Amount != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                g.Amount = Convert.ToDecimal(a.Financial.Guarantee.Amount); // Potential NullReferenceException, FormatException
            //  g.Date = a.Financial.Guarantee.Date; // Original code was commented out
        }
    }
}