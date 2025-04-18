using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void CleanupDocSetDiscpancies(FileTypes fileType)
        {
            try
            {
                Console.WriteLine("Clean Up Discrepancies");
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset =
                            ctx.AsycudaDocumentSetExs.Where(x =>
                                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                                    .ApplicationSettingsId && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                                    .FirstOrDefault();
                    if (docset != null)
                    {
                       // BaseDataModel.CleanUpDiscrepancies(docset.AsycudaDocumentSetId); // CS1061 - Assuming CleanUpDiscrepancies exists on BaseDataModel
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