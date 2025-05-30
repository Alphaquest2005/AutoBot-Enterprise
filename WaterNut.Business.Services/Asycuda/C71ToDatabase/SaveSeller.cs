using Asycuda421; // Assuming Value_declaration_formIdentification_segmentSeller_segment is here
using System;
using System.Linq;
using ValuationDS.Business.Entities; // Assuming xC71_Seller_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void SaveSeller(Value_declaration_formIdentification_segmentSeller_segment aseller, xC71_Seller_segment dseller)
        {
            try
            {
                dseller.Address = aseller.Address.Text.FirstOrDefault(); // Potential NullReferenceException
                dseller.Name = aseller.Name.Text.FirstOrDefault(); // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}