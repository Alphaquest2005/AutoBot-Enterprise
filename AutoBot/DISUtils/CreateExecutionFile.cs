using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitDiscrepanciesToCustoms, TODO_DiscrepanciesExecutionReport are here

namespace AutoBot
{
    // Assuming DiscpancyExecData is defined elsewhere (e.g., CreateDiscrepancyEmail.cs) or needs moving

    public partial class DISUtils
    {
        private static (string executionFile, List<DiscpancyExecData> execData) CreateExecutionFile(string directory, IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data)
        {
            var executionFile = Path.Combine(directory, $"ExecutionReport.csv");
            var execData = new CoreEntitiesContext().TODO_DiscrepanciesExecutionReport
                .Where(x => x.EmailId == data.Key)
                .Select(x => new DiscpancyExecData()
                {
                    InvoiceNo = x.InvoiceNo,
                    InvoiceDate = x.InvoiceDate,
                    ItemNumber = x.ItemNumber,
                    InvoiceQty = x.InvoiceQty,
                    ReceivedQty = x.ReceivedQty,
                    CNumber = x.CNumber,
                    Status = x.Status,
                    xCNumber = x.xCNumber,
                    xLineNumber = x.xLineNumber,
                    xRegistrationDate = x.xRegistrationDate.ToString(), // Potential NullReferenceException
                    EmailId = x.EmailId,
                    ApplicationSettingsId = x.ApplicationSettingsId,
                    AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                    DocumentType = x.DocumentType,
                    ReferenceNumber = x.Declarant_Reference_Number,
                    CustomsProcedure = x.CustomsProcedure,
                    ASYCUDA_Id = x.ASYCUDA_Id

                })
                .ToList();

            var exeRes =
                new ExportToCSV<DiscpancyExecData, List<DiscpancyExecData>>()
                {
                    dataToPrint = execData
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => exeRes.SaveReport(executionFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }

            return (executionFile, execData);
        }
    }
}