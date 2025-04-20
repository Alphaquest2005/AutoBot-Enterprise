using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog; // Added
using OCR.Business.Entities; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        public static string CreateEmail(string file, EmailDownloader.Client client, string error, List<Line> failedlst,
            FileInfo fileInfo, string txtFile)
        {
             _utilsLogger.Debug("CreateEmail called for File: {FilePath}", file);
             if (fileInfo == null)
             {
                 _utilsLogger.Error("CreateEmail failed: FileInfo is null for File: {FilePath}", file);
                 return null; // Cannot proceed without fileInfo
             }
             if (client == null)
             {
                 _utilsLogger.Error("CreateEmail failed: EmailDownloader.Client is null for File: {FilePath}", file);
                 return null; // Cannot proceed without client
             }


            _utilsLogger.Debug("Calling CreateEmailBody for File: {FilePath}", file);
            string body = CreateEmailBody(error, failedlst, fileInfo); // Pass fileInfo directly
             _utilsLogger.Debug("Email body created (Length: {BodyLength}) for File: {FilePath}", body?.Length ?? 0, file);

            _utilsLogger.Debug("Calling SendEmail for File: {FilePath}", file);
            SendEmail(file, client, txtFile, body);

             _utilsLogger.Information("CreateEmail process completed for File: {FilePath}", file);
            return body;
        }

        // Pass FileInfo directly
    }
}