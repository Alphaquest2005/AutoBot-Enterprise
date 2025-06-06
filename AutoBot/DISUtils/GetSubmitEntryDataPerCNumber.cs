using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitAllXMLToCustoms are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _databaseCommandTimeout = 30;

        private static List<TODO_SubmitAllXMLToCustoms> GetSubmitEntryDataPerCNumber(List<string> cplst, List<string> cnumberList)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                List<TODO_SubmitAllXMLToCustoms> res;
                res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId && cplst.Any(z => z == x.CustomsProcedure))
                    .ToList()
                    .Where(x => cnumberList.Contains(x.CNumber))
                    .ToList();
                return res;
            }
        }
    }
}