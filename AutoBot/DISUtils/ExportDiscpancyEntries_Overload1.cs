using System;
using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Need using statement for ExportDocSetSalesReportUtils if it's in a different namespace

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void ExportDiscpancyEntries(string adjustmentType)
        {
            try
            {
                Console.WriteLine("Export Discrepancy Entries");
                var saleInfo = BaseDataModel.CurrentSalesInfo(0); // Assuming CurrentSalesInfo exists

                // Assuming ExportDocSetSalesReportUtils.ExportDocSetSalesReport exists
                ExportDocSetSalesReportUtils.ExportDocSetSalesReport(saleInfo.DocSet.AsycudaDocumentSetId, // Potential NullReferenceException if saleInfo or DocSet is null
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number)).Wait(); // Assuming GetDocSetDirectoryName exists

                BaseDataModel.Instance.ExportDocSet(saleInfo.DocSet.AsycudaDocumentSetId, // Potential NullReferenceException if saleInfo or DocSet is null
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number), true).Wait(); // Assuming ExportDocSet exists


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}