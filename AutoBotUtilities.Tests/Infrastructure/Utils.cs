using AutoBot;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBotUtilities.Tests.Infrastructure
{
    public static class Utils
    {



        public static bool IsTestApplicationSettings()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["CoreEntities"].ConnectionString.Contains(@"JOEXPS\SQLDEVELOPER2019") 
                && System.Configuration.ConfigurationManager.ConnectionStrings["CoreEntities"].ConnectionString.Contains(@"AutoBot-EnterpriseDB");
        }

        public static string GetTestDirectory()
        {

            var testDirectory =
                WaterNut.Business.Services.Utils.DirectoryUtils.GetDirectory(new List<string>()
                    { "Imports", "Test Folder" });


            return testDirectory;
        }

        public static string GetTestSalesFile(List<string> testFile)
        {
            var testDirectory = Infrastructure.Utils.GetTestDirectory();
            var finalpaths = testFile.Prepend(testDirectory);
            var testSalesFile = GetTestSalesFile(finalpaths);
            return testSalesFile;
        }

        private static string GetTestSalesFile(IEnumerable<string> testFile)
        {
            var testSalesFile = Path.Combine(testFile.ToArray());
            if (!File.Exists(testSalesFile))
                throw new ApplicationException($"TestFile Dose not Exists: '{testSalesFile}'");
            return testSalesFile;
        }

        public static void SetTestApplicationSettings(int appId)
        {
            AutoBot.Utils.SetCurrentApplicationSettings(appId);
        }

        public static IEnumerable<FileTypes> GetUnknownCSVFileType() =>
            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.Csv);

        public static IEnumerable<FileTypes> GetPOCSVFileType() =>
            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Csv);

        public static void 
            ClearDataBase()
        {
            if (!Infrastructure.Utils.IsTestApplicationSettings()) return;
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.ExecuteSqlCommand(@"delete from AsycudaSalesAllocations
                            DELETE FROM AsycudaDocumentSet
                            FROM    AsycudaDocumentSet LEFT OUTER JOIN
                                             FileTypes ON AsycudaDocumentSet.Declarant_Reference_Number = FileTypes.DocSetRefernece LEFT OUTER JOIN
                                             SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                            WHERE (SystemDocumentSets.Id IS NULL) --and filetypes.AsycudaDocumentSetId is null
                            delete from xcuda_Item
                            delete from xcuda_ASYCUDA
                            delete from [InventoryItems-NonStock]
                            delete from InventoryItems
                            delete from EntryData");
            }
        }


        public static void ImportDocuments(AsycudaDocumentSet docSet, List<string> fileNames, bool noMessages = false)
        {
            bool importOnlyRegisteredDocument = true;

            bool importTariffCodes = true;

            bool overwriteExisting = true;

            bool linkPi = true;

            BaseDataModel.Instance.ImportDocuments(docSet, fileNames, importOnlyRegisteredDocument,
                importTariffCodes, noMessages, overwriteExisting, linkPi).Wait();
        }


        public static void ImportEntryDataOldWay(List<string> files, string entryType, string fileFormat)
        {
            var testFile = Infrastructure.Utils.GetTestSalesFile(files);
            var fileTypes = FileTypeManager.GetImportableFileType(entryType, fileFormat);
            foreach (var fileType in fileTypes)
            {
                CSVUtils.SaveCsv(new List<FileInfo>() { new FileInfo(testFile) }, fileType);
            }
        }

        public static TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}
