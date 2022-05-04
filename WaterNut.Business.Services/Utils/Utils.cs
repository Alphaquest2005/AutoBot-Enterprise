using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;

namespace WaterNut.DataSpace
{
    public class Utils
    {
        public static Client GetClient()
        {
            return new EmailDownloader.Client
            {
                CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
                DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
                Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
                EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList()
            };
        }


        public static List<AsycudaDocumentSet> GetDocSets(FileTypes fileType)
        {
            List<AsycudaDocumentSet> docSet;
            using (var ctx = new DocumentDSContext())
            {
                docSet = new List<AsycudaDocumentSet>()
                {
                    ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                };
                var ddocset = ctx.FileTypes.First(x => x.Id == fileType.Id).AsycudaDocumentSetId;
                if (fileType.CopyEntryData)
                    docSet.Add(ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == ddocset));
                else
                {
                    if (ctx.SystemDocumentSets.FirstOrDefault(x => x.Id == ddocset) != null)
                    {
                        docSet.Clear();
                        docSet.Add(ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == ddocset));

                    }

                }
                if (!docSet.Any()) throw new ApplicationException("Document Set with reference not found");
            }

            return docSet;
        }

        public static string GetExistingEmailId(string droppedFilePath, FileTypes fileType)
        {
            string emailId = null;

            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.AsycudaDocumentSet_Attachments.Where(x =>
                        x.Attachments.FilePath.Replace(".xlsx", "-Fixed.csv") == droppedFilePath
                        || x.Attachments.FilePath == droppedFilePath)
                    .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                emailId = res?.EmailId ?? (fileType.EmailId == "0" || fileType.EmailId == null ? null : fileType.EmailId);
            }

            return emailId;
        }
    }
}