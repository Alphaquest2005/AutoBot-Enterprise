using AutoBot;
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using Utils = AutoBot.Utils;

namespace InvoiceReader
{
    public class InvoiceReader
    {
        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
        //(int? fileTypeId, int? emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSet, string fileType)
        {
            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            Console.WriteLine("Importing PDF " + fileType.FileImporterInfos.EntryType);
            var failedFiles = new List<string>();
            foreach (var file in pdfFiles.Where(x => x.Extension.ToLower() == ".pdf"))
            {
                string emailId = null;
                int? fileTypeId = 0;
                using (var ctx = new CoreEntitiesContext())
                {

                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == file.FullName)
                        .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                    emailId = res?.EmailId ?? fileType.EmailId;
                    fileTypeId = res?.FileTypeId ?? fileType.Id;
                }

                // Await the async call which returns a Dictionary

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
                var pipe = new InvoiceProcessingPipeline(context, false);
                var pipeResult = await pipe.RunPipeline().ConfigureAwait(false);



                success = context.Imports.ToList();

                if (!context.Imports.Values.Any())
                {
                    var res2 = await PDFUtils.ImportPDFDeepSeek([file], fileType).ConfigureAwait(false);
                    success.AddRange((IEnumerable<KeyValuePair<string, (string file, string, ImportStatus Success)>>)res2);
                }
                else
                {
                    var fails = context.Imports.Values.Where(x => x.Success == ImportStatus.Failed).ToList();
                    if (fails.Any())
                        fails
                            .ForEach(async x =>
                            {
                                var res2 = await PDFUtils.ImportPDFDeepSeek([file], fileType).ConfigureAwait(false);
                                success.AddRange((IEnumerable<KeyValuePair<string, (string file, string, ImportStatus Success)>>)res2);
                            });
                    else
                        success.AddRange(context.Imports);
                }
            }



            return success;
        }
    }
}
