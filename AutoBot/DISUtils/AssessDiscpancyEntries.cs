using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, AsycudaDocumentSetExs are here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void AssessDiscpancyEntries(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var doc = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    // This calls AssessDISEntries, which needs to be in its own partial class
                    if (doc != null) AssessDISEntries(doc.Declarant_Reference_Number);
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