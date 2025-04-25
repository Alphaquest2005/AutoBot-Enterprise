
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailDownloader;
using OCR.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using WaterNut.DataSpace.PipelineInfrastructure;
using Serilog;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace InvoiceReader
{
    public class InvoiceReader
    {
        private static readonly ILogger _logger = Log.ForContext<InvoiceReader>();

        public static Client Client { get; set; } = new Client
        {
            CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
            DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
            Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
            Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
            EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList(),
            DevMode = true
        };

        public static string CommandsTxt => InvoiceProcessingUtils.CommandsTxt;


        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
        {
            LogStartPDFImport(pdfFiles.Length, fileType);

            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
       
           
            foreach (var file in pdfFiles.Where(x => x.Extension.ToLower() == ".pdf"))
            {
                string emailId = null;
                int? fileTypeId = 0;
                LogProcessingFile(file.FullName);

                using (var ctx = new CoreEntitiesContext())
                {
                    LogQueryingDatabase();
                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == file.FullName)
                        .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                    emailId = res?.EmailId ?? fileType.EmailId ?? file.Name;
                    fileTypeId = res?.FileTypeId ?? fileType.Id;

                    LogResolvedEmailAndFileType(emailId, fileTypeId);
                }

                
                success.AddRange(await Import(file.FullName,fileType.Id,emailId,true, null, fileType, Client).ConfigureAwait(false));
            }



            LogPDFImportCompleted(success.Count);
            return success;
        }

        public static async Task<List<KeyValuePair<string, (string file, string, ImportStatus Success)>>> Import(string fileFullName, int fileTypeId, string emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSets, FileTypes fileType, Client client)
        {
      
            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            var failedFiles = new List<string>();

            LogCreatingInvoiceProcessingContext();
            var context = new InvoiceProcessingContext
            {
                FilePath = fileFullName,
                FileInfo = new FileInfo(fileFullName),
                FileTypeId = fileTypeId,
                EmailId = emailId,
                OverWriteExisting = overWriteExisting,
                DocSet = docSets, // Added docSets parameter to set template docSet
                FileType = fileType,
                Client = client,
                PdfText = new StringBuilder(),
                Imports = new Dictionary<string, (string file, string, ImportStatus Success)>()
            };

            LogStartingPipeline(fileFullName);
            var pipe = new InvoiceProcessingPipeline(context, false);
            var pipeResult = await pipe.RunPipeline().ConfigureAwait(false);

            if(!pipeResult) failedFiles.Add(fileFullName);

            LogPipelineCompleted(context.Imports.Count);

            success = context.Imports.ToList();
            return success;
        }

        public static async Task<string> GetPdftxt(string fileFullName)
        {
            var context = new InvoiceProcessingContext
            {
                FilePath = fileFullName,
                FileInfo = new FileInfo(fileFullName),
                PdfText = new StringBuilder()
            };
             await new GetPdfTextStep().Execute(context).ConfigureAwait(false);
             return context.PdfText.ToString();
        }

        public static bool IsInvoiceDocument(Invoices invoice, string fileText, string fileName)
        {
            return GetPossibleInvoicesStep.IsInvoiceDocument(invoice, fileText, fileName);
        }

        private static void LogStartPDFImport(int fileCount, FileTypes fileType)
        {
            _logger.Information("Starting PDF import for {FileCount} files with FileType: {FileTypeName} (ID: {FileTypeId})",
                fileCount, fileType.Description, fileType.Id);
        }

        private static void LogProcessingFile(string fileName)
        {
            _logger.Debug("Processing file: {FileName}", fileName);
        }

        private static void LogQueryingDatabase()
        {
            _logger.Debug("Querying database for existing attachments matching file path");
        }

        private static void LogResolvedEmailAndFileType(string emailId, int? fileTypeId)
        {
            _logger.Debug("Resolved EmailId: {EmailId}, FileTypeId: {FileTypeId}", emailId, fileTypeId);
        }

        private static void LogCreatingInvoiceProcessingContext()
        {
            _logger.Debug("Creating InvoiceProcessingContext");
        }

        private static void LogStartingPipeline(string fileName)
        {
            _logger.Information("Starting invoice processing pipeline for file: {FileName}", fileName);
        }

        private static void LogPipelineCompleted(int importCount)
        {
            _logger.Information("Pipeline completed with {ImportCount} imports", importCount);
        }

        private static void LogPDFImportCompleted(int successCount)
        {
            _logger.Information("PDF import completed with {SuccessCount} successful imports", successCount);
        }


  
    }
}
