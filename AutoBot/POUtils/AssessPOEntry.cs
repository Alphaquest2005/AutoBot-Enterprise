using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSetToExport are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void AssessPOEntry(string docReference, int asycudaDocumentSetId)
        {
            try
            {
                if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                        x.AsycudaDocumentSetId != asycudaDocumentSetId))
                {
                    return;
                }

                if (docReference == null) return;
                var directoryName = BaseDataModel.GetDocSetDirectoryName(docReference); // Assuming GetDocSetDirectoryName exists
                var resultsFile = Path.Combine(directoryName, "InstructionResults.txt");
                var instrFile = Path.Combine(directoryName, "Instructions.txt");

                var lcont = 0;
                while (SikuliAutomationService.AssessComplete(instrFile, resultsFile, out lcont) == false) // Assuming AssessComplete exists
                {
                    // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                    SikuliAutomationService.RunSiKuLi(directoryName, "SaveIM7", lcont.ToString()); // Assuming RunSiKuLi exists
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}