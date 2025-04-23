using AutoBot;
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using WaterNut.DataSpace.PipelineInfrastructure;
using Utils = AutoBot.Utils;
using Serilog;

namespace InvoiceReader
{
    public class InvoiceReader
    {
        private static readonly ILogger _logger = Log.ForContext<InvoiceReader>();

        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
        {
            LogStartPDFImport(pdfFiles.Length, fileType);

            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            var failedFiles = new List<string>();
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

                LogCreatingInvoiceProcessingContext();
                var context = new InvoiceProcessingContext
                {
                    FilePath = file.FullName,
                    FileInfo = new FileInfo(file.FullName),
                    FileTypeId = fileTypeId.GetValueOrDefault(),
                    EmailId = emailId,
                    OverWriteExisting = true,
                    FileType = fileType,
                    Client = Utils.Client,
                    PdfText = new StringBuilder(),
                    Imports = new Dictionary<string, (string file, string, ImportStatus Success)>()
                };

                LogStartingPipeline(file.Name);
                var pipe = new InvoiceProcessingPipeline(context, false);
                var pipeResult = await pipe.RunPipeline().ConfigureAwait(false);
                LogPipelineCompleted(context.Imports.Count);

                success = context.Imports.ToList();
            }

            LogPDFImportCompleted(success.Count);
            return success;
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
