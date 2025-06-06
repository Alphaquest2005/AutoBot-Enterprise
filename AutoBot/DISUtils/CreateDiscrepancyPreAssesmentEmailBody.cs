using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils; // Assuming FormatedSpace extension method is here
using CoreEntities.Business.Entities; // Assuming TODO_TotalAdjustmentsToProcess, SubmitDiscrepanciesErrorReport, TODO_DiscrepancyPreExecutionReport are here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming __ColumnWidth is defined elsewhere or needs moving
        private static readonly int __ColumnWidth = 15;

        private static string CreateDiscrepancyPreAssesmentEmailBody(List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<TODO_DiscrepancyPreExecutionReport> goodadj)
        {
            var body = "The Following Discrepancies Entries were Assessed. \r\n" +
                       $"\t{"Total Discrepancies".FormatedSpace(__ColumnWidth)}{"Total Errors".FormatedSpace(__ColumnWidth)}{"Total Good".FormatedSpace(__ColumnWidth)}\r\n" +
                       $"\t{totaladjustments.Count.ToString().FormatedSpace(__ColumnWidth)}{errBreakdown.Sum(x => x.Count()).ToString().FormatedSpace(__ColumnWidth)}{goodadj.Count.ToString().FormatedSpace(__ColumnWidth)}\r\n" +
                       $"\r\n" +
                       $"Error Breakdown\r\n" +
                       $"\t{"Error".FormatedSpace(__ColumnWidth)}{"Count".FormatedSpace(__ColumnWidth)}\r\n" +
                       // Aggregate might throw InvalidOperationException if errBreakdown is empty
                       $"{(errBreakdown.Any() ? errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(__ColumnWidth)}{current.Count().ToString().FormatedSpace(__ColumnWidth)} \r\n").Aggregate((old, current) => old + current) : "")}" +
                       $"\r\n" +
                       $"Please open the attached email to view Email Thread.\r\n" +
                       $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                       $"\r\n" +
                       $"Regards,\r\n" +
                       $"AutoBot";
            return body;
        }
    }
}