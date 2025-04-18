using System;
using System.IO;
using System.Linq;
using Core.Common.Extensions; // Assuming StringExtensions is here
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using ValuationDS.Business.Entities; // Assuming TODO_C71ToCreate is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class C71Utils
    {
        public static void DownLoadC71(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Attempting Download C71 Files");
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;

                    var lst = ctx.TODO_C71ToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .Take(1)
                        .ToList();

                    if (!lst.Any()) return;
                    var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), "C71")); // Assuming GetDocSetDirectoryName exists

                    Console.WriteLine("Download C71 Files");
                    var notries = 2;
                    var tries = 0;
                    var lcont = 0;
                    // This calls ImportC71Complete, which needs to be in its own partial class
                    while (ImportC71Complete(directoryName, out lcont) == false)
                    {
                        SikuliAutomationService.RunSiKuLi(directoryName, "C71", lcont.ToString()); // Assuming RunSiKuLi exists
                        tries += 1;
                        if (tries >= notries) break;
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