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
        public static void LinkEmail()
        {
            try
            {
                Console.WriteLine("Link Emails");
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                            $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");

                    // var entries = ctx.TODO_ImportCompleteEntries
                    //         .Where(x => x.ApplicationSettingsId ==
                    //                                 BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    var lst = entries
                            .GroupBy(x => x.AsycudaDocumentSetId)
                            .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();

                    foreach (var itm in entries)
                    {
                        var idoc = ctx.AsycudaDocuments.First(x => x.ASYCUDA_Id == itm.AssessedAsycuda_Id);
                        var cdoc = ctx.AsycudaDocuments.First(x => x.ReferenceNumber == idoc.ReferenceNumber);

                        if (cdoc == null) continue;

                        // ctx.AsycudaDocument_Attachments.Add(
                        //         new AsycudaDocument_Attachments(true)
                        //         {
                        //                 AsycudaDocumentId = cdoc.ASYCUDA_Id,
                        //                 Attachments = new Attachments(true)
                        //                 {
                        //                         FilePath = file.FullName,
                        //                         DocumentCode = "NA",
                        //                         Reference = file.Name.Replace(file.Extension, ""),
                        //                         TrackingState = TrackingState.Added

                        //                 },

                        //                 TrackingState = TrackingState.Added
                        //         });

                        // }
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