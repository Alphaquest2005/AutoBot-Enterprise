using System.Data.Entity;
using CoreEntities.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachments(string file, AsycudaDocumentSet docSet)
    {
        using (var ctx = new CoreEntitiesContext())
        {
            var docSetAttachments = ctx.AsycudaDocumentSet_Attachments
                .Include(x => x.Attachments)
                .Where(x =>
                    x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId && x.Attachments.FilePath == file)
                .ToList();
            return docSetAttachments;
        }
    }
}