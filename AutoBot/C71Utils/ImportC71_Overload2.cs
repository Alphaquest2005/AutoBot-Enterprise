using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSet_Attachments, FileTypes, FileImporterInfos are here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class C71Utils
    {
        public static bool ImportC71(string declarant_Reference_Number, int asycudaDocumentSetId)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    //var reference = declarant_Reference_Number;
                    //var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    //    reference);
                    //if (!Directory.Exists(directory)) return false;

                    var lastdbfile =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71);
                    var lastfiledate = lastdbfile != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1); // Potential NullReferenceException if Attachments is null


                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71 && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return true;
                    var desFolder = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), FileTypeManager.EntryTypes.C71); // Assuming GetDocSetDirectoryName exists

                    if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);

                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => x.LastWriteTime >= lastfiledate)
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase) && x.Name != "C71.xml")
                        .ToList();
                    //if (lastC71 == null) return false;
                    //var csvFiles = new FileInfo[] {lastC71};

                    if (csvFiles.Any())
                    {
                        BaseDataModel.Instance.ImportC71(asycudaDocumentSetId, // Assuming ImportC71 exists
                            csvFiles.Select(x => x.FullName).ToList());
                        ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                        //BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, ft).Wait(); // Assuming SaveAttachedDocuments exists
                    }

                    return false;
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