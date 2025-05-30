using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitAllXMLToCustoms are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _tripleLongDatabaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _tripleLongDatabaseCommandTimeout = _databaseCommandTimeout * 3;

        private static List<TODO_SubmitAllXMLToCustoms> GetSubmitEntryDataPerDocSet(int asycudaDocumentSetId, List<string> cplst)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _tripleLongDatabaseCommandTimeout;
                List<TODO_SubmitAllXMLToCustoms> res;
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).Result; // Assuming GetAsycudaDocumentSet exists
                res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId && cplst.Any(z => z == x.CustomsProcedure))
                    .ToList()
                    .Where(x => x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number)) // Potential NullReferenceException if docSet is null
                    .ToList();
                return res;
            }
        }
    }
}