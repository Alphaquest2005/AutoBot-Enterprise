using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using ValuationDS.Business.Entities; // Assuming ValuationDSContext, Registered, xC71_Value_declaration_form are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        public static bool GetRegNumber(FileInfo file, out string regNumber)
        {
            using (var ctx = new ValuationDSContext())
            {
                var c71 = ctx.xC71_Value_declaration_form.OfType<Registered>()
                    .Include(x => x.xC71_Identification_segment) // Eager loading might be needed depending on usage
                    .FirstOrDefault(x => x.SourceFile == file.FullName);
                if (c71 != null)
                {
                    regNumber = c71.RegNumber;
                    return true;
                }
                else
                {
                    regNumber = null;
                    return false;
                }
            }
        }
    }
}