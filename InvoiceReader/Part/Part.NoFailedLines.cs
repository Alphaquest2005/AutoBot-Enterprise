using System.Linq;

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        private bool NoFailedLines()
        {
            int? partId = this.OCR_Part?.Id;
            string methodName = nameof(NoFailedLines);
            _logger.Verbose("Entering {MethodName} for PartId: {PartId}", methodName, partId);

            // Access FailedLines property which has its own logging
            var failedLinesList = this.FailedLines;
            bool noFailed = !failedLinesList.Any();
            _logger.Verbose(
                "{MethodName}: Evaluation result for PartId: {PartId}: {Result} (Based on FailedLines count: {Count})",
                methodName, partId, noFailed, failedLinesList.Count);

            _logger.Verbose("Exiting {MethodName} for PartId: {PartId}", methodName, partId);
            return noFailed;
        }
    }
}