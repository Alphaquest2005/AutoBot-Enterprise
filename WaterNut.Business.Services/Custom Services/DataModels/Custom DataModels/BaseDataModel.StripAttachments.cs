using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void StripAttachments(List<DocumentCT> doclst, string emailId)
    {
        try
        {
            using (var ctx = new DocumentItemDSContext())
            {
                foreach (var doc in doclst)
                {
                    var res = ctx.xcuda_Attachments
                        .Include(x => x.Attachments)
                        .Include(x => x.xcuda_Attached_documents.xcuda_Attachments)
                        .Where(x => x.xcuda_Attached_documents != null)
                        .Where(x => x.xcuda_Attached_documents.xcuda_Item.ASYCUDA_Id == doc.Document.ASYCUDA_Id &&
                                    (x.Attachments.EmailId == null || x.Attachments.EmailId != emailId))
                        .ToList();
                    foreach (var itm in res.ToList())
                        ctx.xcuda_Attached_documents.Remove(itm.xcuda_Attached_documents);

                    ctx.SaveChanges();
                }
            }

            using (var ctx = new DocumentDSContext())
            {
                foreach (var doc in doclst)
                {
                    var res = ctx.AsycudaDocument_Attachments
                        .Include(x => x.Attachment)
                        .Where(x => x.AsycudaDocumentId == doc.Document.ASYCUDA_Id &&
                                    (x.Attachment.EmailId == null || x.Attachment.EmailId != emailId)).ToList();
                    foreach (var itm in res.ToList())
                    {
                        ctx.AsycudaDocument_Attachments.Remove(itm);
                        itm.TrackingState = TrackingState.Deleted;
                    }

                    ctx.SaveChanges();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}