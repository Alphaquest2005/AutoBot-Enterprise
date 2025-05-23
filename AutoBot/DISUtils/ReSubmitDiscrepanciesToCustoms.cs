using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, TODO_SubmitDiscrepanciesToCustoms are here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void ReSubmitDiscrepanciesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                var emailId = ft.EmailId;
                // This calls GetSubmitEntryData, which needs to be in its own partial class
                var lst = GetSubmitEntryData(ft);
                var toBeProcessed =
                    Enumerable.Where<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>>(lst, x => (emailId.Contains("Submit") || x.Key == emailId) || ft.Data.Any(z => x.Any(q => q.CNumber == z.Value )));
                // This calls the private SubmitDiscrepanciesToCustoms, which needs to be in its own partial class
                SubmitDiscrepanciesToCustoms(toBeProcessed);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}