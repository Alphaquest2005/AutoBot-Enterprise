using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;

namespace WaterNut.DataSpace
{
    public class EntryDocSetUtils
    {
        public static (AsycudaDocumentSet docSet, List<IGrouping<string, xcuda_ASYCUDA>> lst) GetDuplicateDocuments(int docKey)
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
                return ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docKey);
            }
        }

        private static List<IGrouping<string, xcuda_ASYCUDA>> FilterForDuplicateDocuments(List<IGrouping<string, xcuda_ASYCUDA>> res, AsycudaDocumentSet docSet)
        {
            return res
                .Where(x => x.Key != null && x.Count() > 1
                            || x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.FileNumber == docSet.LastFileNumber))
                .ToList();
        }

        private static List<IGrouping<string, xcuda_ASYCUDA>> GetRelatedDocuments(AsycudaDocumentSet docSet)
        {

            using (var ctx = new DocumentDSContext())
            {
                var res = ctx.xcuda_ASYCUDA
                    .Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                    .Where(
                        x => x != null && x.xcuda_Declarant != null &&
                             x.xcuda_Declarant.Number.Contains(docSet.Declarant_Reference_Number) &&
                             (x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                              x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false ||
                              x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId != docSet.AsycudaDocumentSetId &&
                              x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete))
                    .GroupBy(x => x.xcuda_Declarant.Number)
                    .ToList();

                // // because the include not loading the properties

                foreach (var asycuda in res.SelectMany(itm => itm))
                {
                    asycuda.xcuda_ASYCUDA_ExtendedProperties =
                        ctx.xcuda_ASYCUDA_ExtendedProperties.First(x => x.ASYCUDA_Id == asycuda.ASYCUDA_Id);
                }

                return res;
            }
        }

        public static void RenameDuplicateDocuments(List<IGrouping<string, xcuda_ASYCUDA>> lst,
            ref AsycudaDocumentSet docSet)
        {
            try
            {
                var docSetAsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                using (var ctx = new DocumentDSContext {StartTracking = true})
                {
                    foreach (var g in lst)
                    foreach (var doc in g)
                    {
                        AppendDocSetLastFileNumber(docSet);
                        var prop = GetXcudaAsycudaExtendedProperties(ctx, doc, docSetAsycudaDocumentSetId);
                        var declarant = ctx.xcuda_Declarant.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        var oldRef = declarant.Number;
                        var letter = oldRef.Substring(oldRef.IndexOf(prop.FileNumber.ToString()) - 1, 1);
                        var newRef = declarant.Number?.Replace(prop.FileNumber.ToString(), docSet.LastFileNumber.ToString());
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

        private static xcuda_ASYCUDA_ExtendedProperties GetXcudaAsycudaExtendedProperties(DocumentDSContext ctx, xcuda_ASYCUDA doc, int docSetAsycudaDocumentSetId)
        {
            return ctx.xcuda_ASYCUDA_ExtendedProperties.First(x =>
                x.ASYCUDA_Id == doc.ASYCUDA_Id && x.AsycudaDocumentSetId == docSetAsycudaDocumentSetId);
        }

        private static void AppendDocSetLastFileNumber(AsycudaDocumentSet docSet)
        {
            docSet.LastFileNumber += 1;
            docSet.TrackingState = TrackingState.Modified;
        }

        private static void UpdateNameDependentAttachments(int asycudaId, string oldRef, string newRef)
        {
            using (var ctx = new DocumentItemDSContext {StartTracking = true})
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

        private static List<xcuda_Attached_documents> GetAttachementItems(int asycudaId, string oldRef, DocumentItemDSContext ctx)
        {
            return ctx.xcuda_Attached_documents
                .Include("xcuda_Attachments.Attachments")
                .Where(x => x.Attached_document_reference == oldRef)
                .Where(x => x.xcuda_Item.ASYCUDA_Id == asycudaId)
                .ToList();
        }

        public static AsycudaDocumentSetEx GetLatestDocSet()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var docSet = ctx.AsycudaDocumentSetExs.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefault();
                return docSet;
            }
        }
    }
}