using System.Threading.Tasks;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace
{
    public interface ISaveDocumentCT
    {
        Task Execute(DocumentCT cdoc);
    }
}