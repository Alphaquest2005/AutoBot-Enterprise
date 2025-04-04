using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Asycuda421; // Assuming Licence class is here
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSet_Attachments, FileTypes, FileImporterInfos, AsycudaDocumentSetEntryDataEx are here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, EntryData, PurchaseOrders are here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static bool ImportLicense(string declarant_Reference_Number,int asycudaDocumentSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;

                var reference = declarant_Reference_Number;
                var directory = BaseDataModel.GetDocSetDirectoryName(reference); // Assuming GetDocSetDirectoryName exists
                if (!Directory.Exists(directory)) return false;

                var lastdbfile =
                    ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic);
                var lastfiledate = lastdbfile  != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1); // Potential NullReferenceException

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
                    .Where(x => x.LastWriteTime >= lastfiledate)
                    .ToList();
                if (!csvFiles.Any()) return false;

                foreach (var file in csvFiles.ToList())
                {
                    var a = Licence.LoadFromFile(file.FullName); // Assuming Licence.LoadFromFile exists

                    if (a.General_segment.Exporter_address.Text.Any(x => x.Contains(file.Name.Replace("-LIC.xml", "")))) // Potential NullReferenceExceptions
                        csvFiles.Remove(file);//the po should be different from the reference number
                    if(!ctx.AsycudaDocumentSetEntryDataEx.Any( x => x.AsycudaDocumentSetId == asycudaDocumentSetId
                                                                    && a.General_segment.Exporter_address.Text.Any(z => z.Contains(x.EntryDataId))) // Potential NullReferenceExceptions
                       && !new EntryDataDSContext().EntryData.OfType<PurchaseOrders>().Any(x => x.EntryDataEx.AsycudaDocumentSetId == asycudaDocumentSetId // Potential NullReferenceException
                           && a.General_segment.Exporter_address.Text.Any(z => z.Contains(x.SupplierInvoiceNo)))) // Potential NullReferenceExceptions

                        csvFiles.Remove(file);
                }

                BaseDataModel.Instance.ImportLicense(asycudaDocumentSetId, // Assuming ImportLicense exists
                    csvFiles.Select(x => x.FullName).ToList());
                ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                BaseDataModel.Instance.SaveAttachedDocuments(csvFiles.ToArray(), ft).Wait(); // Assuming SaveAttachedDocuments exists
                return false;
            }
        }
    }
}