using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Extensions; // For StringExtensions
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, Attachments, AsycudaDocument_Attachments, AsycudaDocuments are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class PDFUtils
    {
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
    }
}