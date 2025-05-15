using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers
{
    using Serilog;

    public interface IFileTypeImporter
    {
        Task Import(string fileName, ILogger log);
    }
}