using Asycuda421; // Assuming Value_declaration_formIdentification_segmentBuyer_segment is here
using System;
using ValuationDS.Business.Entities; // Assuming xC71_Buyer_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void ExportBuyer(Value_declaration_formIdentification_segmentBuyer_segment abuyer, xC71_Buyer_segment dbuyer)
        {
            try
            {
                // Ensure Text collections are initialized if null
                if (abuyer.Address.Text == null) abuyer.Address.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (abuyer.Code.Text == null) abuyer.Code.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (abuyer.Name.Text == null) abuyer.Name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();

                abuyer.Address.Text.Add(dbuyer.Address); // Potential NullReferenceException if dbuyer is null
                abuyer.Code.Text.Add(dbuyer.Code); // Potential NullReferenceException if dbuyer is null
                abuyer.Name.Text.Add(dbuyer.Name); // Potential NullReferenceException if dbuyer is null
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}