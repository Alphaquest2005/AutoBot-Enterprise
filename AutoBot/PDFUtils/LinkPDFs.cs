using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext and TODO_ImportCompleteEntries are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class PDFUtils
    {
        public static void LinkPDFs()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                            $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}")
                        .Select(x => x.AssessedAsycuda_Id)
                        .Distinct()
                        .ToList();

                    //var entries = ctx.TODO_ImportCompleteEntries
                    //    .Where(x => x.ApplicationSettingsId ==
                    //                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    //    .Select(x => x.AssessedAsycuda_Id)
                    //    .Distinct()
                    //    .ToList();


                    BaseDataModel.LinkPDFs(entries);
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