using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public interface IProcessor<TData>
    {
        Result<List<TData>> Execute(List<TData> data);
    }


    
}