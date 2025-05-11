using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers
{
    public interface IImporter
    {
        Task Import(string fileName, bool overWrite);
    }
}