using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here

namespace AutoBot
{
    // Assuming SubmitEntryData is defined elsewhere or needs moving
    public class SubmitEntryData
    {
        public string CNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string DocumentType { get; set; }
        public string CustomsProcedure { get; set; }
        public System.DateTime? RegistrationDate { get; set; }
        public System.DateTime? AssessmentDate { get; set; }
    }

    public partial class DISUtils
    {
        private static string CreateSummaryFile(List<TODO_SubmitDiscrepanciesToCustoms> emailIds, string directory)
        {
            var summaryFile = Path.Combine(directory, $"Summary.csv");
            var sumData = emailIds
                .Select(x => new SubmitEntryData()
                {
                    CNumber = x.CNumber,
                    ReferenceNumber = x.ReferenceNumber,
                    DocumentType = x.DocumentType,
                    CustomsProcedure = x.CustomsProcedure,
                    RegistrationDate = x.RegistrationDate,
                    AssessmentDate = x.AssessmentDate
                })
                .ToList();

            var sumres =
                new ExportToCSV<SubmitEntryData, List<SubmitEntryData>>()
                {
                    dataToPrint = sumData
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }
            return summaryFile;
        }
    }
}