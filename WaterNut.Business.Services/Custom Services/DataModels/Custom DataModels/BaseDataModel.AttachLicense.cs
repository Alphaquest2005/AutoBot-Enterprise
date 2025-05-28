using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using LicenseDS.Business.Entities;
using MoreLinq.Extensions;
using Attachments = CoreEntities.Business.Entities.Attachments;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static async Task AttachLicense(int asycudaDocumentSetId)
    {
        try
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var allocatedItms = new List<LicenceDocItem>();
                var lst = ctx.AsycudaDocumentItems
                    .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                    .Where(x => x.TariffCodeLicenseRequired == true
                                && x.AsycudaDocument.AsycudaDocumentSetId ==
                                asycudaDocumentSetId)
                    .Select(x => new LicenceDocItem
                    {
                        Item_Id = x.Item_Id,
                        Details = x.AsycudaDocumentItemEntryDataDetails.Select(z => z.key).ToList(),
                        TariffCode = x.TariffCode,
                        ItemQuantity = x.ItemQuantity,
                        AsycudaDocumentId = x.AsycudaDocumentId
                    })
                    .ToList();
                if (!lst.Any()) return;
                var docSet = await Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).ConfigureAwait(false);

                ///// Scape any remaining license because i preapply and will still apply for new license when creating docset

                var licAtt = new List<Attachments>();

                //Add available license
                var availableLic = ctx.TODO_LicenceAvailableQty.Where(x =>
                    x.ApplicationSettingsId == Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.Origin == docSet.Country_of_origin_code).OrderBy(x => x.Application_date).ToList();
                foreach (var lic in availableLic)
                {
                    var attlst = ctx.Attachments.Where(x =>
                            x.DocumentCode == "LC02"
                            && x.FilePath == lic.SourceFile)
                        .DistinctBy(x => x.FilePath)
                        .Where(x => x.Reference != "LIC").ToList();
                    licAtt.AddRange(attlst);
                    foreach (var att in attlst)
                        ctx.AsycudaDocumentSet_Attachments.Add(
                            new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                            {
                                AsycudaDocumentSetId = asycudaDocumentSetId,
                                AttachmentId = att.Id
                            });
                    ctx.SaveChanges();
                }


                var res = new Dictionary<Attachments, Registered>();
                foreach (var i in licAtt.DistinctBy(x => x.Reference))
                {
                    var xLicLicense = new LicenseDSContext().xLIC_License.OfType<Registered>()
                        .Include("xLIC_Lic_item_segment.TODO_LicenceAvailableQty")
                        .Include(x => x.xLIC_General_segment)
                        .FirstOrDefault(x => x.SourceFile == i.FilePath);
                    if (xLicLicense != null) res.Add(i, xLicLicense);
                }

                foreach (var al in res)
                {
                    EntryData entryDataId = null;


                    foreach (var lic in al.Value.xLIC_Lic_item_segment)
                    {
                        var truncate = lic.Commodity_code.Truncate(8);
                        var itms = entryDataId == null
                            ? lst.Where(x => x.TariffCode == truncate).ToList()
                            : lst.Where(x =>
                                    x.TariffCode == truncate &&
                                    x.Details.Any(z =>
                                        z.Contains(entryDataId.EntryDataId)))
                                .ToList();

                        double rtotal = lic.TODO_LicenceAvailableQty == null
                            ? lic.Quantity_to_approve // assume all so it wont pass later 
                            : lic.Quantity_to_approve - lic.TODO_LicenceAvailableQty.Balance;


                        foreach (var itm in itms)
                        {
                            if (allocatedItms.Any(z => z.Item_Id == itm.Item_Id)) continue;
                            if (itm.ItemQuantity <= lic.Quantity_to_approve - rtotal &&
                                itm.TariffCode == lic.Commodity_code.Truncate(8))
                            {
                                rtotal += itm.ItemQuantity.GetValueOrDefault();
                                await AttachToDocument(new List<int> { al.Key.Id },
                                    itm.AsycudaDocumentId.GetValueOrDefault(), itm.Item_Id).ConfigureAwait(false);
                                allocatedItms.Add(itm);
                            }
                        }
                    }
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