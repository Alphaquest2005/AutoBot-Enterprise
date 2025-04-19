using System.Collections.Generic;

namespace WaterNut.Business.Services.Utils
{
    public class InventoryData
    {
        public (dynamic ItemNumber, dynamic ItemDescription, dynamic TariffCode) Key { get; }
        public List<dynamic> Data { get; }

        public InventoryData((dynamic ItemNumber, dynamic ItemDescription, dynamic TariffCode) key, List<dynamic> data)
        {
            Key = key;
            Data = data;
        }
    }
}