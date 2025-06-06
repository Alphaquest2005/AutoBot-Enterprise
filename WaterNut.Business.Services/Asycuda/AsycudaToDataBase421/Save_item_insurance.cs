using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Valuation_item, xcuda_item_insurance are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_item_insurance(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            //use same implementation as external freight
            var i = vi.xcuda_item_insurance; // Potential NullReferenceException
            if (i == null)
            {
                i = new xcuda_item_insurance(true) { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_item_insurance = i; // Potential NullReferenceException
            }
            if (ai.Valuation_item.item_insurance.Amount_foreign_currency != "") // Potential NullReferenceException
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.item_insurance.Amount_foreign_currency); // Potential NullReferenceException
            if (ai.Valuation_item.item_insurance.Amount_national_currency != "") // Potential NullReferenceException
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.item_insurance.Amount_national_currency); // Potential NullReferenceException

            i.Currency_name = ai.Valuation_item.item_insurance.Currency_code.Text.FirstOrDefault(); // Potential NullReferenceException

            if (ai.Valuation_item.item_insurance.Currency_rate != "") // Potential NullReferenceException
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.item_insurance.Currency_rate); // Potential NullReferenceException
        }
    }
}