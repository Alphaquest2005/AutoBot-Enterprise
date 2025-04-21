//using System.Diagnostics;
//using CoreEntities.Business.Entities;
//using Serilog; // Added for logging
//using WaterNut.Business.Services.Utils;
//using WaterNut.DataSpace;
//using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
//using FileTypes = CoreEntities.Business.Entities.FileTypes;

//namespace InvoiceReaderPipelineTests
//{
//    public static class Utils
//    {
//        // Add a static logger instance for this class
//        private static readonly ILogger _logger = Log.ForContext(typeof(Utils));


//        public static bool IsTestApplicationSettings()
//        {
//            return
//                IsDevSqlServer() 
//                && System.Configuration.ConfigurationManager.ConnectionStrings["CoreEntities"].ConnectionString.Contains(@"AutoBot-EnterpriseDB");
//        }

//        public static bool IsDevSqlServer()
//        {
//            return System.Configuration.ConfigurationManager.ConnectionStrings["CoreEntities"].ConnectionString.Contains(@"MINIJOE\SQLDEVELOPER2022");
//        }

//        public static string GetTestDirectory()
//        {

//            var testDirectory =
//                WaterNut.Business.Services.Utils.DirectoryUtils.GetDirectory(new List<string>()
//                    { "Imports", "Test Folder" });


//            return testDirectory;
//        }

//        public static string GetTestSalesFile(List<string> testFile)
//        {
//            var testDirectory = Utils.GetTestDirectory();
//            var finalpaths = testFile.Prepend(testDirectory);
//            var testSalesFile = GetTestSalesFile(finalpaths);
//            return testSalesFile;
//        }

//        private static string GetTestSalesFile(IEnumerable<string> testFile)
//        {
//            var testSalesFile = Path.Combine(testFile.ToArray());
//            if (!File.Exists(testSalesFile))
//                throw new ApplicationException($"TestFile Dose not Exists: '{testSalesFile}'");
//            return testSalesFile;
//        }

//        public static void SetTestApplicationSettings(int appId)
//        {
//            _logger.Information("Entering SetTestApplicationSettings with appId: {AppId}", appId);
//            try
//            {
//                AutoBot.Utils.SetCurrentApplicationSettings(appId);
//                _logger.Information("Successfully called AutoBot.Utils.SetCurrentApplicationSettings");
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error in SetTestApplicationSettings while calling AutoBot.Utils.SetCurrentApplicationSettings");
//                throw; // Re-throw the exception to ensure test failure is propagated
//            }
//            _logger.Information("Exiting SetTestApplicationSettings");
//        }

//        public static IEnumerable<FileTypes> GetUnknownCSVFileType(string fileName) =>
//            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.Csv, fileName);

//        public static IEnumerable<FileTypes> GetPOCSVFileType(string testFile) =>
//            FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Csv, testFile);

//        public static void
//            ClearDataBase()
//        {
//            _logger.Information("Entering ClearDataBase");
//            if (!Utils.IsTestApplicationSettings())
//            {
//                _logger.Warning("ClearDataBase skipped because IsTestApplicationSettings returned false.");
//                return;
//            }

//            try
//            {
//                _logger.Debug("Creating new CoreEntitiesContext for ClearDataBase.");
//                using (var ctx = new CoreEntitiesContext())
//                {
//                    _logger.Information("CoreEntitiesContext created. Connection String: {ConnectionString}", ctx.Database.Connection.ConnectionString);
//                    _logger.Debug("Executing SQL command to clear database tables.");
//                    ctx.Database.ExecuteSqlCommand(@"delete from AsycudaSalesAllocations
//                             DELETE FROM AsycudaDocumentSet
//                             FROM    AsycudaDocumentSet LEFT OUTER JOIN
//                             FileTypes ON AsycudaDocumentSet.Declarant_Reference_Number = FileTypes.DocSetRefernece LEFT OUTER JOIN
//                             SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
//                             WHERE (SystemDocumentSets.Id IS NULL) --and filetypes.AsycudaDocumentSetId is null
//                             delete from xcuda_Item
//                             delete from xcuda_ASYCUDA
//                             delete from [InventoryItems-NonStock]
//                             delete from InventoryItems
//                             delete from EntryData
//                             delete from ShipmentInvoice
//                             delete from xSalesFiles");
//                    _logger.Information("SQL command executed successfully in ClearDataBase.");
//                }
//                _logger.Information("CoreEntitiesContext disposed in ClearDataBase.");
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error during ClearDataBase execution.");
//                throw; // Re-throw the exception to ensure test failure is propagated
//            }
//            _logger.Information("Exiting ClearDataBase");
//        }


//        public static void ImportDocuments(AsycudaDocumentSet docSet, List<string> fileNames, bool noMessages = false)
//        {
//            bool importOnlyRegisteredDocument = true;

//            bool importTariffCodes = true;

//            bool overwriteExisting = true;

//            bool linkPi = true;

//            BaseDataModel.Instance.ImportDocuments(docSet, fileNames, importOnlyRegisteredDocument,
//                importTariffCodes, noMessages, overwriteExisting, linkPi).Wait();
//        }


//        public static void ImportEntryDataOldWay(List<string> files, string entryType, string fileFormat)
//        {
//            var testFile = Utils.GetTestSalesFile(files);
//            var fileTypes = FileTypeManager.GetImportableFileType(entryType, fileFormat, testFile);
//            foreach (var fileType in fileTypes)
//            {
//                CSVUtils.SaveCsv(new List<FileInfo>() { new FileInfo(testFile) }, fileType);
//            }
//        }

//        public static TimeSpan Time(Action toTime)
//        {
//            var timer = Stopwatch.StartNew();
//            toTime();
//            timer.Stop();
//            return timer.Elapsed;
//        }
//    }
//}
