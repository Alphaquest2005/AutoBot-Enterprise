using System.Linq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        private static bool ValueContainsRequiredField(Line x, Fields field)
        {
            return x.Values.All(v => v.Value.Keys.Any(k => k.Fields == field));
        }
    }
}