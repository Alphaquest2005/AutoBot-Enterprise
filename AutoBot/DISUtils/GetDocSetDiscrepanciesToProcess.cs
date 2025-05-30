using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_TotalAdjustmentsToProcess are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _tripleLongDatabaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _tripleLongDatabaseCommandTimeout = _databaseCommandTimeout * 3;

        private static List<TODO_TotalAdjustmentsToProcess> GetDocSetDiscrepanciesToProcess(int docSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _tripleLongDatabaseCommandTimeout;
                var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId && x.AsycudaDocumentSetId == docSetId)
                    .ToList();
                return totaladjustments;
            }
        }
    }
}