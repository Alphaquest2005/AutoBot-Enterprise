using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachements(string file, string emailId,
            int fileTypeId, FileInfo fileInfo,
            AsycudaDocumentSet docSet)
        {
            var existingAttachment = GetDocSetAttachments(file, docSet);

            if (existingAttachment.Any()) return existingAttachment;

            existingAttachment.Add(CreateDocSetAttachment(file, emailId, fileTypeId, fileInfo, docSet));

            return existingAttachment;
        }
    }
}