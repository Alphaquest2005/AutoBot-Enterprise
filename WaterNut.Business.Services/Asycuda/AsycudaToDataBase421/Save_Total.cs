using System;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Valuation, xcuda_Total are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Total(xcuda_Valuation v)
        {
            var t = v.xcuda_Total; // Potential NullReferenceException
            if (t == null)
            {
                t = new xcuda_Total(true) { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Total = t; // Potential NullReferenceException
            }
            t.Total_invoice = Convert.ToSingle(a.Valuation.Total.Total_invoice); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            t.Total_weight = Convert.ToSingle(a.Valuation.Total.Total_weight); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
        }
    }
}