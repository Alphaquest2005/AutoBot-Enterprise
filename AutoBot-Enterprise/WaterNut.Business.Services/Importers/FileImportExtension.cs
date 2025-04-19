using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using MoreLinq;
using OCR.Business.Services;
using WaterNut.Business.Services.Importers.EntryData;
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
                {FileTypeManager.FileFormats.Xlsx, new XLSXImporter(FileType)},
                {FileTypeManager.FileFormats.PDF, new PDFImporter(FileType)},

            };
        }
        public void Import(string fileName)
        {
            var importer = _importers[FileType.FileImporterInfos.Format];
            importer.Import(fileName, true);
        }

        private readonly Dictionary<string, IImporter> _importers;

    }

    public class XLSXImporter : IImporter
    {
        private readonly FileTypes _fileType;

        public XLSXImporter(FileTypes fileType)
        {
            _fileType = fileType;
            
        }

        public void Import(string fileName, bool overWrite)
        {
            try
            {
                var result = XLSXUtils.ExtractTables(new FileInfo(fileName));
                var DataSetProcessors = new Dictionary<string, IProcessor<DataTable>>()
                {
                    { FileTypeManager.EntryTypes.ShipmentInvoice, new MisMatchesProcessor(_fileType) },
                    { FileTypeManager.EntryTypes.Po, new XlsxEntryDataProcessor(_fileType, fileName, overWrite) },
                    { FileTypeManager.EntryTypes.Sales, new XlsxEntryDataProcessor(_fileType, fileName, overWrite) },
                    { FileTypeManager.EntryTypes.Dis, new XlsxEntryDataProcessor(_fileType, fileName, overWrite) },
                    { FileTypeManager.EntryTypes.Unknown, new XlsxEntryDataProcessor(_fileType, fileName, overWrite) },
                };

                DataSetProcessors[_fileType.FileImporterInfos.EntryType]
                    .Execute(result.Tables.GetEnumerator().ToList<DataTable>());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }

    public class PDFImporter : IImporter
    {
        public PDFImporter(FileTypes fileType)
        {
           
        }

        public void Import(string fileName, bool overWrite)
        {
            
        }
    }
}
