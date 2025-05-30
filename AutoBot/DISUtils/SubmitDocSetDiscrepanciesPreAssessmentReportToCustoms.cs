using System;
using System.Collections.Generic;
using System.Data.Entity; // For TransactionalBehavior
using System.Linq;
using AdjustmentQS.Business.Entities; // Assuming AdjustmentQSContext, AdjustmentDetails are here
using CoreEntities.Business.Entities; // Assuming FileTypes, TODO_TotalAdjustmentsToProcess, SubmitDiscrepanciesErrorReport, TODO_DiscrepancyPreExecutionReport are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(FileTypes fileType)
        {
            try
            {
                // These call private methods which need to be in their own partial classes
                var totaladjustments = GetDocSetDiscrepanciesToProcess(fileType.AsycudaDocumentSetId);
                var errors = GetDocSetDiscrepancyErrors(fileType.AsycudaDocumentSetId);
                var goodadj = GetDocSetDiscrepancyPreExecutionReports(fileType.AsycudaDocumentSetId);
                //var errBreakdown = errors.GroupBy(x => x.Error).ToList(); // CS1061 - Assuming ErrorMessage exists
                 var errBreakdown = errors.GroupBy(x => x.comment).ToList(); // Using comment

                var directory = BaseDataModel.GetDocSetDirectoryName("Discrepancies"); // Assuming GetDocSetDirectoryName exists
                // These call private methods which need to be in their own partial classes
                var pdfs = AttachExecutions(goodadj, directory);
                pdfs.AddRange(AttachErrors(errors, directory));

                // This calls a private method which needs to be in its own partial class
                var sent = SendDocSetDiscrepancyEmail(fileType, errors, totaladjustments, errBreakdown, goodadj, pdfs);
                if (sent)
                {
                    using (var ctx = new AdjustmentQSContext())
                    {
                        ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                            $"UPDATE       AdjustmentDetails SET Status = 'Submitted' WHERE AsycudaDocumentSetId = {fileType.AsycudaDocumentSetId}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}