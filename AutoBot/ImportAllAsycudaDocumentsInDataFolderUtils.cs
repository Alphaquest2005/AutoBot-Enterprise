using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class ImportAllAsycudaDocumentsInDataFolderUtils
    {
        public static void ImportAllAsycudaDocumentsInDataFolder(bool overwriteExisting)
        {
            try
            {
                Console.WriteLine("Import All Asycuda Documents in DataFolder");

                BaseDataModel.Instance.ImportDocuments(GetAsycudaDocumentSetEx("Imports").AsycudaDocumentSetId, GetImportFileList(), true, true, true, overwriteExisting, true)
                    .Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<string> GetImportFileList()
        {
            var iFiles = IFiles();

            return GetFiles()
                .Select(file => ( file, Match: GetFileMatch(file)))
                .Where(t => t.Match.Success)
                .Select(t => ( t, i : IsFileMatch(iFiles, t) ))
                .Where(t => !t.i.Equals(default))
                .Select(t => t.t.file).ToList();
        }

        private static (string Office, string CNumber, string Year) IsFileMatch(IEnumerable<(string Office, string CNumber, string Year)> iFiles, (string file, Match Match) t) =>
            iFiles.FirstOrDefault(x =>
                x.CNumber == t.Match.Groups["CNumber"].Value && x.Office == t.Match.Groups["Office"].Value &&
                (string.IsNullOrEmpty(t.Match.Groups["Year"].ToString()) ||
                 x.Year == t.Match.Groups["Year"].ToString()));

        private static Match GetFileMatch(string file) =>
            Regex.Match(file, @"(?<Office>[A-Z]+)(\-(?<Year>\d{4}))?\-(?<CNumber>\d+).xml",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        private static List<(string Office, string CNumber, string Year)> IFiles() =>
            new CoreEntitiesContext().AsycudaDocuments
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.ImportComplete == true).Select(x => new
                {
                    Office = x.Customs_clearance_office_code,
                    x.CNumber,
                    Year = (x.RegistrationDate ?? DateTime.MinValue).Year.ToString()
                })
                .ToList()
                .Select(x => (x.Office,x.CNumber,x.Year))
                .ToList();

        private static AsycudaDocumentSetEx GetAsycudaDocumentSetEx(string DocSetReference) =>
            new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                x.Declarant_Reference_Number == DocSetReference);

        private static string[] GetFiles() =>
            Directory.GetFiles(
                Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports"), "*.xml");
    }
}
