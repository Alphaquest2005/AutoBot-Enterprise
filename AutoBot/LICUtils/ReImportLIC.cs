using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSet are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void ReImportLIC()
        {
            Console.WriteLine("Export Latest PO Entries"); // Log message seems incorrect?
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docset =
                    ctx.TODO_PODocSet.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    // This calls the ImportLicense overload, which needs to be in its own partial class
                    LICUtils.ImportLicense(docset.Declarant_Reference_Number, docset.AsycudaDocumentSetId);
                }
            }
        }
    }
}