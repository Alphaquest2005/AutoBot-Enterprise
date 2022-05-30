using System.Collections.Generic;

namespace WaterNut.Business.Services.Importers
{
    public interface IDocumentProcessor
    {
        DataFile Execute(DataFile lines);
    }
}