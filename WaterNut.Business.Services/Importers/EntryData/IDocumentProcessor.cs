using System.Collections.Generic;

using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public interface IDocumentProcessor
    {
        Task<List<dynamic>> Execute(List<dynamic> lines);
    }
}