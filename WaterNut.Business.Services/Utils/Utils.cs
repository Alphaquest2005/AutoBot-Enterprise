using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using MoreLinq.Extensions;

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
            HashSet<AsycudaDocumentSet> docSet;
            var asycudaDocumentSet = EntryDocSetUtils.GetAsycudaDocumentSet(fileType.DocSetRefernece, true);

            docSet = new HashSet<AsycudaDocumentSet>();
            
            var originaldocSetRefernece = GetFileTypeOriginalReference(fileType);
            if (fileType.CopyEntryData)
            {
                docSet.Add(asycudaDocumentSet);
                GetAsycudaDocumentSets(originaldocSetRefernece).ForEach(x => docSet.Add(x));
            }
            else
            {
                if (IsSystemDocSet(asycudaDocumentSet))
                {
                    docSet.Clear();
                    GetAsycudaDocumentSets(originaldocSetRefernece).ForEach(x => docSet.Add(x));

                }

            }

            if (!docSet.Any()) throw new ApplicationException("Document Set with reference not found");


            return docSet.DistinctBy(x => x.AsycudaDocumentSetId).ToList();
        }

        private static bool IsSystemDocSet(AsycudaDocumentSet asycudaDocumentSet) =>
            new DocumentDSContext().SystemDocumentSets.FirstOrDefault(x => x.Id == asycudaDocumentSet.AsycudaDocumentSetId) != null;

        private static string GetFileTypeOriginalReference(FileTypes fileType)
        {
            using (var ctx = new DocumentDSContext())
            {
                return ctx.FileTypes.First(x => x.Id == fileType.Id).DocSetRefernece;
            }
        }

        private static List<AsycudaDocumentSet> GetAsycudaDocumentSets(string docSetRefernece)
        {
            using (var ctx = new DocumentDSContext())
            {
                return ctx.AsycudaDocumentSets
                    .Include(x => x.SystemDocumentSet)
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Declarant_Reference_Number == docSetRefernece).ToList();
            }
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