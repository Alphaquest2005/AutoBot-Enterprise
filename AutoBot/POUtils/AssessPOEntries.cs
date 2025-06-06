using System;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_PODocSetToAssess are here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void AssessPOEntries(FileTypes ft)
        {
            Console.WriteLine("Assessing PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSetToAssess
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId) //ft.AsycudaDocumentSetId == 0 ||
                    .ToList();
                foreach (var doc in res)
                {
                    // This calls AssessPOEntry, which needs to be in its own partial class
                    AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                }
            }
            // This calls SubmitAssessPOErrors, which needs to be in its own partial class
            SubmitAssessPOErrors(ft);
        }
    }
}