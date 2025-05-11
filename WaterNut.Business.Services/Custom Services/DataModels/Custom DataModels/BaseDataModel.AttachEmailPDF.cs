using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void AttachEmailPDF(int asycudaDocumentSetId, string emailId)
    {
        try
        {
            if (emailId == null) return;
            var email = emailId;

            List<xcuda_ASYCUDA> docs;
            List<xcuda_Item> itms;
            List<Attachment> pdfs;
            using (var ctx = new DocumentDSContext())
            {
                var pdfFileTypeInfo = new CoreEntitiesContext().FileImporterInfos.First(x =>
                    x.EntryType == FileTypeManager.EntryTypes.Inv && x.Format == FileTypeManager.FileFormats.PDF);
                docs = ctx.xcuda_ASYCUDA
                    .Include(x => x.AsycudaDocument_Attachments)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ==
                                asycudaDocumentSetId).ToList();
                pdfs = ctx.AsycudaDocumentSet_Attachments
                    .Include(x => x.Attachment)
                    .Where(x => x.Attachment.FilePath.EndsWith("pdf")
                                && x.FileType.FileInfoId != pdfFileTypeInfo.Id
                                && x.AsycudaDocumentSetId == asycudaDocumentSetId
                                && x.EmailId == email)
                    .Select(x => x.Attachment).AsEnumerable().DistinctBy(x => x.FilePath).ToList();

                var nonpdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                        !x.Attachment.FilePath.EndsWith("pdf") && x.FileType.FileInfoId != pdfFileTypeInfo.Id
                                                               && x.AsycudaDocumentSetId == asycudaDocumentSetId
                                                               && x.EmailId == email)
                    .Select(x => x.Attachment).AsEnumerable().DistinctBy(x => x.FilePath).ToList();

                pdfs.AddRange(nonpdfs);
            }

            using (var ctx = new DocumentItemDSContext())
            {
                var list = docs.Select(z => z.ASYCUDA_Id).ToList();
                itms = ctx.xcuda_Item.Where(x => list.Contains(x.ASYCUDA_Id)).ToList();
            }


            foreach (var doc in docs)
                AttachToDocument(pdfs, doc, itms.Where(x => x.ASYCUDA_Id == doc.ASYCUDA_Id).ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}