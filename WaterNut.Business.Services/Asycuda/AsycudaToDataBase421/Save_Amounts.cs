using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Financial, xcuda_Financial_Amounts are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Amounts(xcuda_Financial f)
        {
            var fa = f.xcuda_Financial_Amounts.FirstOrDefault(); // Potential NullReferenceException
            if (fa == null)
            {
                fa = new xcuda_Financial_Amounts(true) { Financial_Id = f.Financial_Id, TrackingState = TrackingState.Added };
                f.xcuda_Financial_Amounts.Add(fa); // Potential NullReferenceException
            }
            if (a.Financial.Amounts.Global_taxes != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                fa.Global_taxes = Convert.ToDecimal(a.Financial.Amounts.Global_taxes); // Potential NullReferenceException, FormatException
            // fa.Total_manual_taxes = a.Financial.Amounts.Total_manual_taxes; // Original code was commented out
            if (a.Financial.Amounts.Totals_taxes != "") // Assuming 'a' is accessible field, Potential NullReferenceException
                fa.Totals_taxes = Convert.ToDecimal(a.Financial.Amounts.Totals_taxes); // Potential NullReferenceException, FormatException
        }
    }
}