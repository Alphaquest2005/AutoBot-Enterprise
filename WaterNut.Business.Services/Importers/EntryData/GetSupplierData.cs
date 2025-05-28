using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class GetSupplierData : IProcessor<SupplierData>
    {
        private readonly List<dynamic> _lines;
 
        public GetSupplierData(List<dynamic> lines)
        {
            _lines = lines;
        }
 
        public Task<Result<List<SupplierData>>> Execute(List<SupplierData> data, ILogger log)
        {
            var supplierDatas = _lines
                .GroupBy(x => (x.SupplierCode, x.SupplierName, x.SupplierAddress, x.CountryCode ))
                .Select(x => new SupplierData(x.Key, x.ToList()))
                .ToList();
            return Task.FromResult(Task.FromResult(new Result<List<SupplierData>>(supplierDatas, true, "")).Result); // Wrap in Task.FromResult
        }
    }
}