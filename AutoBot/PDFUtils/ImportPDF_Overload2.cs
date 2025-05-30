using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, AsycudaDocumentSet_Attachments are here
using WaterNut.Business.Services.Utils; // Assuming InvoiceReader is here
using WaterNut.DataSpace; // Assuming Utils is here
using AutoBot.Services; // Assuming ImportStatus is here (or needs to be defined/moved)

namespace AutoBot
{
    public partial class PDFUtils
    {
        // Change signature to async Task<>
        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
            //(int? fileTypeId, int? emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSet, string fileType)
        {
            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> success = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            Console.WriteLine("Importing PDF " + fileType.FileImporterInfos.EntryType); // Assuming FileImporterInfos exists on FileTypes
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
                // Assuming Utils.Client exists and is accessible
                var importResult = await InvoiceReader.Import(file.FullName, fileTypeId.GetValueOrDefault(), emailId, true, WaterNut.DataSpace.Utils.GetDocSets(fileType), fileType, Utils.Client).ConfigureAwait(false);
                // Add the Dictionary directly (AddRange works with Dictionary<TKey, TValue> as it's IEnumerable<KeyValuePair<TKey, TValue>>)
                success.AddRange(importResult);
            }

            return success;
        }
    }
}