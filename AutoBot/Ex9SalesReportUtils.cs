using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationQS.Business.Entities;

namespace AutoBot
{
    public class Ex9SalesReportUtils
    {
        public static async Task<ObservableCollection<EX9Utils.SaleReportLine>> Ex9SalesReport(int ASYCUDA_Id)
        {
            try
            {
                var alst = GetAsycudaSalesAllocationsExes(ASYCUDA_Id);
                var d = CreateSaleReportLines(alst);
                return new ObservableCollection<EX9Utils.SaleReportLine>(d);

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private static IEnumerable<EX9Utils.SaleReportLine> CreateSaleReportLines(List<AsycudaSalesAllocationsEx> alst) =>
            alst.Where(x => x.xLineNumber != null)
                .Where(x => !string.IsNullOrEmpty(x.pCNumber)) // prevent pre assessed entries
                .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                .OrderBy(s => s.xLineNumber)
                .ThenBy(s => s.InvoiceNo)
                .Select(s => new EX9Utils.SaleReportLine
                {
                    Line = Convert.ToInt32(s.xLineNumber),
                    Date = Convert.ToDateTime(s.InvoiceDate),
                    InvoiceNo = s.InvoiceNo,
                    CustomerName = s.CustomerName.StartsWith("- ") ? s.CustomerName.Substring("- ".Length) : s.CustomerName,
                    ItemNumber = s.ItemNumber,
                    ItemDescription = s.ItemDescription,
                    TariffCode = s.TariffCode,
                    SalesFactor = Convert.ToDouble(s.SalesFactor),
                    SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                    xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                    Price = Convert.ToDouble(s.Cost),
                    SalesType = s.DutyFreePaid,
                    GrossSales = Convert.ToDouble(s.TotalValue),
                    PreviousCNumber = s.pCNumber,
                    PreviousLineNumber = s.pLineNumber.ToString(),
                    PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                    CIFValue =
                        (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                        Convert.ToDouble(s.QtyAllocated),
                    DutyLiablity =
                        (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                        Convert.ToDouble(s.QtyAllocated),
                    //Comments = s.Comments
                }).Distinct();

        private static List<AsycudaSalesAllocationsEx> GetAsycudaSalesAllocationsExes(int ASYCUDA_Id) =>
            new AllocationQSContext().AsycudaSalesAllocationsExs.Where(
                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null").ToList();
    }
}