using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version
using AutoBot.Services;

namespace AutoBot
{
    public partial class EntryDocSetUtils
    {
        public static void CleanupEntries()
        {
            Console.WriteLine("Cleanup ...");
            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_DocumentsToDelete
                        .Where(x => x.ApplicationSettingsId ==
                                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => new { x.ASYCUDA_Id, x.AsycudaDocumentSetId }).ToList();
                foreach (var itm in lst)
                {
                    using (var dtx = new DocumentDSContext())
                    {
                        var docEds = dtx.AsycudaDocumentEntryDatas.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).ToList();
                        foreach (var ed in docEds)
                        {
                            var docsetEd = dtx.AsycudaDocumentSetEntryDatas.FirstOrDefault(x =>
                                    x.AsycudaDocumentSetId == itm.AsycudaDocumentSetId && x.EntryData_Id == ed.EntryData_Id);
                            if (docsetEd != null) dtx.AsycudaDocumentSetEntryDatas.Remove(docsetEd);
                        }

                        dtx.SaveChanges();
                    }

                    BaseDataModel.Instance.DeleteAsycudaDocument(itm.ASYCUDA_Id).Wait();
                }

                var doclst = ctx.TODO_DeleteDocumentSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.AsycudaDocumentSetId).ToList();

                foreach (var itm in doclst)
                {
                    BaseDataModel.Instance.DeleteAsycudaDocumentSet(itm).Wait();
                }

                //   this wont work because i am saving entrydata in system documentsets

                // ctx.Database.ExecuteSqlCommand(@"delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where AsycudaDocumentSetId is null)
                //                                                                 delete from EntryData where EntryDataId not in (SELECT EntryDataId
                //                                                                 FROM         AsycudaDocumentSetEntryData)");
            }
        }
    }
}