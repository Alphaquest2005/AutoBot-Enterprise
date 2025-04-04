using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using LicenseDS.Business.Entities; // Assuming TODO_LICToCreate is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void ImportLicense(FileTypes ft)
        {
            Console.WriteLine("Import License");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docSets = ctx.TODO_LICToCreate
                    //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                    .ToList();

                foreach (var poInfo in docSets)
                {
                    // This calls the other ImportLicense overload, which needs to be in its own partial class
                    ImportLicense(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId);
                }
            }
        }
    }
}