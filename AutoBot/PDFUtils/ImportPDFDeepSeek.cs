using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils; // Assuming LoggingConfig is here
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using MoreLinq; // For ForEach
using WaterNut.Business.Services.Utils; // Assuming InvoiceReader, DeepSeekInvoiceApi, FileTypeManager are here
using WaterNut.DataSpace; // Assuming Utils is here
using AutoBot.Services; // Assuming ImportStatus is here (or needs definition/move)

namespace AutoBot
{
    public partial class PDFUtils
    {
        public static async Task<List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>>> ImportPDFDeepSeek(FileInfo[] fileInfos, FileTypes fileType)
        {
            //List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>> success = new List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>>();
            var success = new Dictionary<string, (string FileName, string DocumentType, ImportStatus status)>();
            var logger = LoggingConfig.CreateLogger();
            var docTypes = new Dictionary<string, string>()
                { { "Invoice", "Shipment Invoice" }, { "CustomsDeclaration", "Simplified Declaration" } };
            foreach (var file in fileInfos)
            {
              var txt = InvoiceReader.GetPdftxt(file.FullName);
              var res = await new DeepSeekInvoiceApi().ExtractShipmentInvoice(new List<string>(){txt.ToString()}).ConfigureAwait(false);
              foreach (var doc in res.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList())
                           .GroupBy(x => x["DocumentType"]))
              {
                  var docSet = WaterNut.DataSpace.Utils.GetDocSets(fileType); // Assuming Utils.GetDocSets exists
                  var docType = docTypes[(doc.Key as string) ?? "Unknown"];
                  var docFileType = FileTypeManager.GetFileType(FileTypeManager.EntryTypes.GetEntryType(docType),
                      FileTypeManager.FileFormats.PDF, file.FullName).FirstOrDefault();
                  if (docFileType == null)
                  {
                      continue;
                  }

                  // Need to define or move SetFileTypeMappingDefaultValues method
                  // SetFileTypeMappingDefaultValues(docFileType, doc);

                  // Need to define or move ImportSuccessState method
                  // var import = await ImportSuccessState(file.FullName, fileType.EmailId, docFileType, true, docSet,
                  //     new List<dynamic>() { doc.ToList() }).ConfigureAwait(false);
                  // success.Add($"{file}-{docType}-{doc.Key}",
                  //     import
                  //         ? (file.FullName, FileTypeManager.EntryTypes.GetEntryType(docType), ImportStatus.Success)
                  //         : (file.FullName, FileTypeManager.EntryTypes.GetEntryType(docType), ImportStatus.Failed));
                  // Temporarily commenting out calls to missing methods
              }
            }

            return success.ToList();
        }
    }
}