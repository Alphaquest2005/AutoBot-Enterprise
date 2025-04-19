using CoreEntities.Business.Entities;
using TrackableEntities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
    private static AsycudaDocumentSet_Attachments CreateDocSetAttachment(string file, string emailId,
        int fileTypeId,
        FileInfo fileInfo, AsycudaDocumentSet docSet)
    {
        using (var ctx = new CoreEntitiesContext())
        {
            var newAttachment = new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
            {
                TrackingState = TrackingState.Added,
                AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                Attachments = new Attachments(true)
                {
                    TrackingState = TrackingState.Added,
                    EmailId = emailId,
                    FilePath = file
                },
                DocumentSpecific = true,
                FileDate = fileInfo.CreationTime,
                FileTypeId = fileTypeId,
            };
            ctx.AsycudaDocumentSet_Attachments.Add(newAttachment);
            ctx.SaveChanges();
            return newAttachment;
        }
    }
}