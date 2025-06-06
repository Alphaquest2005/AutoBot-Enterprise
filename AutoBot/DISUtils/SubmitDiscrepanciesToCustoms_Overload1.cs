using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_SubmitDiscrepanciesToCustoms are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere in a partial class or needs to be moved
        // private static readonly int _databaseCommandTimeout = 30;

        public static void SubmitDiscrepanciesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = _databaseCommandTimeout;

                    IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                    lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x => x.EmailId == ft.EmailId
                                                                           && x.ApplicationSettingsId ==
                                                                           BaseDataModel.Instance
                                                                               .CurrentApplicationSettings
                                                                               .ApplicationSettingsId)

                        .ToList()

                        .GroupBy(x => x.EmailId);
                    // This calls the private SubmitDiscrepanciesToCustoms, which needs to be in its own partial class
                    SubmitDiscrepanciesToCustoms(lst);

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