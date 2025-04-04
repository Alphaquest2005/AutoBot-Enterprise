using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitDiscrepanciesToCustoms are here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _databaseCommandTimeout = 30;

        private static void SubmitDiscrepanciesToCustoms(IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst)
        {
            try
            {
                Console.WriteLine("Submit Discrepancies To Customs");

                // var saleInfo = CurrentSalesInfo(); // Assuming CurrentSalesInfo exists if needed

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = _databaseCommandTimeout;
                    // This calls GetContacts, which needs to be in its own partial class
                    var contacts = GetContacts(new List<string>(){ "Customs" });

                    foreach (var data in lst)
                    {
                        // This calls CreateDiscrepancyEmail, which needs to be in its own partial class
                        CreateDiscrepancyEmail(data, contacts);
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}