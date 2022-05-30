using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class CSVDataFile
    {
        private readonly string _droppedFilePath;
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;
        private readonly bool _overWriteExisting;

        static CSVDataFile()
        {
        }

        public CSVDataFile(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting)
        {
            _droppedFilePath = droppedFilePath;
            _fileType = fileType;
            _docSet = docSet;
            _overWriteExisting = overWriteExisting;
        }

        public async Task Save()
        {
            var csvImporter = new CSVImporter(_fileType);
            var emailId = Utils.GetExistingEmailId(_droppedFilePath, _fileType);

            var lines = csvImporter.GetFileLines(_droppedFilePath).ToArray();

            var fixedHeadings = csvImporter.GetHeadings(lines).ToArray();


            await SaveCsvSubItems.Instance.ExtractSubItems(_fileType, lines, fixedHeadings)
                .ConfigureAwait(false);

            await SaveCsvEntryData.Instance
                .ExtractEntryData(_fileType, lines, fixedHeadings, _docSet, _overWriteExisting, emailId,
                    _droppedFilePath).ConfigureAwait(false);


        }
    }
}