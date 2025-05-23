using System;
using System.IO;
using System.Linq;
using AutoBotUtilities.CSV; // Assuming CSVUtils is here
using Core.Common.Utils; // Assuming FormatedSpace extension method is here
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_EntriesExpiringNextMonth, Contacts are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void EmailEntriesExpiringNextMonth()
        {
            var info = BaseDataModel.CurrentSalesInfo(-1); // Assuming CurrentSalesInfo exists
            var directory = info.Item4; // Potential NullReferenceException
            var errorfile = Path.Combine(directory, "EntriesExpiringNextMonth.csv");

            using (var ctx = new CoreEntitiesContext())
            {
                var errors = ctx.TODO_EntriesExpiringNextMonth
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

                CSVUtils.SaveCSVReport(errors, errorfile); // Assuming SaveCSVReport exists

                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                {
                    var body = "The following entries are expiring within the next month. \r\n" +
                               $"Start Date: {DateTime.Now:yyyy-MM-dd} End Date {DateTime.Now.AddMonths(1):yyyy-MM-dd}: \r\n" +
                               $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"Document Type".FormatedSpace(20)}{"RegistrationDate".FormatedSpace(20)}{"ExpiryDate".FormatedSpace(20)}\r\n" +
                               // Aggregate might throw InvalidOperationException if errors is empty
                               $"{(errors.Any() ? errors.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.Reference.FormatedSpace(20)}{current.DocumentType.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)}{current.ExpiryDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current) : "")}" + // Potential NullReferenceExceptions
                               $"\r\n" +
                               $"{Utils.Client.CompanyName} is kindly requesting these Entries be extended an additional 730 days to facilitate ex-warehousing. \r\n" + // Assuming Utils.Client exists
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Entries Expiring {DateTime.Now:yyyy-MM-dd} - {DateTime.Now.AddMonths(1):yyyy-MM-dd}", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[] // Assuming Utils.Client exists
                    {
                        errorfile
                    });
                }
            }
        }
    }
}