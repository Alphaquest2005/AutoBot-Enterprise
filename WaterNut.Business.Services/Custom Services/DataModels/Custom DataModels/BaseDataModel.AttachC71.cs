using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using ValuationDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static void AttachC71(int asycudaDocumentSetId)
    {
        using (var ctx = new CoreEntitiesContext())
        {
            var lst = ctx.AsycudaItemBasicInfo
                .Where(x => x.LineNumber == 1
                            && x.AsycudaDocumentSetId ==
                            asycudaDocumentSetId).ToList();
            var c71Att = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).Where(x =>
                    x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71 &&
                    x.AsycudaDocumentSetId == asycudaDocumentSetId)
                .Select(x => x.Attachments).AsEnumerable().DistinctBy(x => x.FilePath).Where(x =>
                    new FileInfo(x.FilePath).Name != $"{FileTypeManager.EntryTypes.C71}.xml" &&
                    x.Reference != FileTypeManager.EntryTypes.C71).ToList();

            if (!c71Att.Any())
            {
                var eC71 = ctx.AsycudaDocumentSetC71.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (eC71 != null)
                {
                    var att = ctx.Attachments.FirstOrDefault(x => x.Id == eC71.AttachmentId);
                    if (att == null)
                    {
                        att = new Attachments
                        {
                            TrackingState = TrackingState.Added,
                            FilePath = eC71.FilePath,
                            DocumentCode = "DC05",
                            Reference = eC71.RegNumber
                        };
                        ctx.Attachments.Add(att);
                    }

                    c71Att = new List<Attachments> { att };

                    ctx.AsycudaDocumentSet_Attachments.Add(
                        new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                        {
                            AsycudaDocumentSetId = asycudaDocumentSetId,
                            AttachmentId = att.Id
                        });
                    ctx.SaveChanges();
                }
            }


            var res = new Dictionary<Attachments, ValuationDS.Business.Entities.Registered>();
            var registeredC71s = new ValuationDSContext().xC71_Value_declaration_form
                .OfType<ValuationDS.Business.Entities.Registered>()
                .Include(x => x.xC71_Item)
                .Include(x => x.xC71_Identification_segment).ToList();
            var asycudaDocumentItemEntryDataDetails = lst.GroupJoin(ctx.AsycudaDocumentItemEntryDataDetails,
                x => x.Item_Id, e => e.Item_Id,
                (x, e) => new { x.Item_Id, x.LineNumber, x.ASYCUDA_Id, data = e.Select(z => z.key) }).ToList();
            foreach (var i in c71Att)
            {
                var c71 = registeredC71s
                    .FirstOrDefault(x => x.SourceFile == i.FilePath);
                if (c71 != null) res.Add(i, c71);
            }

            foreach (var al in res)
            {
                foreach (var c71Item in al.Value.xC71_Item)
                {
                    var itms = asycudaDocumentItemEntryDataDetails
                        .Where(x =>
                            x.data.Any(z =>
                                z.Contains(c71Item.Invoice_Number)) &&
                            x.LineNumber == 1).ToList();

                    foreach (var itm in itms)
                        AttachToDocument(new List<int> { al.Key.Id },
                            itm.ASYCUDA_Id, itm.Item_Id);
                }
            }
        }
    }
}