using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBot;

public partial class EX9Utils
{
    public static async Task EmailEntriesExpiringNextMonth()
    {
        var info = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
        var directory = info.Item4;
        var errorfile = Path.Combine(directory, "EntriesExpiringNextMonth.csv");

        using (var ctx = new CoreEntitiesContext())
        {
            var errors = ctx.TODO_EntriesExpiringNextMonth
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

            await CSVUtils.SaveCSVReport(errors, errorfile).ConfigureAwait(false);


            var contacts = ctx.Contacts
                .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .ToList();
            if (File.Exists(errorfile))
            {
                var body = "The following entries are expiring within the next month. \r\n" +
                           $"Start Date: {DateTime.Now:yyyy-MM-dd} End Date {DateTime.Now.AddMonths(1):yyyy-MM-dd}: \r\n" +
                           $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"Document Type".FormatedSpace(20)}{"RegistrationDate".FormatedSpace(20)}{"ExpiryDate".FormatedSpace(20)}\r\n" +
                           $"{errors.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.Reference.FormatedSpace(20)}{current.DocumentType.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)}{current.ExpiryDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                           $"\r\n" +
                           $"{Utils.Client.CompanyName} is kindly requesting these Entries be extended an additional 730 days to facilitate ex-warehousing. \r\n" +
                           $"\r\n" +
                           $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                           $"\r\n" +
                           $"Regards,\r\n" +
                           $"AutoBot";
                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory,
                    $"Entries Expiring {DateTime.Now:yyyy-MM-dd} - {DateTime.Now.AddMonths(1):yyyy-MM-dd}",
                    contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[]
                    {
                        errorfile
                    }).ConfigureAwait(false);
            }
        }
    }
}