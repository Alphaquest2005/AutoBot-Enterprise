using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Valuation, xcuda_Gs_external_freight are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Gs_External_freight(xcuda_Valuation v)
        {
            var gf = v.xcuda_Gs_external_freight; // Potential NullReferenceException
            if (gf == null)
            {
                gf = new xcuda_Gs_external_freight(true) { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Gs_external_freight = gf; // Potential NullReferenceException
            }

            gf.Amount_foreign_currency = Convert.ToSingle(a.Valuation.Gs_external_freight.Amount_foreign_currency); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            gf.Amount_national_currency = Convert.ToSingle(a.Valuation.Gs_external_freight.Amount_national_currency); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            gf.Currency_code = a.Valuation.Gs_external_freight.Currency_code.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            gf.Currency_name = a.Valuation.Gs_external_freight.Currency_name.Text.FirstOrDefault(); // Assuming 'a' is accessible field, Potential NullReferenceException
            gf.Currency_rate = Convert.ToSingle(a.Valuation.Gs_external_freight.Currency_rate); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
        }
    }
}