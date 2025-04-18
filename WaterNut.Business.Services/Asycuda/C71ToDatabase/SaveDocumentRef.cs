using System.Text.RegularExpressions;
using ValuationDS.Business.Entities; // Assuming Registered is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private static void SaveDocumentRef(Registered da)
        {
            var match = Regex.Match(da.xC71_Identification_segment.xC71_Seller_segment.Address, // Potential NullReferenceException
                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
            if (match.Success)
                da.DocumentReference = match.Groups["Value"].Value.Trim().Replace(",",""); // Potential NullReferenceException
        }
    }
}