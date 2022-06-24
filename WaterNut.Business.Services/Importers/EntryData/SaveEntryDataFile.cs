using System.Collections.Generic;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveEntryDataFile : IDocumentProcessor
    {
        private readonly ImportSettings _importSettings;

        public SaveEntryDataFile(ImportSettings importSettings)
        {
            _importSettings = importSettings;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            var eslst = EntryDataFileImporter.SetDefaults(lines);

            EntryDataFileImporter.SaveDataFile(eslst, _importSettings.FileType, _importSettings.EmailId, _importSettings.DroppedFilePath);

            return lines;
        }
    }
}