using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveNewSuppliers : IProcessor<SupplierData>
    {
        public async Task<Result<List<SupplierData>>> Execute(List<SupplierData> data)
        {
            await SupplierProcessor.SaveNewSuppliers(data).ConfigureAwait(false);
            return new Result<List<SupplierData>>(data, true, "") ;
        }
    }
}