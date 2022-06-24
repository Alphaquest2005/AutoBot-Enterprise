using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetSupplierData : IProcessor<SupplierData>
    {
        private readonly List<dynamic> _lines;

        public GetSupplierData(List<dynamic> lines)
        {
            _lines = lines;
        }

        public Result<List<SupplierData>> Execute(List<SupplierData> data)
        {
            var supplierDatas = _lines
                .GroupBy(x => (x.SupplierCode, x.SupplierName, x.SupplierAddress, x.CountryCode ))
                .Select(x => new SupplierData(x.Key, x.ToList()))
                .ToList();
            return new Result<List<SupplierData>>(supplierDatas, true, "");
        }
    }
}