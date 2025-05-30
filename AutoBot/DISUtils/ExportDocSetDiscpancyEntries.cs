using System;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Need using statement for ExportDocSetSalesReportUtils if it's in a different namespace

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void ExportDocSetDiscpancyEntries(string adjustmentType, FileTypes fileType)
        {
            try
            {
                Console.WriteLine("Export Discrepancy Entries");
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Result; // Assuming GetAsycudaDocumentSet exists

                // Assuming ExportDocSetSalesReportUtils.ExportDocSetSalesReport exists
                ExportDocSetSalesReportUtils.ExportDocSetSalesReport(docSet.AsycudaDocumentSetId, // Potential NullReferenceException if docSet is null
                    BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number)).Wait(); // Assuming GetDocSetDirectoryName exists

                BaseDataModel.Instance.ExportDocSet(docSet.AsycudaDocumentSetId, // Potential NullReferenceException if docSet is null
                    BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number), true).Wait(); // Assuming ExportDocSet exists


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}