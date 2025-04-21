using System.Linq;

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        private bool AllRequiredFieldsFilled()
        {
            int? partId = this.OCR_Part?.Id;
            string methodName = nameof(AllRequiredFieldsFilled);
            _logger.Verbose("Entering {MethodName} for PartId: {PartId}", methodName, partId);

            // Access FailedFields property which has its own logging
            var failedFieldsList = this.FailedFields;
            bool hasNoFailedRequiredFields = !failedFieldsList.Any();
            _logger.Verbose(
                "{MethodName}: Evaluation result for PartId: {PartId}: {Result} (Based on FailedFields count: {Count})",
                methodName, partId, hasNoFailedRequiredFields, failedFieldsList.Count);

            _logger.Verbose("Exiting {MethodName} for PartId: {PartId}", methodName, partId);
            return hasNoFailedRequiredFields;
        }
    }
}