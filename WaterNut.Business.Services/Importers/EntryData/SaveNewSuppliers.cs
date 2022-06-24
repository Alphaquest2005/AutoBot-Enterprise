using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveNewSuppliers : IProcessor<SupplierData>
    {
        public Result<List<SupplierData>> Execute(List<SupplierData> data)
        {
            SupplierProcessor.SaveNewSuppliers(data).Wait();
            return new Result<List<SupplierData>>(data, true, "") ;
        }
    }
}