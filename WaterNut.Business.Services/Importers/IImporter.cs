using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers
{
    using Serilog;

    public interface IImporter
    {
        Task Import(string fileName, bool overWrite, ILogger log);
    }
}