using System;
using System.Data.Entity; // For Include
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, Contacts, AsycudaDocumentSetExs, TODO_SubmitPOInfo are here
using MoreLinq; // For MaxBy
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void SubmitPOs()
        {
            try
            {
                Console.WriteLine("Submit POs");

                // var saleInfo = CurrentSalesInfo(); // Assuming CurrentSalesInfo exists if needed

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts
                        .Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();
                    //var sysLst = new DocumentDSContext().SystemDocumentSets.Select(x => x.Id).ToList();   -- dont bother try to filter it
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();

                    var lst = MoreEnumerable.MaxBy(ctx.TODO_SubmitPOInfo
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.FileTypeId != null)
                            // .Where(x => !sysLst.Contains(x.AsycudaDocumentSetId))
                            .Where(x => x.IsSubmitted == false)
                            .Where(x => x.CNumber != null)
                            .Where(x => x.Reference.Contains(docset.Declarant_Reference_Number)) // Potential NullReferenceException if docset is null
                            .ToList(), x => x.AsycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments"),
                            x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z })
                        .ToList();

                    foreach (var doc in lst)
                    {
                        // This calls the other SubmitPOs overload, which needs to be in its own partial class
                        SubmitPOs(doc.z, doc.x.ToList(), contacts, poContacts);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}