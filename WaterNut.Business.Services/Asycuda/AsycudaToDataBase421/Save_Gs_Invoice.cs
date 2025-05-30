using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Valuation, xcuda_Gs_Invoice are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Gs_Invoice(xcuda_Valuation v)
        {
            var gi = v.xcuda_Gs_Invoice; // Potential NullReferenceException
            if (gi == null)
            {
                gi = new xcuda_Gs_Invoice(true) { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Gs_Invoice = gi; // Potential NullReferenceException
            }

            gi.Amount_foreign_currency = Convert.ToSingle(a.Valuation.Gs_Invoice.Amount_foreign_currency); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            gi.Amount_national_currency = Convert.ToSingle(a.Valuation.Gs_Invoice.Amount_national_currency); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            gi.Currency_code = a.Valuation.Gs_Invoice.Currency_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            gi.Currency_rate = Convert.ToSingle(a.Valuation.Gs_Invoice.Currency_rate); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            if (a.Valuation.Gs_Invoice.Currency_name.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                gi.Currency_name = a.Valuation.Gs_Invoice.Currency_name.Text[0]; // Assuming 'a' is accessible field, Potential NullReferenceException
        }
    }
}