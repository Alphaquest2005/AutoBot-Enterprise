using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public async Task SaveAttachedDocuments(FileInfo[] csvFiles, FileTypes fileType)
    {
        try
        {
            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var file in csvFiles)
                {
                    var attachment =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).FirstOrDefault(x =>
                            x.Attachments.FilePath == file.FullName &&
                            x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);


                    using (var ctx1 = new CoreEntitiesContext { StartTracking = true })
                    {
                        var reference = GetReference(file, fileType);
                        if (reference == null) continue;
                        if (attachment == null)
                        {
                            if (string.IsNullOrEmpty(reference)) continue;
                            ctx1.AsycudaDocumentSet_Attachments.Add(
                                new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                                {
                                    AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                                    Attachments = new Attachments(true)
                                    {
                                        FilePath = file.FullName,
                                        DocumentCode = fileType.DocumentCode,
                                        Reference = reference,
                                        EmailId = fileType.EmailId
                                    },
                                    DocumentSpecific = fileType.DocumentSpecific,
                                    FileDate = file.LastWriteTime,

                                    FileTypeId = fileType.Id,
                                    TrackingState = TrackingState.Added
                                });
                        }
                        else
                        {
                            attachment.DocumentSpecific = fileType.DocumentSpecific;
                            attachment.FileDate = file.LastWriteTime;
                            attachment.Attachments.Reference = reference;
                            attachment.Attachments.DocumentCode = fileType.DocumentCode;
                            attachment.FileTypeId = fileType.Id;
                        }

                        ctx1.SaveChanges();
                    }
                }

                ctx.SaveChanges();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}