using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace AutoBotUtilities.Tests.Infrastructure
{
    public static class Utils
    {
        public static string GetTestDirectory()
        {

            var testDirectory =
                WaterNut.Business.Services.Utils.DirectoryUtils.GetDirectory(new List<string>()
                    { "Imports", "Test Folder" });


            return testDirectory;
        }

        public static string GetTestSalesFile(string testFile)
        {
            var testDirectory = Infrastructure.Utils.GetTestDirectory();
            var testSalesFile = GetTestSalesFile(testDirectory, testFile);
            return testSalesFile;
        }

        public static string GetTestSalesFile(string testDirectory, string testFile)
        {
            var testSalesFile = Path.Combine(testDirectory, testFile);
            if (!File.Exists(testSalesFile))
                throw new ApplicationException($"TestFile Dose not Exists: '{testSalesFile}'");
            return testSalesFile;
        }

        public static void SetTestApplicationSettings(int appId)
        {
            AutoBot.Utils.SetCurrentApplicationSettings(appId);
        }

        public static IEnumerable<FileTypes> GetUnknownCSVFileType() =>
            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.CSV);

        public static IEnumerable<FileTypes> GetPOCSVFileType() =>
            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.PO, FileTypeManager.FileFormats.CSV);

        public static void ClearDataBase()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.ExecuteSqlCommand(@"delete from AsycudaSalesAllocations
                            DELETE FROM AsycudaDocumentSet
                            FROM    AsycudaDocumentSet LEFT OUTER JOIN
                                             FileTypes ON AsycudaDocumentSet.AsycudaDocumentSetId = FileTypes.AsycudaDocumentSetId LEFT OUTER JOIN
                                             SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                            WHERE (SystemDocumentSets.Id IS NULL) and filetypes.AsycudaDocumentSetId is null
                            delete from xcuda_Item
                            delete from xcuda_ASYCUDA
                            delete from [InventoryItems-NonStock]
                            delete from InventoryItems
                            delete from EntryData");
            }
        }
    }
}
