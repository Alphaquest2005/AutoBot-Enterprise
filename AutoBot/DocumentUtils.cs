using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;

namespace AutoBot
{
    public class DocumentUtils
    {
        public static async Task ImportPOEntries(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (await ImportEntries(overwriteExisting, fileTypes, DateTime.Today.AddHours(-12)).ConfigureAwait(false)) return;

                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting);

               await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task ImportAllSalesEntries(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (await ImportEntries(overwriteExisting, fileTypes, DateTime.MinValue).ConfigureAwait(false)) return;

                ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting);

                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task ImportSalesEntries(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (await ImportEntries(overwriteExisting, fileTypes).ConfigureAwait(false)) return;

                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting);

                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task ImportEntries(bool overwriteExisting, string fileLst)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (await ImportEntries(overwriteExisting, fileTypes, fileLst).ConfigureAwait(false)) return;

                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting);

                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<bool> ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes, DateTime? getMinFileDate = null)
        {
            if (!fileTypes.Any()) return true;

            var docSetId = await GetDefaultAsycudaDocumentSetId().ConfigureAwait(false);

            if (getMinFileDate == null) getMinFileDate = GetDocSetLastFileDate(docSetId);

            var fileTypeFiles = GetFileTypeFiles(fileTypes, docSetId, getMinFileDate.GetValueOrDefault());

            ImportEntries(overwriteExisting, fileTypeFiles);
            return false;
        }

        private static async Task<bool> ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes, string filelst)
        {
            if (!fileTypes.Any()) return true;

            var docSetId = await GetDefaultAsycudaDocumentSetId().ConfigureAwait(false);

            var fileTypeFiles = GetFileTypeFiles(fileTypes, docSetId, filelst);

            ImportEntries(overwriteExisting, fileTypeFiles);
            return false;
        }

        private static IEnumerable<(FileTypes FileType, List<FileInfo> Files)> GetFileTypeFiles(List<FileTypes> fileTypes, int docSetId, DateTime lastfiledate)
        {
            var directoryInfo = GetDocSetDirectoryInfo(docSetId);

            var fileTypeFiles = fileTypes.Select(ft => (FileType: ft, Files: directoryInfo.GetFiles()
                .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                .Where(x => x.LastWriteTime >= lastfiledate)
                .ToList()));
            return fileTypeFiles;
        }

        private static IEnumerable<(FileTypes FileType, List<FileInfo> Files)> GetFileTypeFiles(List<FileTypes> fileTypes, int docSetId, string filelst)
        {
            var directoryInfo = GetDocSetDirectoryInfo(docSetId);
            var files = filelst.Split(new[] { "\r\n", " ", "," }, StringSplitOptions.RemoveEmptyEntries);

            var fileTypeFiles = fileTypes.Select(ft => (FileType: ft, Files: directoryInfo.GetFiles()
                                                               .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                                                               .Where(x => files.Any(z => x.FullName.Contains($"-{z}.xml")))
                                                               .ToList()));
            return fileTypeFiles;
        }

        private static void ImportEntries(bool overwriteExisting, IEnumerable<(FileTypes FileType, List<FileInfo> Files)> fileTypeFiles) =>
            fileTypeFiles.ForEach(async ft =>
            {
                var asycudaDocumentSet = await WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(ft.FileType.DocSetRefernece, true).ConfigureAwait(false);
                await  BaseDataModel.Instance.ImportDocuments(
                    asycudaDocumentSet
                        .AsycudaDocumentSetId,
                    ft.Files.Select(x => x.FullName).ToList(), true, true, false, overwriteExisting, true).ConfigureAwait(false);
            });

        private static DateTime GetDocSetLastFileDate(int docSetId) => GetFileCreationDate(GetAsycudaDocumentSetAttachments(docSetId));

        private static DirectoryInfo GetDocSetDirectoryInfo(int docSetId)
        {
            var docSetDirectoryInfo = new DirectoryInfo(GetDocSetDestinationFolder(docSetId));
            docSetDirectoryInfo.Refresh();
            return docSetDirectoryInfo;
        }

        private static string GetDocSetDestinationFolder(int docSetId) => BaseDataModel.GetDocSetDirectoryName(GetDocSetReference(docSetId));

        private static string GetDocSetReference(int docSetId) =>
            new DocumentDSContext().AsycudaDocumentSets.Where(x => x.AsycudaDocumentSetId == docSetId)
                .Select(x => x.Declarant_Reference_Number).First();

        private static DateTime GetFileCreationDate(AsycudaDocumentSet_Attachments lastDbFile) =>
            lastDbFile != null
                ? File.GetCreationTime(lastDbFile.Attachments.FilePath)
                : DateTime.Today.AddDays(-1);

        private static AsycudaDocumentSet_Attachments GetAsycudaDocumentSetAttachments(int docSetId)
        => new CoreEntitiesContext().AsycudaDocumentSet_Attachments
            .Include(x => x.Attachments)
            .OrderByDescending(x => x.AttachmentId)
            .FirstOrDefault(x => x.AsycudaDocumentSetId == docSetId);

        private static async Task<int> GetDefaultAsycudaDocumentSetId()
        {
            var currentSalesInfo = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
            return new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                       x.ApplicationSettingsId ==
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                       x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ??
                   currentSalesInfo.Item3.AsycudaDocumentSetId;
        }
    }
}