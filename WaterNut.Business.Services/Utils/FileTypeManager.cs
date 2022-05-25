using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Utils
{
    public static class FileTypeManager
    {
        private static List<FileTypes> _fileTypes;
        public static FileTypes GetFileType(FileTypes fileTypes) => Enumerable.First<FileTypes>(FileTypes(), x => x.Id == fileTypes.Id);

        public static List<FileTypes> FileTypes()
        {
            try
            {
                if (_fileTypes == null || BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId !=
                    _fileTypes.First().ApplicationSettingsId)
                    using (var ctx = new CoreEntitiesContext())
                    {
                        _fileTypes = ctx.FileTypes
                            .Include("FileTypeContacts.Contacts")
                            .Include("FileTypeActions.Actions")
                           // .Include("AsycudaDocumentSetEx")
                            .Include("ChildFileTypes")
                            .Include("FileTypeMappings.FileTypeMappingRegExs")
                            .Include(x => x.FileImporterInfos)
                            .Include(x => x.ImportActions)
                            .Include(x => x.FileTypeReplaceRegex)
                            .Where(x => x.FileImporterInfos != null)
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();

                    }

                return _fileTypes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static FileTypes GetFileType(int fileTypeId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes.FirstOrDefault(x => x.Id == fileTypeId);
                return fileType != null ? GetFileType(fileType) : null;
            }
        }

        public static FileTypes GetHeadingFileType(IEnumerable<string> heading, FileTypes suggestedfileType)
        {

            using (var ctx = new CoreEntitiesContext())
            {
                var mappingFileType =
                    ctx.FileTypes
                        .Include(x => x.FileTypeMappings)
                        .Include(x => x.ImportActions)
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                    x.FileTypeMappings.Any())
                        .SelectMany(x => x.FileTypeMappings.Where(z =>
                            heading.Select(h => h.ToUpper().Trim()).Contains(z.OriginalName.ToUpper().Trim())))
                        .GroupBy(x => x.FileTypes)
                        .OrderByDescending(x => x.Count())
                        .Where(x => x.Key.IsImportable == null || x.Key.IsImportable == true)
                        .FirstOrDefault(x =>
                            suggestedfileType.FileImporterInfos.EntryType == EntryTypes.Unknown
                                ? x.Key.FileImporterInfos.EntryType != null
                                : x.Key.FileImporterInfos.EntryType == suggestedfileType.FileImporterInfos.EntryType)?.Key;


                FileTypes fileType;
                if (mappingFileType != null
                    && mappingFileType.Id != suggestedfileType.Id
                    && mappingFileType.Id != suggestedfileType.ParentFileTypeId)
                {
                    fileType = GetFileType(mappingFileType);
                }
                else
                {
                    fileType = GetFileType(suggestedfileType);
                }

                return fileType;

            }
        }


     

        public static class EntryTypes
        {
            public const string Unknown = "Unknown";
            public const string Po = "PO";
            public const string Sales = "Sales";
            public const string Inv = "INV";
            public const string ShipmentInvoice = "Shipment Invoice";
            public const string Ops = "OPS";
            public const string Adj = "ADJ";
            public const string Dis = "DIS";
            public const string Rcon = "RCON";
            public const string CancelledEntries = "CancelledEntries";
            public const string ExpiredEntries = "ExpiredEntries";
            public const string Freight = "Freight";
            public const string BL = "BL";
            public const string Manifest = "Manifest";
            public const string Rider = "Rider";
            public const string SubItems = "SubItems";
            public const string C71 = "C71";
            public const string Lic = "LIC";
            public const string POTemplate = "POTemplate";
            public const string Info = "Info";
            public const string xSales = "xSales";
        }


        public static class FileFormats
        {
            public const string Csv = "CSV";
            public const string Xlsx = "XLSX";
            public const string PDF = "PDF";
            public const string XML = "XML";
        }

        public static List<FileTypes> GetImportableFileType(string entryType, string fileFormat) =>
            FileTypes()
                .Where(x => x.FileImporterInfos?.EntryType == entryType && x.FileImporterInfos?.Format == fileFormat)
                .Where(x => x.FileTypeMappings.Any() || entryType == EntryTypes.Unknown).ToList();
    }
}
