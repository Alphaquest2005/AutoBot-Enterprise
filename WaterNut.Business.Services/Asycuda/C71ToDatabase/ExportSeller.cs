using Asycuda421; // Assuming Value_declaration_formIdentification_segmentSeller_segment is here
using System;
using ValuationDS.Business.Entities; // Assuming xC71_Seller_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void ExportSeller(Value_declaration_formIdentification_segmentSeller_segment aseller, xC71_Seller_segment dseller)
        {
            try
            {
                // Ensure Text collections are initialized if null
                if (aseller.Address.Text == null) aseller.Address.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aseller.Name.Text == null) aseller.Name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();

                aseller.Address.Text.Add(dseller.Address); // Potential NullReferenceException if dseller is null
                aseller.Name.Text.Add(dseller.Name); // Potential NullReferenceException if dseller is null
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}