using System;

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private static bool ToBool(string val)
        {
            // Convert.ToBoolean might throw FormatException if val is not "true" or "false" (case-insensitive)
            return !string.IsNullOrEmpty(val) && Convert.ToBoolean(val);
        }
    }
}