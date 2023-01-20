using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class EntryDocSetUtils
    {
        public static (AsycudaDocumentSet docSet, List<IGrouping<string, xcuda_ASYCUDA>> lst)
            GetDuplicateDocuments(int docKey)
        {
            try
            {

                var docSet = GetAsycudaDocumentSet(docKey);

                var res = GetRelatedDocuments(docSet);

                var lst = FilterForDuplicateDocuments(res, docSet);

                return (docSet, lst);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static AsycudaDocumentSet GetAsycudaDocumentSet(int docKey)
        {
            using (var ctx = new DocumentDSContext())
            {
                return ctx.AsycudaDocumentSets.Include("xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA")
                    .First(x => x.AsycudaDocumentSetId == docKey);
            }
        }

        private static List<IGrouping<string, xcuda_ASYCUDA>> FilterForDuplicateDocuments(
            List<IGrouping<string, xcuda_ASYCUDA>> res, AsycudaDocumentSet docSet)
        {
            return res
                .Where(x => x.Key != null && x.Count() > 1 &&
                            x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false)
                            || x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.FileNumber == docSet.LastFileNumber))
                .ToList();
        }

        public static List<IGrouping<string, xcuda_ASYCUDA>> GetRelatedDocuments(AsycudaDocumentSet docSet)
        {

            using (var ctx = new DocumentDSContext())
            {
                var res = ctx.xcuda_ASYCUDA
                    .Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                    .Include(x => x.xcuda_Declarant)
                    .Where(
                        x => x != null && x.xcuda_Declarant != null &&
                             x.xcuda_Declarant.Number.Contains(docSet.Declarant_Reference_Number) &&
                             ((x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                               x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false) ||
                              (x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId != docSet.AsycudaDocumentSetId &&
                               x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete))
                    )
                    .ToList() // must load first to get the navigation properties
                    .GroupBy(x => x.xcuda_Declarant.Number) //
                    .ToList();

                return res;
            }
        }

        public static void RenameDuplicateDocuments(List<IGrouping<string, xcuda_ASYCUDA>> lst,
            ref AsycudaDocumentSet docSet)
        {
            try
            {
                var docSetAsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                using (var ctx = new DocumentDSContext { StartTracking = true })
                {
                    foreach (var g in lst)
                    foreach (var doc in g)
                    {
                        AppendDocSetLastFileNumber(docSet);
                        var prop = GetXcudaAsycudaExtendedProperties(doc, docSetAsycudaDocumentSetId);
                        var declarant = ctx.xcuda_Declarant.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        var oldRef = declarant.Number;
                        //var letter = oldRef.Substring(oldRef.IndexOf(prop.FileNumber.ToString()) - 1, 1);
                        var newRef = declarant.Number?.Replace(prop.FileNumber.ToString(),
                            docSet.LastFileNumber.ToString());
                        declarant.Number = newRef;
                        declarant.TrackingState = TrackingState.Modified;
                        prop.FileNumber = docSet.LastFileNumber;
                        ctx.SaveChanges();
                        ctx.ApplyChanges(docSet);
                        ctx.SaveChanges();

                        UpdateNameDependentAttachments(prop.ASYCUDA_Id, oldRef, newRef);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static xcuda_ASYCUDA_ExtendedProperties GetXcudaAsycudaExtendedProperties(xcuda_ASYCUDA doc,
            int docSetAsycudaDocumentSetId)
        {
            try
            {
                return new DocumentDSContext { StartTracking = true }.xcuda_ASYCUDA_ExtendedProperties.First(x =>
                    x.ASYCUDA_Id == doc.ASYCUDA_Id); //&& x.AsycudaDocumentSetId == docSetAsycudaDocumentSetId
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void AppendDocSetLastFileNumber(AsycudaDocumentSet docSet)
        {
            docSet.LastFileNumber += 1;
            docSet.TrackingState = TrackingState.Modified;
        }

        private static void UpdateNameDependentAttachments(int asycudaId, string oldRef, string newRef)
        {
            using (var ctx = new DocumentItemDSContext { StartTracking = true })
            {
                var itms = GetAttachementItems(asycudaId, oldRef, ctx);

                foreach (var itm in itms)
                {
                    UpdateAttachments(oldRef, newRef, itm);
                }

                ctx.SaveChanges();
            }
        }

        private static void UpdateAttachments(string oldRef, string newRef, xcuda_Attached_documents itm)
        {
            itm.Attached_document_reference = newRef;
            foreach (var att in itm.xcuda_Attachments.Where(x => x.Attachments.Reference == oldRef)
                         .ToList())
            {
                att.Attachments.Reference = newRef;
                att.Attachments.FilePath = att.Attachments.FilePath.Replace(oldRef, newRef);
            }
        }

        private static List<xcuda_Attached_documents> GetAttachementItems(int asycudaId, string oldRef,
            DocumentItemDSContext ctx)
        {
            return ctx.xcuda_Attached_documents
                .Include("xcuda_Attachments.Attachments")
                .Where(x => x.Attached_document_reference == oldRef)
                .Where(x => x.xcuda_Item.ASYCUDA_Id == asycudaId)
                .ToList();
        }

        public static AsycudaDocumentSet GetLatestDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefault();
                return docSet;
            }
        }

        public static AsycudaDocumentSet GetDocSet(string Reference)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Declarant_Reference_Number.Contains(Reference))
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefault();
                return docSet;
            }
        }

        public static List<AsycudaDocumentSet> GetLatestModifiedDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Any(z => z.ImportComplete == false))
                    .OrderByDescending(x => x.xcuda_ASYCUDA_ExtendedProperties.Max(z => z.ASYCUDA_Id))
                    .ToList();
                return docSet;
            }
        }

        public static (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) CreateMonthYearAsycudaDocSet(
            DateTime startDate)
        {
            var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
            var docRef = startDate.ToString("MMMM") + " " + startDate.Year;
            var docSet = GetAsycudaDocumentSet(docRef, false);

            var dirPath =
                StringExtensions.UpdateToCurrentUser(
                    BaseDataModel.GetDocSetDirectoryName(docRef));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return (StartDate: startDate, EndDate: endDate, DocSet: docSet, DirPath: dirPath);
        }

        public static AsycudaDocumentSet GetAsycudaDocumentSet(string docRef, bool isSystemDocSet)
        {
            AsycudaDocumentSet docSet;
            docSet = new DocumentDSContext().AsycudaDocumentSets.FirstOrDefault(x =>
                x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
            if (docSet == null) docSet = CreateAsycudaDocumentSet(docRef, isSystemDocSet);
            return docSet;
        }

        public static AsycudaDocumentSet CreateAsycudaDocumentSet(string docRef, bool isSystemDocSet)
        {
            try
            {


                AsycudaDocumentSet docSet;
                using (var ctx = new DocumentDSContext())
                {
                    var doctype = BaseDataModel.Instance.Customs_Procedures
                        .First(x => x.CustomsOperationId == (int)CustomsOperations.Warehouse
                                    && x.IsDefault == true); //&& x.Discrepancy != true
                    ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{docRef}',{doctype.Customs_ProcedureId},0)");

                    docSet = ctx.AsycudaDocumentSets.FirstOrDefault(x =>
                        x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);

                    if (!isSystemDocSet) return docSet;
                    ctx.SystemDocumentSets.Add(new SystemDocumentSet(true)
                        { AsycudaDocumentSet = docSet, TrackingState = TrackingState.Added });
                    ctx.SaveChanges();
                }

                return docSet;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}