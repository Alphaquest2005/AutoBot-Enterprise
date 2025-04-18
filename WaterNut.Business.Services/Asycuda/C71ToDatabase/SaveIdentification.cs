using Asycuda421; // Assuming Value_declaration_formIdentification_segment is here
using System;
using System.Linq;
using ValuationDS.Business.Entities; // Assuming xC71_Identification_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void SaveIdentification(Value_declaration_formIdentification_segment aId, xC71_Identification_segment dId)
        {
            try
            {
                // Potential NullReferenceExceptions throughout if aId or its properties are null
                dId.Customs_Decision_Date = aId.Customs_Decision_Date.Text.FirstOrDefault();
                dId.Contract_Date = aId.Contract_Date.Text.FirstOrDefault();
                dId.No_7A = ToBool(aId.No_7A); // Calls ToBool, needs to be in its own partial class
                dId.No_7B = ToBool(aId.No_7B);
                dId.No_7C = ToBool(aId.No_7C);
                dId.No_8A = ToBool(aId.No_8A);
                dId.No_8B = ToBool(aId.No_8B);
                dId.No_9A = ToBool(aId.No_9A);
                dId.No_9B = ToBool(aId.No_9B);
                dId.Yes_7A = ToBool(aId.Yes_7A);
                dId.Yes_7B = ToBool(aId.Yes_7B);
                dId.Yes_7C = ToBool(aId.Yes_7C);
                dId.Yes_8A = ToBool(aId.Yes_8A);
                dId.Yes_8B = ToBool(aId.Yes_8B);
                dId.Yes_9A = ToBool(aId.Yes_9A);
                dId.Yes_9B = ToBool(aId.Yes_9B);

                // These call methods which need to be in their own partial classes
                SaveBuyer(aId.Buyer_segment, dId.xC71_Buyer_segment); // Potential NullReferenceException
                SaveDeclarant(aId.Declarant_segment, dId.xC71_Declarant_segment); // Potential NullReferenceException
                SaveSeller(aId.Seller_segment, dId.xC71_Seller_segment); // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}