using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSetExs, FileTypes, FileImporterInfos are here
using LicenseDS.Business.Entities; // Assuming LicenseDSContext, xLIC_License, Registered are here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static bool ImportAllLicense()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;

                var docSet = ctx.AsycudaDocumentSetExs.First(x => // InvalidOperationException if no match
                                    x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                    x.Declarant_Reference_Number == "Imports");

                var ft = ctx.FileTypes
                    .Include(x => x.FileImporterInfos)
                    .FirstOrDefault(x =>
                    x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return true;

                var desFolder = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), "LIC"); // Assuming GetDocSetDirectoryName exists
                if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);

                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                    .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                    .ToList();
                if (!csvFiles.Any()) return false;

                var ifiles = new LicenseDSContext().xLIC_License.OfType<Registered>()
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Select(x =>  x.RegistrationNumber).ToList();

                foreach (var file in csvFiles.ToList())
                {
                    var match = Regex.Match(file.FullName, ft.FilePattern, RegexOptions.IgnoreCase).Groups["RegNumber"].Value;
                    if (!ifiles.Contains(match)) continue;
                    csvFiles.Remove(file);
                }

                BaseDataModel.Instance.ImportLicense(docSet.AsycudaDocumentSetId, // Assuming ImportLicense exists
                    csvFiles.Select(x => x.FullName).ToList());

                BaseDataModel.Instance.SaveAttachedDocuments(csvFiles.ToArray(), ft).Wait(); // Assuming SaveAttachedDocuments exists
                return false;
            }
        }
    }
}