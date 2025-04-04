using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSetToExport, ToDo_POToXML are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void RecreatePOEntries(int asycudaDocumentSetId)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (ctx.TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                    }

                    Console.WriteLine("RecreatePOEntries");

                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z => z.EntryDataDetailsId).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        // This calls CreatePOEntries, which needs to be in its own partial class
                        CreatePOEntries(docSetId.DocSetId, docSetId.Entrylst);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}