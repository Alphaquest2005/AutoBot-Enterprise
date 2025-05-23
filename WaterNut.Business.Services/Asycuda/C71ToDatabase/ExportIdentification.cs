using Asycuda421; // Assuming Value_declaration_formIdentification_segment is here
using System;
using ValuationDS.Business.Entities; // Assuming xC71_Identification_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void ExportIdentification(Value_declaration_formIdentification_segment aId, xC71_Identification_segment dId)
        {
            try
            {
                if(aId == null) aId = new Value_declaration_formIdentification_segment();
                //aId.Customs_Decision_Date = dId.Customs_Decision_Date; // Original code commented out
                //aId.Contract_Date = dId.Contract_Date; // Original code commented out
                aId.No_7A = dId.No_7A.ToString(); // Potential NullReferenceException
                aId.No_7B = dId.No_7B.ToString(); // Potential NullReferenceException
                aId.No_7C = dId.No_7C.ToString(); // Potential NullReferenceException
                aId.No_8A = dId.No_8A.ToString(); // Potential NullReferenceException
                aId.No_8B = dId.No_8B.ToString(); // Potential NullReferenceException
                aId.No_9A = dId.No_9A.ToString(); // Potential NullReferenceException
                aId.No_9B = dId.No_9B.ToString(); // Potential NullReferenceException
                aId.Yes_7A = dId.Yes_7A.ToString(); // Potential NullReferenceException
                aId.Yes_7B = dId.Yes_7B.ToString(); // Potential NullReferenceException
                aId.Yes_7C = dId.Yes_7C.ToString(); // Potential NullReferenceException
                aId.Yes_8A = dId.Yes_8A.ToString(); // Potential NullReferenceException
                aId.Yes_8B = dId.Yes_8B.ToString(); // Potential NullReferenceException
                aId.Yes_9A = dId.Yes_9A.ToString(); // Potential NullReferenceException
                aId.Yes_9B = dId.Yes_9B.ToString(); // Potential NullReferenceException

                // These call methods which need to be in their own partial classes
                ExportBuyer(aId.Buyer_segment, dId.xC71_Buyer_segment); // Potential NullReferenceException
                ExportDeclarant(aId.Declarant_segment, dId.xC71_Declarant_segment); // Potential NullReferenceException
                ExportSeller(aId.Seller_segment, dId.xC71_Seller_segment); // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}