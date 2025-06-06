using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_PODocSetToExport are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void ClearPOEntries()
        {
            Console.WriteLine("Clear PO Entries");

            // var saleInfo = CurrentSalesInfo(); // Assuming CurrentSalesInfo exists if needed

            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_PODocSetToExport.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait(); // Assuming ClearAsycudaDocumentSet exists
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0); // Assuming UpdateAsycudaDocumentSetLastNumber exists
                }
            }
        }
    }
}