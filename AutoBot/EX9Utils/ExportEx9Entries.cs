using System;
using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Need using statement for ExportDocSetSalesReportUtils if it's in a different namespace

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void ExportEx9Entries(int months)
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var saleInfo =  BaseDataModel.CurrentSalesInfo(months); // Assuming CurrentSalesInfo exists

                // Assuming ExportDocSetSalesReportUtils.ExportDocSetSalesReport exists
                ExportDocSetSalesReportUtils.ExportDocSetSalesReport(saleInfo.DocSet.AsycudaDocumentSetId, // Potential NullReferenceException
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number)).Wait(); // Assuming GetDocSetDirectoryName exists

                BaseDataModel.Instance.ExportDocSet(saleInfo.DocSet.AsycudaDocumentSetId, // Potential NullReferenceException
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