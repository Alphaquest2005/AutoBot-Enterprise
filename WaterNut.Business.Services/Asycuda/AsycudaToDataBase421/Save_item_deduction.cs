using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Valuation_item, xcuda_item_deduction are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_item_deduction(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            //use same implementation as external freight
            var i = vi.xcuda_item_deduction; // Potential NullReferenceException
            if (i == null)
            {
                i = new xcuda_item_deduction(true) { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_item_deduction = i; // Potential NullReferenceException
            }
            if (ai.Valuation_item.item_deduction.Amount_foreign_currency != "") // Potential NullReferenceException
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.item_deduction.Amount_foreign_currency); // Potential NullReferenceException
            if (ai.Valuation_item.item_deduction.Amount_national_currency != "") // Potential NullReferenceException
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.item_deduction.Amount_national_currency); // Potential NullReferenceException

            i.Currency_name= ai.Valuation_item.item_deduction.Currency_code.Text.FirstOrDefault(); // Potential NullReferenceException

            if (ai.Valuation_item.item_deduction.Currency_rate != "") // Potential NullReferenceException
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.item_deduction.Currency_rate); // Potential NullReferenceException
        }
    }
}