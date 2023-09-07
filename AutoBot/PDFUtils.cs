using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using TrackableEntities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class PDFUtils
    {
        public static void ProcessUnknownPDFFileType(FileTypes ft, FileInfo[] fs)
        {
            
        }

        public static void AttachEmailPDF(FileTypes ft, FileInfo[] fs)
        {
            BaseDataModel.AttachEmailPDF(ft.AsycudaDocumentSetId, ft.EmailId);
        }

        public static void ImportPDF()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes

                    .FirstOrDefault(x => x.Id == 17);
                var files = new FileInfo[]
                    {new FileInfo(@"D:\OneDrive\Clients\Budget Marine\Emails\30-16170\7006359.pdf")};
                ImportPDF(files, fileType);
            }
        }

        public static void LinkPDFs()
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

                    //var entries = ctx.TODO_ImportCompleteEntries
                    //    .Where(x => x.ApplicationSettingsId ==
                    //                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    //    .Select(x => x.AssessedAsycuda_Id)
                    //    .Distinct()
                    //    .ToList();


                    BaseDataModel.LinkPDFs(entries);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
            //(int? fileTypeId, int? emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSet, string fileType)
        {
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
                    emailId = res?.EmailId;
                    fileTypeId = res?.FileTypeId;
                }
                var success = InvoiceReader.Import(file.FullName, fileTypeId.GetValueOrDefault(), emailId, true, WaterNut.DataSpace.Utils.GetDocSets(fileType), fileType, Utils.Client);
               
            }
        }

        public static void DownloadPDFs()
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

        public static void ReLinkPDFs()
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




                        ctx.SaveChanges();

                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ConvertPNG2PDF()
        {
            var directoryName = BaseDataModel.GetDocSetDirectoryName("Old Imports");
            Console.WriteLine("Convert PNG 2 PDF");
            var pngFiles = new DirectoryInfo(directoryName).GetFiles($"*.png");
                //.Where(x => x.LastWriteTime.ToString("d") == DateTime.Today.ToString("d")).ToArray();
            foreach (var pngFile in pngFiles)
            {

            }
        }
    }
}