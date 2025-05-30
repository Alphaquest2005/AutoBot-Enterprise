using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version
using AutoBot.Services;

namespace AutoBot
{
    public partial class EntryDocSetUtils
    {
        public static void ClearDocSetEntries(FileTypes fileType)
        {
            try
            {
                Console.WriteLine($"Clear {fileType.FileImporterInfos.EntryType} Entries");

                //   var saleInfo = CurrentSalesInfo();
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Result;
                string directoryName = BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number);

                var instFile = Path.Combine(directoryName, "Instructions.txt");
                var resFile = Path.Combine(directoryName, "InstructionResults.txt");
                if (File.Exists(resFile))
                {
                    var resTxt = File.ReadAllText(resFile);

                    foreach (var doc in docSet.Documents.ToList())
                    {
                        if (!resTxt.Contains(doc.ReferenceNumber))
                            BaseDataModel.Instance.DeleteAsycudaDocument(doc).Wait();
                    }

                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId,
                            docSet.Documents.Count());
                }
                else
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId, 0);

                    // if (File.Exists(resFile)) File.Delete(resFile);
                }

                ClearDocSetEntryData(fileType.AsycudaDocumentSetId);
                ClearDocSetAttachments(fileType.AsycudaDocumentSetId, fileType.EmailId);
                if (File.Exists(instFile)) File.Delete(instFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}