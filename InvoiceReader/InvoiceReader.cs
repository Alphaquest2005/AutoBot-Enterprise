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
            _logger.Information("Starting PDF import for {FileCount} files with FileType: {FileTypeName} (ID: {FileTypeId})",
                pdfFiles.Length, fileType.Description, fileType.Id);
            
            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            var failedFiles = new List<string>();
            foreach (var file in pdfFiles.Where(x => x.Extension.ToLower() == ".pdf"))
            {
                string emailId = null;
                int? fileTypeId = 0;
                _logger.Debug("Processing file: {FileName}", file.FullName);
                
                using (var ctx = new CoreEntitiesContext())
                {
                    _logger.Debug("Querying database for existing attachments matching file path");
                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == file.FullName)
                        .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                    emailId = res?.EmailId ?? fileType.EmailId;
                    fileTypeId = res?.FileTypeId ?? fileType.Id;
                    
                    _logger.Debug("Resolved EmailId: {EmailId}, FileTypeId: {FileTypeId}", emailId, fileTypeId);
                }

                // Await the async call which returns a Dictionary

                _logger.Debug("Creating InvoiceProcessingContext");
                var context = new InvoiceProcessingContext
                {
                    FilePath = file.FullName,
                    FileTypeId = fileTypeId.GetValueOrDefault(),
                    EmailId = emailId,
                    OverWriteExisting = true,
                    DocSet = WaterNut.DataSpace.Utils.GetDocSets(fileType),
                    FileType = fileType,
                    Client = Utils.Client,
                    PdfText = new StringBuilder(),
                    FormattedPdfText = string.Empty,
                    Imports = new Dictionary<string, (string file, string, ImportStatus Success)>()
                };
                
                _logger.Information("Starting invoice processing pipeline for file: {FileName}", file.Name);
                var pipe = new InvoiceProcessingPipeline(context, false);
                var pipeResult = await pipe.RunPipeline().ConfigureAwait(false);
                _logger.Information("Pipeline completed with {ImportCount} imports", context.Imports.Count);



                success = context.Imports.ToList();

                // if (!context.Imports.Values.Any())
                // {
                //     _logger.Warning("No imports from pipeline, attempting fallback with PDFUtils.ImportPDFDeepSeek");
                //     var res2 = await PDFUtils.ImportPDFDeepSeek(new FileInfo[] { file }, fileType).ConfigureAwait(false);
                //     _logger.Information("PDFUtils.ImportPDFDeepSeek returned {ResultCount} results", res2.Count);
                //     // Project res2 to match the tuple structure of 'success'
                //     // Explicitly create a new list with the target tuple structure
                //     var convertedRes2 = res2.Select(kvp => {
                //         // Explicitly create the target tuple type first
                //         (string file, string, ImportStatus Success) targetTuple =
                //             (file: kvp.Value.FileName,
                //              kvp.Value.DocumentType,
                //              // Explicitly cast the enum to the target type
                //              Success: (WaterNut.DataSpace.ImportStatus)(int)kvp.Value.status);
                //         // Now create the KeyValuePair using the correctly typed tuple
                //         return new KeyValuePair<string, (string file, string, ImportStatus Success)>(kvp.Key, targetTuple);
                //     }).ToList(); // Materialize the list
                //     success.AddRange(convertedRes2);
                // }
                // else
                // {
                //     var fails = context.Imports.Values.Where(x => x.Success == ImportStatus.Failed).ToList();
                //     if (fails.Any())
                //         fails
                //             .ForEach(async x =>
                //             {
                //                 _logger.Warning("Failed imports detected, attempting fallback with PDFUtils.ImportPDFDeepSeek");
                //                 var res2 = await PDFUtils.ImportPDFDeepSeek(new FileInfo[] { file }, fileType).ConfigureAwait(false);
                //                 _logger.Information("PDFUtils.ImportPDFDeepSeek fallback returned {ResultCount} results", res2.Count);
                //                 // Project res2 to match the tuple structure of 'success'
                //                 // Explicitly create a new list with the target tuple structure
                //                 var convertedRes2Loop = res2.Select(kvp => {
                //                     // Explicitly create the target tuple type first
                //                     (string file, string, ImportStatus Success) targetTuple =
                //                         (file: kvp.Value.FileName,
                //                          kvp.Value.DocumentType,
                //                          // Explicitly cast the enum to the target type
                //                          Success: (WaterNut.DataSpace.ImportStatus)(int)kvp.Value.status);
                //                     // Now create the KeyValuePair using the correctly typed tuple
                //                     return new KeyValuePair<string, (string file, string, ImportStatus Success)>(kvp.Key, targetTuple);
                //                 }).ToList(); // Materialize the list
                //                 success.AddRange(convertedRes2Loop);
                //             });
                //     else
                //         success.AddRange(context.Imports);
                // }
            }



            _logger.Information("PDF import completed with {SuccessCount} successful imports", success.Count);
            return success;
        }
    }
}
