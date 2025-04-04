using System.Linq;
using System.Windows.Forms; // For Application.Exit()
using WaterNut.Business.Entities; // Assuming DocumentCT is here
// Need using statements for CreateEX9Utils, DocumentUtils, ImportWarehouseErrorsUtils, PDFUtils, SubmitSalesXmlToCustomsUtils, EntryDocSetUtils if they are in different namespaces

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void RecreateEx9(int months)
        {
            // This calls CreateEX9Utils.CreateEx9, which needs to be accessible
            var genDocs = CreateEX9Utils.CreateEx9(true, months);

            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                // These call methods within this class, which need to be in their own partial classes
                ExportEx9Entries(months);
                AssessEx9Entries(months);
                DownloadSalesFiles(10, "IM7", false);
                // These call methods in other Utils classes, which need to be accessible
                DocumentUtils.ImportSalesEntries(true);
                ImportWarehouseErrorsUtils.ImportWarehouseErrors(months);
                RecreateEx9(months); // Recursive call
                Application.Exit();
            }
            else // reimport and submit to customs
            {
                // These call methods in other Utils classes, which need to be accessible
                PDFUtils.LinkPDFs();
                SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(months);
                EntryDocSetUtils.CleanupEntries();
                Application.Exit();
            }
        }
    }
}