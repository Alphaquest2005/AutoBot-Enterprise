using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using WaterNut.Business.Entities; // Assuming DocumentCT is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Need using statement for CreateEX9Utils if it's in a different namespace

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void RecreateEx9(FileTypes filetype, FileInfo[] files)
        {
            // This calls CreateEX9Utils.CreateEx9, which needs to be accessible
            var genDocs = CreateEX9Utils.CreateEx9(true, -1);
            var saleInfo = BaseDataModel.CurrentSalesInfo(-1); // Assuming CurrentSalesInfo exists
            filetype.AsycudaDocumentSetId = saleInfo.DocSet.AsycudaDocumentSetId; // Potential NullReferenceException
            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                // Assuming ProcessNextStep exists on FileTypes and is a List<string>
                filetype.ProcessNextStep.AddRange(new List<string>() { "ExportEx9Entries", "AssessEx9Entries", "DownloadPOFiles", "ImportSalesEntries", "ImportWarehouseErrors", "RecreateEx9" });
            }
            else // reimport and submit to customs
            {
                // Assuming ProcessNextStep exists on FileTypes and is a List<string>
                filetype.ProcessNextStep.AddRange(new List<string>() { "LinkPDFs", "SubmitToCustoms", "CleanupEntries", "Kill" });
            }
        }
    }
}