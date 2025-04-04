using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using TrackableEntities; // Assuming TrackingState is here
using ValuationDS.Business.Entities; // Assuming Registered is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private static void AttachC71ToDocset(AsycudaDocumentSet docSet, FileInfo file, Registered ndoc = null)
        {
            using (var cctx = new CoreEntitiesContext())
            {
                try
                {
                    var fileType = cctx.FileTypes
                        .Include(x => x.FileImporterInfos)
                        .First(x => // InvalidOperationException if no match
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                        x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71); // Assuming FileTypeManager exists

                    var elst = ndoc?.xC71_Item.Select(x => x.Invoice_Number).ToList(); // Potential NullReferenceException

                    if (ndoc != null && ndoc.xC71_Item.Any() && !cctx.AsycudaDocumentSetEntryDataEx.Any(z => // Potential NullReferenceException
                            z.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                            elst.Any(x => x == z.EntryDataId))) return;

                    var attachments =
                        cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).Where(x =>
                            x.Attachments.FilePath == file.FullName &&
                            x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId).ToList();
                    if (attachments.Any())
                    {
                        cctx.Attachments.RemoveRange(attachments.Select(x => x.Attachments));
                        cctx.AsycudaDocumentSet_Attachments.RemoveRange(attachments);
                        cctx.SaveChanges();
                    }

                    var c71s = cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments)
                        .Where(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                                    x.FileTypeId == fileType.Id && x.Attachments.Reference != "C71")
                        .ToList(); //OrderByDescending(x => x.AttachmentId).Skip(1).ToList();

                    var rc71s = cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments)
                        .Where(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                                    x.FileTypeId == fileType.Id && x.Attachments.Reference == "C71" &&
                                    !x.Attachments.FilePath.Contains("\\C71.xml")).ToList();

                    cctx.Attachments.RemoveRange(c71s.Select(x => x.Attachments));
                    cctx.AsycudaDocumentSet_Attachments.RemoveRange(c71s);
                    cctx.Attachments.RemoveRange(rc71s.Select(x => x.Attachments));
                    cctx.AsycudaDocumentSet_Attachments.RemoveRange(rc71s);
                    cctx.SaveChanges();

                    var res = new AsycudaDocumentSet_Attachments(true)
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
                            Reference = ndoc?.RegNumber ?? "C71", // Potential NullReferenceException
                            TrackingState = TrackingState.Added,
                        }
                    };
                    cctx.AsycudaDocumentSet_Attachments.Add(res);
                    cctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}