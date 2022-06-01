using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using OCR.Business.Services;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers
{
    public class FileTypeImporter : IFileTypeImporter
    {
        public  FileTypes FileType { get; private set; }

        public FileTypeImporter(FileTypes fileType)
        {
            FileType = fileType;
            _importers = new Dictionary<string, IImporter>()
            {
                {FileTypeManager.FileFormats.Csv, new CSVImporter(FileType)},
            };
        }
        public void Import(string fileName)
        {
            var importer = _importers[FileType.FileImporterInfos.Format];
            importer.Import(fileName, true);
        }

        private readonly Dictionary<string, IImporter> _importers;

    }
}
