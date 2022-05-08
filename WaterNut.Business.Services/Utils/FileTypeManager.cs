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
                            suggestedfileType.Type == "Unknown"
                                ? x.Key.Type != null
                                : x.Key.Type == suggestedfileType.Type)?.Key;


                FileTypes fileType;
                if (mappingFileType != null
                    && mappingFileType.Id != suggestedfileType.Id
                    && mappingFileType.Id != suggestedfileType.ParentFileTypeId)
                {
                    fileType = GetFileType(mappingFileType);
                }
                else
                {
                    fileType = suggestedfileType;
                }

                return fileType;

            }
        }


     

        public static class EntryTypes
        {
            public const string Unknown = "Unknown";
            public const string PO = "PO";
        }


        public static class FileFormats
        {
            public const string CSV = "CSV";
        }

        public static List<FileTypes> GetImportableFileType(string entryType, string fileFormat) =>
            FileTypes()
                .Where(x => x.FileImporterInfos?.EntryType == entryType && x.FileImporterInfos?.Format == fileFormat)
                .Where(x => x.FileTypeMappings.Any() || entryType == EntryTypes.Unknown).ToList();
    }
}
