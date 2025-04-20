using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        private static List<AsycudaDocumentSet_Attachments> GetOrSaveDocSetAttachmentsList(
            List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId, FileInfo fileInfo) =>
            asycudaDocumentSets.SelectMany(docSet => GetDocSetAttachements(file, emailId, fileTypeId, fileInfo, docSet))
                .ToList();
    }
}