using System.Collections.Generic;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class SaveEntryDataFile : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;
 
        public SaveEntryDataFile(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }
 
        public Task<List<dynamic>> Execute(List<dynamic> lines, ILogger log)
        {
            var eslst = EntryDataFileImporter.SetDefaults(lines);
 
            EntryDataFileImporter.SaveDataFile(eslst, _importSettings.FileType, _importSettings.EmailId, _importSettings.DroppedFilePath);
 
            return Task.FromResult(lines);
        }
    }
}