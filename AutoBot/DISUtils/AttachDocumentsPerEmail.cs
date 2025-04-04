using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitDiscrepanciesToCustoms, AsycudaDocuments, AsycudaDocumentSet_Attachments, Attachments, AttachmentLog, AsycudaDocumentSetExs are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _databaseCommandTimeout = 30;

        private static void AttachDocumentsPerEmail(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                foreach (var item in emailIds)
                {
                    var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                        x.ASYCUDA_Id == item.ASYCUDA_Id &&
                        x.ApplicationSettingsId == item.ApplicationSettingsId);
                    var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                        x.Attachments.FilePath == sfile.SourceFileName); // Potential NullReferenceException if sfile is null
                    if (eAtt != null)
                    {
                        ctx.AttachmentLog.Add(new AttachmentLog(true)
                        {
                            DocSetAttachment = eAtt.Id,
                            Status = "Submit XML To Customs",
                            TrackingState = TrackingState.Added
                        });
                    }
                    else
                    {
                        var attachment =
                            ctx.Attachments.First(x => x.FilePath == sfile.SourceFileName); // Potential NullReferenceException if sfile is null, InvalidOperationException if no match
                        ctx.AsycudaDocumentSet_Attachments.Add(new AsycudaDocumentSet_Attachments()
                        {
                            TrackingState = TrackingState.Added,
                            AsycudaDocumentSetId = ctx.AsycudaDocumentSetExs.First(x => // InvalidOperationException if no match
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId && x.Declarant_Reference_Number == "Imports").AsycudaDocumentSetId,
                            AttachmentId = attachment.Id, // Potential NullReferenceException if attachment is null
                            DocumentSpecific = true,
                            FileDate = DateTime.Now,
                            EmailId = attachment.EmailId, // Potential NullReferenceException if attachment is null
                        });
                    }
                }

                ctx.SaveChanges();
            }
        }
    }
}