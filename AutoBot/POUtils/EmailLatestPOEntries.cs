using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void EmailLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.AsycudaDocumentSetExs.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();

                if (docset != null)
                {
                    // This calls EmailPOEntries, which needs to be in its own partial class
                    EmailPOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }
    }
}