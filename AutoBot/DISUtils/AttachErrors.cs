using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming SubmitDiscrepanciesErrorReport is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<string> AttachErrors(List<SubmitDiscrepanciesErrorReport> errors, string directory)
        {
            var pdfs = new List<string>();
            var errorFile = Path.Combine(directory, $"ErrorReport.csv");
            var errRes =
                new ExportToCSV<SubmitDiscrepanciesErrorReport, List<SubmitDiscrepanciesErrorReport>>()
                {
                    dataToPrint = errors
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => errRes.SaveReport(errorFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }

            pdfs.Add(errorFile);
            return pdfs;
        }
    }
}