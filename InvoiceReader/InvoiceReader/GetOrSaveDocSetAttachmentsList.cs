using CoreEntities.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    private static List<AsycudaDocumentSet_Attachments> GetOrSaveDocSetAttachmentsList(
        List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
        int fileTypeId, FileInfo fileInfo) =>
        asycudaDocumentSets.SelectMany(docSet => GetDocSetAttachements(file, emailId, fileTypeId, fileInfo, docSet))
            .ToList();
}