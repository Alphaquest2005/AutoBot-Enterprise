using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSetToExport are here
using DocumentDS.Business.Entities; // Assuming DocumentDSContext, xcuda_ASYCUDA are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void ExportPOEntries(int asycudaDocumentSetId)
        {
            using (var ctx = new DocumentDSContext())
            {
                try
                {
                    IQueryable<xcuda_ASYCUDA> docs;
                    var defaultCustomsOperation = BaseDataModel.GetDefaultCustomsOperation(); // Assuming GetDefaultCustomsOperation exists
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true)
                    {
                        if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                                x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                        Console.WriteLine("Export PO Entries");
                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)
                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        &&  x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == defaultCustomsOperation);
                    }
                    else
                    {
                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)
                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == defaultCustomsOperation);
                    }

                    var res = docs.GroupBy(x => new
                        {
                            x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId,
                            ReferenceNumber = x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                .Declarant_Reference_Number
                        }).Select(x => new
                        {
                            DocSet = x.Key,
                            Entrylst = x.Select(z => new { z, ReferenceNumber = z.xcuda_Declarant.Number }).ToList()
                        })
                        .ToList();


                    foreach (var docSetId in res)
                    {
                        var directoryName = BaseDataModel.GetDocSetDirectoryName(docSetId.DocSet.ReferenceNumber); // Assuming GetDocSetDirectoryName exists
                        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName); //continue;
                        if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                            File.Delete(Path.Combine(directoryName, "Instructions.txt"));
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            //if (File.Exists(expectedfileName)) continue;
                            BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).Wait(); // Assuming ExportDocument exists
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    BaseDataModel.EmailExceptionHandler(ex); // Assuming EmailExceptionHandler exists
                    throw;
                }
            }
        }
    }
}