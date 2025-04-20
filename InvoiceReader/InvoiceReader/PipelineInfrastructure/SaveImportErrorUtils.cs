using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public static void SaveImportError(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId, string pdftxt,
            string error, List<Line> failedlst, FileInfo fileInfo)
        {
            var existingAttachment =
                GetOrSaveDocSetAttachmentsList(asycudaDocumentSets, file, emailId, fileTypeId, fileInfo);

            SaveDbErrorDetails(pdftxt, error, failedlst, existingAttachment);
        }
    }
}