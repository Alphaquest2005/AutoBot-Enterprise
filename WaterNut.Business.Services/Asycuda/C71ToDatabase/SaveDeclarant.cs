using Asycuda421; // Assuming Value_declaration_formIdentification_segmentDeclarant_segment is here
using System;
using System.Linq;
using ValuationDS.Business.Entities; // Assuming xC71_Declarant_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void SaveDeclarant(Value_declaration_formIdentification_segmentDeclarant_segment aDecl, xC71_Declarant_segment dDecl)
        {
            try
            {
                dDecl.Address = aDecl.Address.Text.FirstOrDefault(); // Potential NullReferenceException
                dDecl.Code = aDecl.Code.Text.FirstOrDefault(); // Potential NullReferenceException
                dDecl.Name = aDecl.Name.Text.FirstOrDefault(); // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}