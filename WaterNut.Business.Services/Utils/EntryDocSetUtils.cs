using System.Threading.Tasks;
﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class EntryDocSetUtils
    {
        public static async Task<(AsycudaDocumentSet docSet, List<IGrouping<string, xcuda_ASYCUDA>> lst)>
            GetDuplicateDocuments(int docKey)
        {
            try
            {
 
                var docSet = await GetAsycudaDocumentSet(docKey).ConfigureAwait(false);
 
                var res = await GetRelatedDocuments(docSet).ConfigureAwait(false);
 
                var lst = FilterForDuplicateDocuments(res, docSet);
 
                return (docSet, lst);
 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int docKey)
        {
            using (var ctx = new DocumentDSContext())
            {
                return await ctx.AsycudaDocumentSets.Include("xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA")
                    .FirstAsync(x => x.AsycudaDocumentSetId == docKey).ConfigureAwait(false);
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

        public static async Task<List<IGrouping<string, xcuda_ASYCUDA>>> GetRelatedDocuments(AsycudaDocumentSet docSet)
        {
 
            using (var ctx = new DocumentDSContext())
            {
                var res = await ctx.xcuda_ASYCUDA
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
                    .ToListAsync() // must load first to get the navigation properties
                    .ConfigureAwait(false);
                return res.GroupBy(x => x.xcuda_Declarant.Number).ToList();
 
            }
        }

        public static async Task RenameDuplicateDocuments(List<IGrouping<string, xcuda_ASYCUDA>> lst,
            AsycudaDocumentSet docSet) // Removed 'ref' keyword
        {
            try
            {
                var docSetAsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                using (var ctx = new DocumentDSContext { StartTracking = true })
                {
                    foreach (var g in lst)
                    foreach (var doc in g.Where(x => x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false).ToList())
                    {
                        //AppendDocSetLastFileNumber(docSet);
                        var prop = await GetXcudaAsycudaExtendedProperties(doc, docSetAsycudaDocumentSetId).ConfigureAwait(false);
                        var declarant = ctx.xcuda_Declarant.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        var oldRef = declarant.Number;
                        var newRef = GetNewReference(docSet, declarant, ctx);
                        declarant.Number = newRef;
                        declarant.TrackingState = TrackingState.Modified;
                        //prop.FileNumber = docSet.LastFileNumber;
                        ctx.SaveChanges();
                        ctx.ApplyChanges(docSet);
                        ctx.SaveChanges();
 
                        await UpdateNameDependentAttachments(prop.ASYCUDA_Id, oldRef, newRef).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string GetNewReference(AsycudaDocumentSet docSet, xcuda_Declarant declarant, DocumentDSContext ctx)
        {
            var newNum = Convert.ToInt32(Regex.Match(declarant.Number, @"\d+(?=[^\d]*$)").Value);

            string newRef;
            while (true)
            {
                newNum += 1;
                newRef = Regex.Replace(declarant.Number, @"\d+(?=[^\d]*$)", newNum.ToString());
                if (!ctx.xcuda_Declarant.Any(x => x.Number == newRef)) break;
            }

            //newRef = Regex.Replace(declarant.Number, @"\d+(?=[^\d]*$)", docSet.LastFileNumber.ToString());
            return newRef;
        }

        private static async Task<xcuda_ASYCUDA_ExtendedProperties> GetXcudaAsycudaExtendedProperties(xcuda_ASYCUDA doc,
            int docSetAsycudaDocumentSetId)
        {
            try
            {
                using (var ctx = new DocumentDSContext { StartTracking = true })
                {
                    return await ctx.xcuda_ASYCUDA_ExtendedProperties.FirstAsync(x =>
                        x.ASYCUDA_Id == doc.ASYCUDA_Id).ConfigureAwait(false); //&& x.AsycudaDocumentSetId == docSetAsycudaDocumentSetId
                }
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

        private static async Task UpdateNameDependentAttachments(int asycudaId, string oldRef, string newRef)
        {
            using (var ctx = new DocumentItemDSContext { StartTracking = true })
            {
                var itms = await GetAttachementItems(asycudaId, oldRef, ctx).ConfigureAwait(false);
 
                foreach (var itm in itms)
                {
                    UpdateAttachments(oldRef, newRef, itm);
                }
 
                await ctx.SaveChangesAsync().ConfigureAwait(false);
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

        private static async Task<List<xcuda_Attached_documents>> GetAttachementItems(int asycudaId, string oldRef,
            DocumentItemDSContext ctx)
        {
            return await ctx.xcuda_Attached_documents
                .Include("xcuda_Attachments.Attachments")
                .Where(x => x.Attached_document_reference == oldRef)
                .Where(x => x.xcuda_Item.ASYCUDA_Id == asycudaId)
                .ToListAsync().ConfigureAwait(false);
        }

        public static async Task<AsycudaDocumentSet> GetLatestDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<AsycudaDocumentSet> GetDocSet(string Reference)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Declarant_Reference_Number.Contains(Reference))
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<List<AsycudaDocumentSet>> GetLatestModifiedDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Any(z => z.ImportComplete == false))
                    .OrderByDescending(x => x.xcuda_ASYCUDA_ExtendedProperties.Max(z => z.ASYCUDA_Id))
                    .ToListAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<(DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)> CreateMonthYearAsycudaDocSet(
            DateTime startDate)
        {
            var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
            var docRef = startDate.ToString("MMMM") + " " + startDate.Year;
            var docSet = await GetAsycudaDocumentSet(docRef, false).ConfigureAwait(false);
 
            var dirPath =
                StringExtensions.UpdateToCurrentUser(
                    BaseDataModel.GetDocSetDirectoryName(docRef));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return (StartDate: startDate, EndDate: endDate, DocSet: docSet, DirPath: dirPath);
        }

        public static async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(string docRef, bool isSystemDocSet)
        {
            AsycudaDocumentSet docSet;
            using (var ctx = new DocumentDSContext()) // Add using for proper disposal
            {
                docSet = ctx.AsycudaDocumentSets.FirstOrDefault(x => // Use FirstOrDefaultAsync
                    x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
            }
            if (docSet == null) docSet = await CreateAsycudaDocumentSet(docRef, isSystemDocSet).ConfigureAwait(false);
            return docSet;
        }

        public static async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(string docRef, bool isSystemDocSet)
        {
            try
            {


                AsycudaDocumentSet docSet;
                var defaultCustomsOperation = BaseDataModel.GetDefaultCustomsOperation();
                using (var ctx = new DocumentDSContext())
                {
                    var doctype = BaseDataModel.Instance.Customs_Procedures
                        .First(x =>
                        {
                            
                            return x.CustomsOperationId == defaultCustomsOperation
                                   && x.IsDefault == true;
                        }); //&& x.Discrepancy != true
                    await ctx.Database.ExecuteSqlCommandAsync($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{docRef}',{doctype.Customs_ProcedureId},0)").ConfigureAwait(false);

                    docSet = await ctx.AsycudaDocumentSets.FirstOrDefaultAsync(x =>
                        x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);

                    if (!isSystemDocSet) return docSet;
                    ctx.SystemDocumentSets.Add(new SystemDocumentSet(true)
                        { AsycudaDocumentSet = docSet, TrackingState = TrackingState.Added });
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
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