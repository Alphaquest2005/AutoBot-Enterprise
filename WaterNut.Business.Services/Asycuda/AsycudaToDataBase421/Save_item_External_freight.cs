using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Valuation_item, xcuda_item_external_freight are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_item_External_freight(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var i = vi.xcuda_item_external_freight; // Potential NullReferenceException
            if (i == null)
            {
                i = new xcuda_item_external_freight(true) { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_item_external_freight = i; // Potential NullReferenceException
            }
            if (ai.Valuation_item.item_external_freight.Amount_foreign_currency != "") // Potential NullReferenceException
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.item_external_freight.Amount_foreign_currency); // Potential NullReferenceException
            if (ai.Valuation_item.item_external_freight.Amount_national_currency != "") // Potential NullReferenceException
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.item_external_freight.Amount_national_currency); // Potential NullReferenceException

            i.Currency_code = ai.Valuation_item.item_external_freight.Currency_code.Text.FirstOrDefault(); // Potential NullReferenceException

            if (ai.Valuation_item.item_external_freight.Currency_rate != "") // Potential NullReferenceException
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.item_external_freight.Currency_rate); // Potential NullReferenceException
        }
    }
}