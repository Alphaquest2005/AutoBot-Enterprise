using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, Contacts, AsycudaDocumentSetExs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void EmailPOEntries(int asycudaDocumentSetId)
        {
            try
            {
                var contacts = new CoreEntitiesContext().Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "PO Clerk")
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();

                if (!contacts.Any() || BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true) return;
                //if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == asycudaDocumentSetId); //CurrentPOInfo();
                if (docSet == null) return;
                //foreach (var poInfo in lst.Where(x => x.Item1.AsycudaDocumentSetId == asycudaDocumentSetId))
                //{

                // if (poInfo.Item1 == null) return;

                var reference = docSet.Declarant_Reference_Number;
                var directory = BaseDataModel.GetDocSetDirectoryName(reference); // Assuming GetDocSetDirectoryName exists
                if (!Directory.Exists(directory)) return;
                var sourcefiles = Directory.GetFiles(directory, "*.xml");

                var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                if (!instructions.Exists) return;
                if (emailres.Exists) File.Delete(emailres.FullName);
                Console.WriteLine("Emailing Po Entries");
                string[] files;
                if (File.Exists(emailres.FullName))
                {
                    var eRes = File.ReadAllLines(emailres.FullName);
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.Where(x => insts.Any(z => z.Contains(x)) && !eRes.Any(z => z.Contains(x)))
                        .ToArray();
                }
                else
                {
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.ToList().Where(x => insts.Any(z => z.Contains(x))).ToArray();
                }

                if (files.Length > 0)
                    // Assuming Utils.Client exists
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Entries for {reference}",
                        contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files);

                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}