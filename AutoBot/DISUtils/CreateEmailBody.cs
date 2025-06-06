using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here
using Core.Common.Utils; // Assuming FormatedSpace extension method is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming __ColumnWidth is defined elsewhere or needs moving
        private static readonly int __ColumnWidth = 15;

        private static string CreateEmailBody(List<TODO_SubmitDiscrepanciesToCustoms> cNumbers)
        {
            var body = "The Following Discrepancies Entries were Assessed. \r\n" +
                       $"\t{"pCNumber".FormatedSpace(__ColumnWidth)}{"Reference".FormatedSpace(__ColumnWidth)}{"To Be Paid".FormatedSpace(__ColumnWidth)}{"AssessmentDate".FormatedSpace(__ColumnWidth)}\r\n" +
                       $"{cNumbers.Select(current => $"\t{current.CNumber.FormatedSpace(__ColumnWidth)}{current.ReferenceNumber.FormatedSpace(__ColumnWidth)}{current.ToBePaid.FormatedSpace(__ColumnWidth)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(__ColumnWidth)} \r\n").Aggregate((old, current) => old + current)}" + // Potential NullReferenceException if RegistrationDate is null
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