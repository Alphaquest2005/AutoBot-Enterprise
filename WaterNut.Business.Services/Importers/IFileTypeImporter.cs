using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers
{
    public interface IFileTypeImporter
    {
        Task Import(string fileName);
    }
}