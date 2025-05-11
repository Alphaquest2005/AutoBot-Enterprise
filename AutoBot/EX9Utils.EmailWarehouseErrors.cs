using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using CoreEntities.Business.Entities;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.DataSpace;

namespace AutoBot;

public partial class EX9Utils
{
    public static async Task EmailWarehouseErrors()
    {
        var info = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
        var directory = info.Item4;
        var errorfile = Path.Combine(directory, "WarehouseErrors.csv");
        if (File.Exists(errorfile)) return;
        using (var ctx = new CoreEntitiesContext())
        {
            var errors = ctx.TODO_ERRReport_SubmitWarehouseErrors
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

            var res =
                new ExportToCSV<TODO_ERRReport_SubmitWarehouseErrors, List<TODO_ERRReport_SubmitWarehouseErrors>>();
            res.IgnoreFields.AddRange(typeof(IIdentifiableEntity).GetProperties());
            res.IgnoreFields.AddRange(typeof(IEntityWithKey).GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackable).GetProperties());
            res.IgnoreFields.AddRange(
                typeof(Core.Common.Business.Entities.BaseEntity<TODO_ERRReport_SubmitWarehouseErrors>).GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackingCollection<TODO_ERRReport_SubmitWarehouseErrors>)
                .GetProperties());
            res.dataToPrint = errors;
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None,
                    sta).ConfigureAwait(false);
            }

            var contacts = ctx.Contacts
                .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .ToList();
            if (File.Exists(errorfile))
            {
                var body =
                    "Attached are Issues that have been found on Assessed Entries that prevent Ex-warehousing. \r\n" +
                    $"{Utils.Client.CompanyName} is kindly requesting Technical Assistance in resolving these issues to facilitate Ex-Warehousing. \r\n" +
                    $"\r\n" +
                    $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                    $"\r\n" +
                    $"Regards,\r\n" +
                    $"AutoBot";
                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory, $"Warehouse Errors",
                    contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[]
                    {
                        errorfile
                    }).ConfigureAwait(false);
            }
        }
    }
}