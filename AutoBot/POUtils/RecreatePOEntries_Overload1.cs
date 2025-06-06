using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSet are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void RecreatePOEntries()
        {
            Console.WriteLine("Create PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.TODO_PODocSet
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    // This calls the other RecreatePOEntries overload, which needs to be in its own partial class
                    RecreatePOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }
    }
}