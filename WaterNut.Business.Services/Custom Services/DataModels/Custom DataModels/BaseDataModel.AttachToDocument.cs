using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void AttachToDocument(List<Attachment> alst, xcuda_ASYCUDA doc, List<xcuda_Item> itms)
    {
        try
        {
            foreach (var att in alst.OrderBy(x => x.Id))

            {
                if (att.Reference == "Info") continue;
                if (doc.AsycudaDocument_Attachments.FirstOrDefault(x =>
                        x.AsycudaDocumentId == doc.ASYCUDA_Id && x.AttachmentId == att.Id) == null)
                    doc.AsycudaDocument_Attachments.Add(new AsycudaDocument_Attachments(true)
                    {
                        AttachmentId = att.Id,
                        AsycudaDocumentId = doc.ASYCUDA_Id,
                        TrackingState = TrackingState.Added,
                        Attachment = att
                    });


                foreach (var itm in itms)
                {
                    if (itm.xcuda_Attached_documents.Any(x => x.Attached_document_reference == att.Reference))
                        break;
                    itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                    {
                        Attached_document_code = att.DocumentCode,
                        Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                        Attached_document_reference = att.Reference,
                        xcuda_Attachments = new List<xcuda_Attachments>
                        {
                            new xcuda_Attachments(true)
                            {
                                AttachmentId = att.Id,
                                TrackingState = TrackingState.Added,
                                Attachments = new global::DocumentItemDS.Business.Entities.Attachments()
                                {
                                    TrackingState = (att.Id == 0 ? TrackingState.Added : att.TrackingState),
                                    Id = att.Id,
                                    EmailId = att.EmailId,
                                    FilePath = att.FilePath,
                                    Reference = att.Reference,
                                    DocumentCode = att.DocumentCode
                                }
                            }
                        },
                        TrackingState = TrackingState.Added
                    });
                    break;
                }
            }

            if (doc.ASYCUDA_Id == 0) return;

            foreach (var itm in itms)
                using (var docItemCtx = new DocumentItemDSContext { StartTracking = true })
                {
                    foreach (var ad in itm.xcuda_Attached_documents)
                    {
                        if (docItemCtx.xcuda_Attached_documents.FirstOrDefault(x =>
                                x.Item_Id == itm.Item_Id &&
                                x.Attached_document_reference == ad.Attached_document_reference) !=
                            null) continue;
                        ad.Item_Id = itm.Item_Id;
                        docItemCtx.xcuda_Attached_documents.Add(ad);
                    }

                    docItemCtx.SaveChanges();
                }

            using (var ctx = new DocumentDSContext { StartTracking = true })
            {
                foreach (var at in doc.AsycudaDocument_Attachments)
                {
                    if (ctx.AsycudaDocument_Attachments.FirstOrDefault(x =>
                            x.AsycudaDocumentId == doc.ASYCUDA_Id && x.AttachmentId == at.AttachmentId) !=
                        null) continue;
                    at.AsycudaDocumentId = doc.ASYCUDA_Id;
                    ctx.AsycudaDocument_Attachments.Add(at);
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

public partial class BaseDataModel
{
    private static void AttachToDocument(IEnumerable<int> alst, int docId, int itmId)
    {
        try
        {
            xcuda_ASYCUDA doc;
            List<Attachment> attlst;

            List<xcuda_Item> itms;
            using (var docCtx = new DocumentDSContext())
            {
                doc = docCtx.xcuda_ASYCUDA.First(x => x.ASYCUDA_Id == docId);

                attlst = docCtx.Attachments.Where(x => alst.Contains(x.Id)).ToList();
            }

            using (var docItemCtx = new DocumentItemDSContext())
            {
                itms = docItemCtx.xcuda_Item.Include("xcuda_Attached_documents.xcuda_Attachments")
                    .Where(x => x.Item_Id >= itmId).ToList();
            }

            AttachToDocument(attlst, doc, itms);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}