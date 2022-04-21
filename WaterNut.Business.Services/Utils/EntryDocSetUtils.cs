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
        public static (AsycudaDocumentSet docSet, List<IGrouping<string, xcuda_ASYCUDA>> lst)
            GetDuplicateDocuments(int docKey)
        {
            try
            {

                List<IGrouping<string, xcuda_ASYCUDA>> lst;
                AsycudaDocumentSet docSet;
                using (var ctx = new DocumentDSContext())
                {
                    docSet = ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docKey);
                    //docSet.xcuda_ASYCUDA_ExtendedProperties =
                    //    null; //loading property and creating trouble updating it think its a circular navigation property issue

                    var res = ctx.xcuda_ASYCUDA
                        .Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                        .Where(
                            x => x != null && x.xcuda_Declarant != null &&
                                 x.xcuda_Declarant.Number.Contains(docSet.Declarant_Reference_Number) &&
                                 (x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == docKey &&
                                  x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false ||
                                  x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId != docKey &&
                                  x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete))
                        .GroupBy(x => x.xcuda_Declarant.Number)
                        
                        .ToList();

                    // // because the include not loading the properties

                    foreach (var asycuda in res.SelectMany(itm => itm))
                    {
                        asycuda.xcuda_ASYCUDA_ExtendedProperties =
                            ctx.xcuda_ASYCUDA_ExtendedProperties.First(x => x.ASYCUDA_Id == asycuda.ASYCUDA_Id);
                    }

                    lst = res
                        .Where(x => x.Key != null && x.Count() > 1
                             || x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.FileNumber == docSet.LastFileNumber)  
                            
                            )
                        .ToList();

                    return (docSet, lst);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
                        docSet.LastFileNumber += 1;
                        docSet.TrackingState = TrackingState.Modified;
                        var prop = ctx.xcuda_ASYCUDA_ExtendedProperties.FirstOrDefault(x =>
                            x.ASYCUDA_Id == doc.ASYCUDA_Id && x.AsycudaDocumentSetId == docSetAsycudaDocumentSetId);
                        if (prop == null) continue;

                        var declarant = ctx.xcuda_Declarant.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        var oldRef = declarant.Number;
                        var letter = oldRef.Substring(oldRef.IndexOf(prop.FileNumber.ToString()) - 1, 1);
                        declarant.Number =
                            declarant.Number?.Replace(prop.FileNumber.ToString(), docSet.LastFileNumber.ToString());

                        var newRef = declarant.Number;
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

        private static void UpdateNameDependentAttachments(int asycudaId, string oldRef, string newRef)
        {
            using (var ctx = new DocumentItemDSContext {StartTracking = true})
            {
                var itms = ctx.xcuda_Attached_documents
                    .Include("xcuda_Attachments.Attachments")
                    .Where(x => x.Attached_document_reference == oldRef)
                    .Where(x => x.xcuda_Item.ASYCUDA_Id == asycudaId)
                    .ToList();

                foreach (var itm in itms)
                {
                    itm.Attached_document_reference = newRef;
                    foreach (var att in itm.xcuda_Attachments.Where(x => x.Attachments.Reference == oldRef)
                                 .ToList())
                    {
                        att.Attachments.Reference = newRef;
                        att.Attachments.FilePath = att.Attachments.FilePath.Replace(oldRef, newRef);
                    }
                }

                ctx.SaveChanges();
            }
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