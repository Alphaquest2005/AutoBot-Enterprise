using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static void AddPreviousDocument(AsycudaDocumentSet currentAsycudaDocumentSet, DocumentCT cdoc,
        PurchaseOrders p,
        List<Attachment> alst)
    {
        var pCnumber = new Regex(@"[C\#]+").Replace(p.PreviousCNumber, "");


        LinkPDFs(new List<string> { pCnumber }, "DO02");
        var pdf = $"{pCnumber}.pdf";
        List<Attachment> previousDocuments;

        previousDocuments = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
            .Where(x => x.Attachment.FilePath.Contains(pdf) && x.Attachment.DocumentCode == "DO02")
            .Select(x => x.Attachment).ToList();
        if (!previousDocuments.Any())
            using (var ctx = new DocumentDSContext())
            {
                previousDocuments = ctx.Attachments
                    .Where(x => x.FilePath.Contains(pdf) &&
                                x.DocumentCode == "NA")
                    .ToList();
                foreach (var itm in previousDocuments.ToList())
                {
                    previousDocuments.Remove(itm);

                    var att = new Attachment
                    {
                        TrackingState = TrackingState.Added,
                        FilePath = itm.FilePath,
                        Reference = pCnumber,
                        DocumentCode = "DO02"
                    };
                    ctx.Attachments.Add(att);

                    ctx.SaveChanges();
                    cdoc.Document.AsycudaDocument_Attachments.Add(
                        new AsycudaDocument_Attachments(true)
                        {
                            AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                            Attachment = att,

                            TrackingState = TrackingState.Added
                        });

                    currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments.Add(
                        new AsycudaDocumentSet_Attachments(true)
                        {
                            AsycudaDocumentSetId = currentAsycudaDocumentSet.AsycudaDocumentSetId,
                            Attachment = att,

                            TrackingState = TrackingState.Added
                        });

                    previousDocuments.Add(att);
                    ctx.SaveChanges();
                }
            }

        alst.AddRange(previousDocuments);
    }
}