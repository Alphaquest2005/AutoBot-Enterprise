using System;
using System.IO;
using System.Linq;
using Core.Common.Extensions; // Assuming StringExtensions is here
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext are here
using LicenseDS.Business.Entities; // Assuming TODO_LICToCreate is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void DownLoadLicence(bool redownload, FileTypes ft)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_LICToCreate
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();

                    if (!pOs.Any()) return;
                    var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports", "LIC")); ;
                    Console.WriteLine("Download License Files");
                    var lcont = 0;
                    if (redownload)
                    {
                        SikuliAutomationService.RunSiKuLi(directoryName, "LIC", lcont.ToString()); // Assuming RunSiKuLi exists
                    }
                    else
                    {
                        // This calls ImportLICComplete, which needs to be in its own partial class
                        while (ImportLICComplete(directoryName, out lcont) == false)
                        {
                            SikuliAutomationService.RunSiKuLi(directoryName, "LIC", lcont.ToString()); // Assuming RunSiKuLi exists
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