using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace AutoBot
{
    using Serilog;
using Serilog.Context;

    public class PDFUtils
    {
        //public static Task ProcessUnknownPDFFileType(FileTypes ft, FileInfo[] fs)
        //{

        //}

        public static async Task AttachEmailPDF(FileTypes ft, FileInfo[] fs)
        {
            await BaseDataModel.AttachEmailPDF(ft.AsycudaDocumentSetId, ft.EmailId).ConfigureAwait(false);
        }


        public static async Task LinkPDFs()
        {
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                            $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}")
                        .Select(x => x.AssessedAsycuda_Id)
                        .Distinct()
                        .ToList();

                 await BaseDataModel.LinkPDFs(entries).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Change signature to async Task<>
        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType, ILogger logger)
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
                    emailId = res?.EmailId ?? fileType.EmailId?? file.Name;
                    fileTypeId = res?.FileTypeId ?? fileType.Id;
                }

                // Await the async call which returns a Dictionary
                var docSets = await WaterNut.DataSpace.Utils.GetDocSets(fileType, logger).ConfigureAwait(false);
                var importResult = await InvoiceReader.InvoiceReader.Import(file.FullName, fileTypeId.GetValueOrDefault(), emailId,
                    true, docSets, fileType, Utils.Client, logger).ConfigureAwait(false);
                // Add the Dictionary directly (AddRange works with Dictionary<TKey, TValue> as it's IEnumerable<KeyValuePair<TKey, TValue>>)


                if (BaseDataModel.Instance.CurrentApplicationSettings.UseAIClassification != true)
                {
                    continue;
                }
                else
                {
                    if (!importResult.Any())
                    {
                        var res2 = await PDFUtils.ImportPDFDeepSeek([file], fileType, logger).ConfigureAwait(false);
                        success.AddRange(res2);
                    }
                    else
                    {
                        var fails = importResult.Select(x => x.Value).Where(x => x.Status == ImportStatus.Failed).ToList();
                        if (fails.Any())
                            fails
                                .ForEach(async x =>
                                {
                                    var res2 = await PDFUtils.ImportPDFDeepSeek([file], fileType, logger).ConfigureAwait(false);
                                    success.AddRange(res2);
                                });
                        else
                            success.AddRange(importResult);
                    }
                }
            }



            return success;
        }

        public static async Task DownloadPDFs()
        {
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                        $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");

                    var lst = entries

                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();
                    foreach (var doc in lst)
                    {
                        var directoryName = StringExtensions.UpdateToCurrentUser(BaseDataModel.GetDocSetDirectoryName(doc.z.Declarant_Reference_Number)); ;
                        Console.WriteLine("Download PDF Files");
                        var lcont = 0;
                        while (ImportPDFComplete(directoryName, out lcont) == false)
                        {
                            Utils.RunSiKuLi(directoryName, "IM7-PDF", lcont.ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool ImportPDFComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            if (File.Exists(Path.Combine(desFolder, "OverView-PDF.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "OverView-PDF.txt"))
                    .Split(new[] { $"\r\n{DateTime.Now.Year}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        : line.Split('\t');
                    if (p.Length < 8) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}.pdf"))
                        && File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}-Assessment.pdf")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                return existingfiles != 0;
            }
            else
            {

                return false;
            }
        }

        public static async Task ReLinkPDFs()
        {
            Console.WriteLine("ReLink PDF Files");
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {

                    var directoryName = StringExtensions.UpdateToCurrentUser(BaseDataModel.GetDocSetDirectoryName("Imports"));


                    var csvFiles = new DirectoryInfo(directoryName).GetFiles($"*.pdf")
                        .Where(x =>
                            //Regex.IsMatch(x.FullName,@".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",RegexOptions.IgnoreCase)&&
                            x.LastWriteTime.ToString("d") == DateTime.Today.ToString("d")).ToArray();

                    foreach (var file in csvFiles)
                    {
                        var mat = Regex.Match(file.FullName,
                            @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                        if (!mat.Success) continue;

                        var dfile = ctx.Attachments.Include(x => x.AsycudaDocument_Attachments).FirstOrDefault(x => x.FilePath == file.FullName);


                        var cnumber = mat.Groups["pCNumber"].Value;
                        var cdoc = ctx.AsycudaDocuments.FirstOrDefault(x => x.CNumber == cnumber);
                        if (cdoc == null) continue;
                        if (dfile != null && dfile.AsycudaDocument_Attachments.Any(x => x.AsycudaDocumentId == cdoc.ASYCUDA_Id)) continue;


                        ctx.AsycudaDocument_Attachments.Add(
                            new AsycudaDocument_Attachments(true)
                            {
                                AsycudaDocumentId = cdoc.ASYCUDA_Id,
                                Attachments = new Attachments(true)
                                {
                                    FilePath = file.FullName,
                                    DocumentCode = "NA",
                                    Reference = file.Name.Replace(file.Extension, ""),
                                    TrackingState = TrackingState.Added

                                },

                                TrackingState = TrackingState.Added
                            });


                      await  ctx.SaveChangesAsync().ConfigureAwait(false);

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //public static Task ConvertPNG2PDF()
        //{
        //    var directoryName = BaseDataModel.GetDocSetDirectoryName("Old Imports");
        //    Console.WriteLine("Convert PNG 2 PDF");
        //    var pngFiles = new DirectoryInfo(directoryName).GetFiles($"*.png");
        //        //.Where(x => x.LastWriteTime.ToString("d") == DateTime.Today.ToString("d")).ToArray();
        //    foreach (var pngFile in pngFiles)
        //    {

        //    }
        //}

        public static async
            Task<List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>>>
            ImportPDFDeepSeek(FileInfo[] fileInfos, FileTypes fileType, ILogger logger)
        {
            //List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>> success = new List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>>();
            var success = new Dictionary<string, (string FileName, string DocumentType, ImportStatus status)>();
            var docTypes = new Dictionary<string, string>()
                { { "Template", "Shipment Template" }, { "CustomsDeclaration", "Simplified Declaration" } };
            foreach (var file in fileInfos)
            {
              var txt = await InvoiceReader.InvoiceReader.GetPdftxt(file.FullName, logger).ConfigureAwait(false);
              var res = await new DeepSeekInvoiceApi(logger).ExtractShipmentInvoice(new List<string>(){txt.ToString()}).ConfigureAwait(false);
              foreach (var doc in res.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList())
                           .GroupBy(x => x["DocumentType"]))
              {
                  var docSet = await WaterNut.DataSpace.Utils.GetDocSets(fileType, logger).ConfigureAwait(false);
                  var docType = docTypes[(doc.Key as string) ?? "Unknown"];
                  var type = await FileTypeManager.GetFileType(FileTypeManager.EntryTypes.GetEntryType(docType),
                      FileTypeManager.FileFormats.PDF, file.FullName).ConfigureAwait(false);
                  var docFileType = type.FirstOrDefault();
                  if (docFileType == null)
                  {
                      continue;
                  }

                  SetFileTypeMappingDefaultValues(docFileType, doc);

                  var import = await ImportSuccessState(file.FullName, fileType.EmailId, docFileType, true,  docSet,
                      new List<dynamic>() { doc.ToList() }, logger).ConfigureAwait(false);
                  success.Add($"{file}-{docType}-{doc.Key}",
                      import
                          ? (file.FullName, FileTypeManager.EntryTypes.GetEntryType(docType), ImportStatus.Success)
                          : (file.FullName, FileTypeManager.EntryTypes.GetEntryType(docType), ImportStatus.Failed));




              }


            }

            return success.ToList();
        }

        private static void SetFileTypeMappingDefaultValues(FileTypes docFileType, IGrouping<object, IDictionary<string, object>> doc)
        {
            foreach (var mapping in docFileType.FileTypeMappings.Where(x => x.FileTypeMappingValues.Any()).ToList())
            {
                doc.ToList().Cast<IDictionary<string, object>>()
                    .Select(x => ((IDictionary<string, object>)x))
                    .Where(x => !x.ContainsKey(mapping.DestinationName))
                    .ForEach(x => x[mapping.DestinationName] = mapping.FileTypeMappingValues.First().Value);
            }
        }

        private static async Task<bool> ImportSuccessState(string file, string emailId, FileTypes fileType, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, List<dynamic> csvLines, ILogger logger)
        {
            using (LogContext.PushProperty("Method", nameof(ImportSuccessState)))
            using (LogContext.PushProperty("File", file))
            using (LogContext.PushProperty("EmailId", emailId))
            using (LogContext.PushProperty("FileTypeId", fileType?.Id))
            using (LogContext.PushProperty("OverwriteExisting", overWriteExisting))
            using (LogContext.PushProperty("DocSetCount", docSet?.Count))
            using (LogContext.PushProperty("CsvLinesCount", csvLines?.Count))
            {
                logger.Information("METHOD_ENTRY: ImportSuccessState. Intention: Process imported data file.");

                try
                {
                    logger.Debug("INTERNAL_STEP: ImportSuccessState - DataFileProcessor.Process. Intention: Invoke DataFileProcessor.");
                    var dataFile = new DataFile(fileType, docSet, overWriteExisting, emailId, file, csvLines, null);
                    logger.Debug("INTERNAL_STEP: ImportSuccessState - DataFileProcessor.Process. InitialState: {DataFileState}", new { FileType = dataFile.FileType?.Description, DocSetCount = dataFile.DocSet?.Count, OverwriteExisting = dataFile.OverWriteExisting, EmailId = dataFile.EmailId, DroppedFilePath = dataFile.DroppedFilePath, DataCount = dataFile.Data?.Count });

                    var processResult = await new DataFileProcessor().Process(dataFile).ConfigureAwait(false);

                    logger.Debug("INTERNAL_STEP: ImportSuccessState - DataFileProcessor.Process. Outcome: {ProcessResult}", processResult);
                    logger.Information("METHOD_EXIT_SUCCESS: ImportSuccessState. IntentionAchieved: Data processing completed. FinalState: {FinalState}", new { ProcessResult = processResult });
                    return processResult;
                }
                catch (Exception e)
                {
                    logger.Error(e, "METHOD_EXIT_FAILURE: ImportSuccessState. IntentionFailed: Data processing failed for file {FileName}.", file);
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// Get file text content for OCR correction
        /// </summary>
        private static async Task<string> GetFileTextAsync(string filePath, ILogger logger)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    logger.Warning("File not found for OCR correction: {FilePath}", filePath);
                    return null;
                }

                // For PDF files, we need to extract text
                if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    // Use the existing PDF text extraction method from InvoiceReader
                    return await InvoiceReader.InvoiceReader.GetPdftxt(filePath, logger).ConfigureAwait(false);
                }
                else
                {
                    // For text files, read directly
                    return File.ReadAllText(filePath);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error reading file text for OCR correction: {FilePath}", filePath);
                return null;
            }
        }
    }
}