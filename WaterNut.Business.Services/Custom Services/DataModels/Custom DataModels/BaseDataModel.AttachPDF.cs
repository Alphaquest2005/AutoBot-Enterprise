using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using MoreLinq.Extensions;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static void AttachPDF(int asycudaDocumentSetId)
    {
        List<xcuda_ASYCUDA> docs;
        List<xcuda_Item> itms;
        List<Attachment> pdfs;
        using (var ctx = new DocumentDSContext())
        {
            docs = ctx.xcuda_ASYCUDA.Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ==
                                                asycudaDocumentSetId).ToList();
            pdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                    x.Attachment.FilePath.ToLower().EndsWith("pdf") &&
                    x.FileType.DocumentSpecific != true &&
                    x.AsycudaDocumentSetId == asycudaDocumentSetId)
                .Select(x => x.Attachment).AsEnumerable().OrderByDescending(x => x.Id)
                .Where(x => File.Exists(x.FilePath)).DistinctBy(x => new FileInfo(x.FilePath).Name).ToList();

            var nonpdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                    !x.Attachment.FilePath.ToLower().EndsWith("pdf")
                    && !x.Attachment.FilePath.ToLower().Contains("xml")
                    && !x.Attachment.FilePath.ToLower().Contains("Info.txt".ToLower())
                    && x.FileType.DocumentSpecific != true
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId)
                .Select(x => x.Attachment).AsEnumerable().OrderByDescending(x => x.Id)
                .Where(x => File.Exists(x.FilePath)).DistinctBy(x => new FileInfo(x.FilePath).Name).ToList();

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
}