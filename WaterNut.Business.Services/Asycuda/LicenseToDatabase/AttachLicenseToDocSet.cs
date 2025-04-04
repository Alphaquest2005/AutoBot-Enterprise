using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private static void AttachLicenseToDocSet(AsycudaDocumentSet docSet, FileInfo file, string regNumber)
        {
            using (var cctx = new CoreEntitiesContext())
            {
                var fileType = cctx.FileTypes.First(x => // InvalidOperationException if no match
                    x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                    x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic); // Assuming FileTypeManager exists
                var res = cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId
                    && x.Attachments.FilePath == file.FullName);
                if (res == null)
                {
                     res = new AsycudaDocumentSet_Attachments(true)
                    {
                        AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                        DocumentSpecific = true,
                        FileDate = file.LastWriteTime,
                        EmailId = null,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,
                        Attachments = new CoreEntities.Business.Entities.Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = fileType.DocumentCode,
                            Reference = regNumber,
                            TrackingState = TrackingState.Added,
                        }
                    };
                    cctx.AsycudaDocumentSet_Attachments.Add(res);
                }
                else
                {
                    res.Attachments.Reference = regNumber; // Potential NullReferenceException
                }

                cctx.SaveChanges();
            }
        }
    }
}