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
        public static void AddUpdateDocSetAttachement(FileTypes fileType, Email email, CoreEntitiesContext ctx, FileInfo file,
                Attachments attachment, string newReference)
        {
            var docSetAttachment =
                    ctx.AsycudaDocumentSet_Attachments
                            .Include(x => x.Attachments)
                            .FirstOrDefault(x => x.Attachments.FilePath == file.FullName
                                                                    && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
            if (docSetAttachment == null)
            {
                ctx.AsycudaDocumentSet_Attachments.Add(
                        new AsycudaDocumentSet_Attachments(true)
                        {
                            AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                            Attachments = attachment,
                            DocumentSpecific = fileType.DocumentSpecific,
                            FileDate = file.LastWriteTime,
                            EmailId = email.EmailId,
                            FileTypeId = fileType.Id,
                            TrackingState = TrackingState.Added
                        });
            }
            else
            {
                docSetAttachment.DocumentSpecific = fileType.DocumentSpecific;
                docSetAttachment.FileDate = file.LastWriteTime;
                docSetAttachment.EmailId = email.EmailId;
                docSetAttachment.FileTypeId = fileType.Id;
                docSetAttachment.Attachments.Reference = newReference;
                docSetAttachment.Attachments.DocumentCode = fileType.DocumentCode;
                docSetAttachment.Attachments.EmailId = email.EmailId;
            }

            ctx.SaveChanges();
        }
    }
}