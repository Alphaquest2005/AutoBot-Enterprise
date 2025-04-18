using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using Core.Common.Extensions; // For StringExtensions
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_ImportCompleteEntries, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class PDFUtils
    {
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
                        // Need to define or move ImportPDFComplete method
                        // while (ImportPDFComplete(directoryName, out lcont) == false)
                        // {
                        //     SikuliAutomationService.RunSiKuLi(directoryName, "IM7-PDF", lcont.ToString());
                        // }
                        // Temporarily commenting out the loop relying on ImportPDFComplete
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