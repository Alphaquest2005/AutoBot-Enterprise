using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming TODO_DiscrepancyPreExecutionReport is here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<string> AttachExecutions(List<TODO_DiscrepancyPreExecutionReport> goodadj, string directory)
        {
            var pdfs = new List<string>();
            var executionFile = Path.Combine(directory, $"ExecutionReport.csv");
            var exeRes =
                new ExportToCSV<TODO_DiscrepancyPreExecutionReport, List<TODO_DiscrepancyPreExecutionReport>>()
                {
                    dataToPrint = goodadj
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => exeRes.SaveReport(executionFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }

            pdfs.Add(executionFile);
            return pdfs;
        }
    }
}