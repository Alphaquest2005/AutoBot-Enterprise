using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using ValuationDS.Business.Entities; // Assuming TODO_C71ToCreate is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class C71Utils
    {
        public static void ImportC71(FileTypes ft)
        {
            Console.WriteLine("Import C71");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docSets = ctx.TODO_C71ToCreate
                    //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                    .ToList();
                foreach (var poInfo in docSets)
                {
                    // This calls the other ImportC71 overload, which needs to be in its own partial class
                    ImportC71(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId);
                }
            }
        }
    }
}