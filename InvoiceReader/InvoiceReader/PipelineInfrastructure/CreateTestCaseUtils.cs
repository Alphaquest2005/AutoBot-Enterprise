using Core.Common;
using Core.Common.Extensions;
using Core.Common.Utils;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Line

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        public static void CreateTestCase(string file, List<Line> failedlst, string txtFile, string body)
        {
            // Extract context info safely for logging
            string firstInvoiceName = failedlst?.FirstOrDefault()?.OCR_Lines?.Parts?.Templates?.Name ?? "UnknownInvoice";
            _utilsLogger.Debug("CreateTestCase called for File: {FilePath}, InvoiceName: {InvoiceName}", file, firstInvoiceName);

            try
            {
                _utilsLogger.Debug("Calling CreateTestCaseData for File: {FilePath}", file);
                dynamic testCaseData = CreateTestCaseData(file, failedlst, txtFile, body); // Handles its own logging
                _utilsLogger.Debug("TestCaseData created for File: {FilePath}", file);

                _utilsLogger.Debug("Calling LogTestCaseData for File: {FilePath}", file);
                LogTestCaseData(failedlst, testCaseData); // Handles its own logging
                _utilsLogger.Debug("LogTestCaseData finished for File: {FilePath}", file);

                _utilsLogger.Information("Successfully created and logged test case for File: {FilePath}, InvoiceName: {InvoiceName}", file, firstInvoiceName);
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error during CreateTestCase for File: {FilePath}, InvoiceName: {InvoiceName}", file, firstInvoiceName);
                 // Decide if exception should be propagated
            }
        }

        private static void LogTestCaseData(List<Line> failedlst, dynamic testCaseData)
        {
            string invoiceName = "UnknownInvoice";
            string callingClass = "UnknownClass";
            string dataFolder = "UnknownDataFolder";

            try // Wrap potentially failing calls to get context
            {
                 invoiceName = failedlst?.FirstOrDefault()?.OCR_Lines?.Parts?.Templates?.Name ?? invoiceName;
                 callingClass = FunctionLibary.NameOfCallingClass() ?? callingClass;
                 // Added null checks for BaseDataModel path
                 dataFolder = BaseDataModel.Instance?.CurrentApplicationSettings?.DataFolder ?? dataFolder;
            }
            catch (Exception infoEx)
            {
                _utilsLogger.Warning(infoEx, "Could not retrieve full context for logging test case data (InvoiceName/CallingClass/DataFolder).");
            }

            _utilsLogger.Debug("Logging test case data for InvoiceName: {InvoiceName}, CallingClass: {CallingClass}, DataFolder: {DataFolder}",
                invoiceName, callingClass, dataFolder);

            // Log details of what's being passed to UnitTestLogger
            // Use @ destructuring for dynamic objects if the sink supports it (like Console/File with JSON formatter)
            _utilsLogger.Verbose("Test Case Data to log: {@TestCaseData}", testCaseData);

            try
            {
                //write to info
                UnitTestLogger.Log(
                    new List<String> { callingClass, invoiceName }, // Simplified list
                    dataFolder,
                    testCaseData);
                _utilsLogger.Information("Successfully logged test case data via UnitTestLogger for InvoiceName: {InvoiceName}", invoiceName);
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error logging test case data via UnitTestLogger for InvoiceName: {InvoiceName}", invoiceName);
                 // Decide if exception should be propagated
            }
        }

        private static dynamic CreateTestCaseData(string file, List<Line> failedlst, string txtFile, string body)
        {
             // Extract context info safely for logging
             string firstInvoiceName = failedlst?.FirstOrDefault()?.OCR_Lines?.Parts?.Templates?.Name ?? "UnknownInvoice";
             int? firstInvoiceId = failedlst?.FirstOrDefault()?.OCR_Lines?.Parts?.Templates?.Id; // Nullable int

             _utilsLogger.Debug("Creating test case data object for File: {FilePath}, InvoiceName: {InvoiceName}", file, firstInvoiceName);

             dynamic testCaseData = new BetterExpando();
             try
             {
                 testCaseData.DateTime = DateTime.Now;
                 testCaseData.Id = firstInvoiceId; // Use nullable int
                 testCaseData.Supplier = firstInvoiceName;
                 testCaseData.PdfFile = file;
                 testCaseData.TxtFile = txtFile;
                 // Consider logging length or hash instead of full body if it can be very large or contain sensitive info
                 testCaseData.Message = body;
                 _utilsLogger.Verbose("Test case data object created: {@TestCaseData}", testCaseData);
             }
             catch (Exception ex)
             {
                  _utilsLogger.Error(ex, "Error populating test case data object for File: {FilePath}, InvoiceName: {InvoiceName}", file, firstInvoiceName);
                  // Return partially populated or null/empty object depending on requirements
                  return new BetterExpando(); // Return empty expando on error
             }
             return testCaseData;
        }
    }
}