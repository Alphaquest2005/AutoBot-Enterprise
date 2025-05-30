using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static AsycudaDocumentSetEx GetAsycudaDocumentSetEx(FileTypes fileType)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                                .ApplicationSettingsId && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                                .OrderByDescending(x => x.AsycudaDocumentSetId)
                                .FirstOrDefault();
                return docset;
            }
        }
    }
}