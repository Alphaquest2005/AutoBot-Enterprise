using System;
using System.Data;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using AutoBotUtilities.CSV; // Assuming CSVUtils is here
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, AsycudaDocuments, AsycudaDocumentSetEx are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void AssessDiscrepancyExecutions(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext() {StartTracking = true})
                {
                    ctx.Database.CommandTimeout = 10;
                    foreach (var file in fs)
                    {
                        var dt = CSVUtils.CSV2DataTable(file, "YES");
                        if (dt.Rows.Count == 0) continue;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (string.IsNullOrEmpty(row["Reference"].ToString())) continue;
                            var reference = row["Reference"].ToString();
                            var doc = ctx.AsycudaDocuments.Include(x => x.AsycudaDocumentSetEx).FirstOrDefault(x =>
                                    x.ReferenceNumber == reference && x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                ?.AsycudaDocumentSetEx;
                            // Need to ensure EntryDocSetUtils.AssessEntries is accessible
                            if(doc != null) EntryDocSetUtils.AssessEntries(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                        }

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