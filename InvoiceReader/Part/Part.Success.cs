namespace WaterNut.DataSpace;

public partial class Part
{
    public bool Success
    {
        get
        {
            int? partId = this.OCR_Part?.Id;
            string propertyName = nameof(Success);
            _logger.Verbose("Entering {PropertyName} getter for PartId: {PartId}", propertyName, partId);

            // Call helper methods which now contain logging
            bool requiredFieldsOk = AllRequiredFieldsFilled();
            bool noFailedLines = NoFailedLines();
            bool childrenOk = AllChildPartsSucceded();

            bool finalResult = requiredFieldsOk && noFailedLines && childrenOk;
            _logger.Information(
                "{PropertyName} evaluation result for PartId: {PartId}: {Result} (RequiredFieldsOk={RequiredFieldsOk}, NoFailedLines={NoFailedLines}, ChildrenOk={ChildrenOk})",
                propertyName, partId, finalResult, requiredFieldsOk, noFailedLines, childrenOk);

            _logger.Verbose("Exiting {PropertyName} getter for PartId: {PartId}", propertyName, partId);
            return finalResult;
        }
    }
}