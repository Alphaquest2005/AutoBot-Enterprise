using System.Collections.Generic;
using CoreEntities.Business.Entities;

namespace WaterNut.Business.Services.Importers
{
    public interface IDataExtractor
    {
        List<dynamic> Execute();
        List<dynamic> Execute(List<dynamic> list);
    }
}