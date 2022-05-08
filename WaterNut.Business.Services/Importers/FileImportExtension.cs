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
        public static FileTypes FileType { get; private set; }

        public FileTypeImporter(FileTypes fileType)
        {
            FileType = fileType;
            
        }
        public void Import(string fileName)
        {
            var importer = Importers[FileType.FileImporterInfos.Format];
            importer.Import(fileName, true);
        }

        private readonly Dictionary<string, IImporter> Importers =
            new Dictionary<string, IImporter>()
            {
                {FileTypeManager.FileFormats.CSV, new CSVImporter(FileType)},
            };

    }
}
