using System.Collections.Generic;

namespace WaterNut.Business.Services.Utils
{
    public class SupplierData
    {
        public (dynamic SupplierCode, dynamic SupplierName, dynamic SupplierAddress, dynamic CountryCode) Key { get; }
        public List<dynamic> Data { get; }

        public SupplierData((dynamic SupplierCode, dynamic SupplierName, dynamic SupplierAddress, dynamic CountryCode) key, List<dynamic> data)
        {
            Key = key;
            Data = data;
        }
    }
}