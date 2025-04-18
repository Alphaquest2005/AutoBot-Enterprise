using Asycuda421; // Assuming Value_declaration_formIdentification_segmentBuyer_segment is here
using System;
using System.Linq;
using ValuationDS.Business.Entities; // Assuming xC71_Buyer_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void SaveBuyer(Value_declaration_formIdentification_segmentBuyer_segment abuyer, xC71_Buyer_segment dbuyer)
        {
            try
            {
                dbuyer.Address = abuyer.Address.Text.FirstOrDefault(); // Potential NullReferenceException
                dbuyer.Code = abuyer.Code.Text.FirstOrDefault(); // Potential NullReferenceException
                dbuyer.Name = abuyer.Name.Text.FirstOrDefault(); // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}