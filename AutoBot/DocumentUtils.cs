using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static void ImportPOEntries(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (ImportEntries(overwriteExisting, fileTypes, DateTime.Today.AddHours(-12))) return;

                ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder();

                EntryDocSetUtils.RemoveDuplicateEntries();
                EntryDocSetUtils.FixIncompleteEntries();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportAllSalesEntries()
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (ImportEntries(false, fileTypes, DateTime.MinValue)) return;

                ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder();

                EntryDocSetUtils.RemoveDuplicateEntries();
                EntryDocSetUtils.FixIncompleteEntries();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportSalesEntries(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import Entries");

                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                if (ImportEntries(overwriteExisting, fileTypes)) return;

                ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder();

                EntryDocSetUtils.RemoveDuplicateEntries();
                EntryDocSetUtils.FixIncompleteEntries();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes, DateTime? getMinFileDate = null)
        {
            if (!fileTypes.Any()) return true;

            var docSetId = GetDefaultAsycudaDocumentSetId();

            if (getMinFileDate == null) getMinFileDate = GetDocSetLastFileDate(docSetId);

            var fileTypeFiles = GetFileTypeFiles(fileTypes, docSetId, getMinFileDate.GetValueOrDefault());

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

        private static void ImportEntries(bool overwriteExisting, IEnumerable<(FileTypes FileType, List<FileInfo> Files)> fileTypeFiles) =>
            fileTypeFiles.ForEach(ft =>
                BaseDataModel.Instance.ImportDocuments(WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(ft.FileType.DocSetRefernece, true).AsycudaDocumentSetId,
                    ft.Files.Select(x => x.FullName).ToList(), true, true, false, overwriteExisting, true).Wait());

        private static DateTime GetDocSetLastFileDate(int docSetId) => GetFileCreationDate(GetAsycudaDocumentSetAttachments(docSetId));

        private static DirectoryInfo GetDocSetDirectoryInfo(int docSetId)
        {
            var docSetDirectoryInfo = new DirectoryInfo(GetDocSetDestinationFolder(docSetId));
            docSetDirectoryInfo.Refresh();
            return docSetDirectoryInfo;
        }

        private static string GetDocSetDestinationFolder(int docSetId) => Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, GetDocSetReference(docSetId));

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

        private static int GetDefaultAsycudaDocumentSetId() =>
            new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ??
            BaseDataModel.CurrentSalesInfo(-1).Item3.AsycudaDocumentSetId;
    }
}