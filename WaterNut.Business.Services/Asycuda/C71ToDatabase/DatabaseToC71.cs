using Asycuda421; // Assuming Value_declaration_form is here
using ValuationDS.Business.Entities; // Assuming xC71_Value_declaration_form is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private Value_declaration_form DatabaseToC71(xC71_Value_declaration_form c71)
        {
            Value_declaration_form adoc = new Value_declaration_form();
            // These call methods which need to be in their own partial classes
            ExportIdentification(adoc.Identification_segment, c71.xC71_Identification_segment); // Potential NullReferenceException
            ExportItems(adoc.Item, c71); // Potential NullReferenceException
            return adoc;
        }
    }
}