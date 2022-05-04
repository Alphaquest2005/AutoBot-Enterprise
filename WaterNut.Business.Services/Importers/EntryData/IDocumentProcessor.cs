using System.Collections.Generic;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public interface IDocumentProcessor
    {
        List<dynamic> Execute(List<dynamic> lines);
    }
}