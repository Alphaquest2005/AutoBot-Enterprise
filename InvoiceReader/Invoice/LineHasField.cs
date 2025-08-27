using System.Linq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        private static bool LineHasField(Line x, Fields field)
        {
            return x.OCR_Lines.Fields.Any(z => z.Field == field.Field);
        }
    }
}