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
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = GetDefaultAsycudaDocumentSetId();


                    var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                    if (fileTypes == null) return;
                    var docSetReference = new DocumentDSContext().AsycudaDocumentSets
                        .Where(x => x.AsycudaDocumentSetId == docSetId)
                        .Select(x => x.Declarant_Reference_Number).First();

                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        docSetReference);
                    var directoryInfo = new DirectoryInfo(desFolder);
                    directoryInfo.Refresh();
                    var fileStartTime = DateTime.Today.AddHours(-12);
                    foreach (var ft in fileTypes)
                    {
                        var csvFiles = directoryInfo.GetFiles()
                            .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                            .Where(x => x.LastWriteTime >= fileStartTime)
                            .ToArray();

                        if (csvFiles.Length > 0)
                        {
                            var docSet =
                                WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(ft.DocSetRefernece, true);
                            BaseDataModel.Instance.ImportDocuments(docSet.AsycudaDocumentSetId,
                                    csvFiles.Select(x => x.FullName).ToList(), true, true, false, overwriteExisting,
                                    true)
                                .Wait();
                        }
                    }

                    Utils.ImportAllAsycudaDocumentsInDataFolder();

                    EntryDocSetUtils.RemoveDuplicateEntries();
                    EntryDocSetUtils.FixIncompleteEntries();
                }
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
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = GetDefaultAsycudaDocumentSetId();

                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.FileImporterInfos.Format == FileTypeManager.FileFormats.XML && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                            .Declarant_Reference_Number);
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                        .ToArray();
                    if (csvFiles.Length > 0)
                        BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();

                    Utils.ImportAllAsycudaDocumentsInDataFolder();

                    EntryDocSetUtils.RemoveDuplicateEntries();
                    EntryDocSetUtils.FixIncompleteEntries();
                }
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

                Utils.ImportAllAsycudaDocumentsInDataFolder();

                EntryDocSetUtils.RemoveDuplicateEntries();
                EntryDocSetUtils.FixIncompleteEntries();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes)
        {
            if (!fileTypes.Any()) return true;

            var docSetId = GetDefaultAsycudaDocumentSetId();

            var lastfiledate = GetDocSetLastFileDate(docSetId);

            var directoryInfo = GetDocSetDirectoryInfo(docSetId);


            ImportEntries(overwriteExisting, fileTypes, directoryInfo, lastfiledate);
            return false;
        }

        private static void ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes,
            DirectoryInfo directoryInfo,
            DateTime lastfiledate)
        {

            var fileTypeFiles = fileTypes.Select(ft => (FileType:ft, Files: directoryInfo.GetFiles()
                .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                .Where(x => x.LastWriteTime >= lastfiledate)
                .ToList()));

            fileTypeFiles.ForEach(ft =>
                BaseDataModel.Instance.ImportDocuments(WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(ft.FileType.DocSetRefernece, true).AsycudaDocumentSetId,
                    ft.Files.Select(x => x.FullName).ToList(), true, true, false, overwriteExisting, true).Wait());

          
        }

        private static DateTime GetDocSetLastFileDate(int docSetId)
        {
            var lastDbFile = GetAsycudaDocumentSetAttachments(docSetId);

            var lastfiledate = GetFileCreationDate(lastDbFile);
            return lastfiledate;
        }

        private static DirectoryInfo GetDocSetDirectoryInfo(int docSetId)
        {
            var docSetDirectoryInfo = new DirectoryInfo(GetDocSetDestinationFolder(docSetId));
            docSetDirectoryInfo.Refresh();
            return docSetDirectoryInfo;
        }

        private static string GetDocSetDestinationFolder(int docSetId)
        {
            return Path.Combine(
                BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, GetDocSetReference(docSetId));
        }

        private static string GetDocSetReference(int docSetId)
        {
            return new DocumentDSContext().AsycudaDocumentSets.Where(x => x.AsycudaDocumentSetId == docSetId)
                .Select(x => x.Declarant_Reference_Number).First();
        }

        private static DateTime GetFileCreationDate(AsycudaDocumentSet_Attachments lastDbFile)
        {
            return lastDbFile != null
                ? File.GetCreationTime(lastDbFile.Attachments.FilePath)
                : DateTime.Today.AddDays(-1);
        }

        private static AsycudaDocumentSet_Attachments GetAsycudaDocumentSetAttachments(int docSetId)
        {
            return new CoreEntitiesContext().AsycudaDocumentSet_Attachments.Include(x => x.Attachments)
                .OrderByDescending(x => x.AttachmentId)
                .FirstOrDefault(x => x.AsycudaDocumentSetId == docSetId);
        }

        private static int GetDefaultAsycudaDocumentSetId()
        {
            return new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                       x.ApplicationSettingsId ==
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                       x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ??
                   BaseDataModel.CurrentSalesInfo(-1).Item3.AsycudaDocumentSetId;
        }

      
    }
}