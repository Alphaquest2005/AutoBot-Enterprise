using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using ValuationDS.Business.Entities; // Assuming TODO_C71ToCreate is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class C71Utils
    {
        public static void AssessC71(FileTypes ft)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var res = ctx.TODO_C71ToCreate
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .Take(1)
                    .ToList();

                foreach (var doc in res)
                {
                    var directoryName = BaseDataModel.GetDocSetDirectoryName(doc.Declarant_Reference_Number); // Assuming GetDocSetDirectoryName exists
                    var instrFile = Path.Combine(directoryName, "C71-Instructions.txt");
                    if (!File.Exists(instrFile)) continue;
                    var resultsFile = Path.Combine(directoryName, "C71-InstructionResults.txt");
                    var lcont = 0;
                    // This calls AssessC71Complete, which needs to be in its own partial class
                    while (C71Utils.AssessC71Complete(instrFile, resultsFile, out lcont) == false)
                    {
                        SikuliAutomationService.RunSiKuLi(directoryName, "AssessC71", lcont.ToString()); // Assuming RunSiKuLi exists
                    }
                }
            }
        }
    }
}