using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    private static void SaveImportError(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
        int fileTypeId, string pdftxt,
        string error, List<Line> failedlst, FileInfo fileInfo)
    {
        var existingAttachment =
            GetOrSaveDocSetAttachmentsList(asycudaDocumentSets, file, emailId, fileTypeId, fileInfo);

        SaveDbErrorDetails(pdftxt, error, failedlst, existingAttachment);
    }
}