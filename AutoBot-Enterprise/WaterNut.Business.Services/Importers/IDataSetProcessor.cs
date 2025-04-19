using System.Collections.Generic;
using System.Data;

namespace WaterNut.Business.Services.Importers
{
    public interface IDataSetProcessor
    {
        void Execute(List<DataTable> result);
    }
}