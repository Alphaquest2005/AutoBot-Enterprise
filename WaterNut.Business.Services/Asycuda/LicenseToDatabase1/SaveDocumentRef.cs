using System.Text.RegularExpressions;
using LicenseDS.Business.Entities; // Assuming Registered is here

namespace WaterNut.DataSpace.Asycuda
{
    // Assuming this is part of the same partial class C71ToDataBase as the previous file
    public partial class C71ToDataBase
    {
        private static void SaveDocumentRef(Registered da)
        {
            var match = Regex.Match(da.xLIC_General_segment.Exporter_address, // Potential NullReferenceException
                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
            if (match.Success)
                da.DocumentReference = match.Groups["Value"].Value.Trim().Replace(",",""); // Potential NullReferenceException
        }
    }
}