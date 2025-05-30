using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Valuation_item, xcuda_Item_Invoice are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Item_Invoice(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var i = vi.xcuda_Item_Invoice; // Potential NullReferenceException
            if (i == null)
            {
                i = new xcuda_Item_Invoice(true) { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_Item_Invoice = i; // Potential NullReferenceException
            }
            if (ai.Valuation_item.Item_Invoice.Amount_foreign_currency != "") // Potential NullReferenceException
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Amount_foreign_currency); // Potential NullReferenceException
            if (ai.Valuation_item.Item_Invoice.Amount_national_currency != "") // Potential NullReferenceException
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Amount_national_currency); // Potential NullReferenceException
            if (ai.Valuation_item.Item_Invoice.Currency_code?.Text?.FirstOrDefault() != null) // Potential NullReferenceException
                i.Currency_code = ai.Valuation_item.Item_Invoice.Currency_code.Text.FirstOrDefault(); // Potential NullReferenceException
            if (ai.Valuation_item.Item_Invoice.Currency_rate != "") // Potential NullReferenceException
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Currency_rate); // Potential NullReferenceException
        }
    }
}