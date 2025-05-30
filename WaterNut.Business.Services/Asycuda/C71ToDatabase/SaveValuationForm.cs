using Asycuda421; // Assuming Value_declaration_form is here
using System;
using ValuationDS.Business.Entities; // Assuming Registered is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void SaveValuationForm(Value_declaration_form adoc, ValuationDS.Business.Entities.Registered da)
        {
            try
            {
                da.id = adoc.id; // Potential NullReferenceException
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}