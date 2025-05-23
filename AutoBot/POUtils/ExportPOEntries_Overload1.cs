using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSetToExport are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void ExportPOEntries()
        {
            try
            {
                Console.WriteLine("Export PO Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    foreach (var docset in
                             ctx.TODO_PODocSetToExport
                                 .Where(x => x.ApplicationSettingsId ==
                                             BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    {
                        // This calls the other ExportPOEntries overload, which needs to be in its own partial class
                        ExportPOEntries(docset.AsycudaDocumentSetId);
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
}