using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using LicenseDS.Business.Entities; // Assuming TODO_LICToAssess is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here
using AutoBot.SQLBlackBox; // Assuming SQLBlackBox is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void AssessLicense(FileTypes ft)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox(); // Assuming RunSqlBlackBox exists

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var res = ctx.TODO_LICToAssess
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();

                    foreach (var doc in res)
                    {
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number);
                        var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number, "LIC-Instructions.txt");
                        if (!File.Exists(instrFile)) continue;
                        var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                        var lcont = 0;
                        // This calls AssessLICComplete, which needs to be in its own partial class
                        while (LICUtils.AssessLICComplete(instrFile, resultsFile, out lcont) == false)
                        {
                            SikuliAutomationService.RunSiKuLi(directoryName, "AssessLIC", lcont.ToString()); // Assuming RunSiKuLi exists
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